using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using ScriptCs.Contracts;

namespace ScriptCs.Arduino {
    public class Arduino : IDisposable, IScriptPackContext {
        private const int MaxDataBytes = 32;

        private const int DigitalMessage = 0x90; // send data for a digital port
        private const int AnalogMessage = 0xE0; // send data for an analog pin (or PWM)
        private const int ReportAnalog = 0xC0; // enable analog input by pin #
        private const int ReportDigital = 0xD0; // enable digital input by port
        private const int SetPinMode = 0xF4; // set a pin to INPUT/OUTPUT/PWM/etc
        private const int ReportVersion = 0xF9; // report firmware version
        private const int SystemReset = 0xFF; // reset from MIDI
        private const int StartSysex = 0xF0; // start a MIDI SysEx message
        private const int EndSysex = 0xF7; // end a MIDI SysEx message
        private readonly object Locked = new Object();
        private readonly int[] StoredInputData = new int[MaxDataBytes];
        private volatile int[] _analogInputData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private int _delay;
        private volatile int[] _digitalInputData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private volatile int[] _digitalOutputData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private int _executeMultiByteCommand;
        private int _multiByteChannel;

        private Action _onBoardReady;

        private bool _parsingSysex;

        private Thread _readThread;
        private SerialPort _serialPort;
        private int _sysexBytesRead;
        private int _waitForData;

        public Arduino()
        {
            IsReady = false;
            _onBoardReady = () =>
            {
                Log("Arduino is ready!");
            };
        }

        public Action OnBoardReady {
            get { return _onBoardReady; }
            set {
                _onBoardReady += value;
                if (IsReady) {
                    Log("Board is already ready. Executing Callback!");
                    value();
                }
            }
        }

        public bool Debug { get; set; }
        public bool IsReady { get; private set; }

        public void Dispose() {
            Close();
        }

        /// <summary>
        ///     Setup serial port
        /// </summary>
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4. Default: Last Serial Port Name</param>
        /// <param name="baudRate">The baud rate of the communication. Default 115200</param>
        /// <param name="delay">
        ///     Time delay that may be required to allow some arduino models to reboot after opening a serial
        ///     connection
        /// </param>
        public void Setup(string serialPortName = "", Int32 baudRate = 57600, int delay = 2000) {
            IsReady = false;
            if (_serialPort != null && _serialPort.IsOpen) {
                Close();
            }
            if (String.IsNullOrEmpty(serialPortName)) {
                serialPortName = List().Last();
            }
            _serialPort = new SerialPort(serialPortName, baudRate) {
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };
            _delay = delay;
            Log(String.Format("Arduino configured: Serial Port Name: {0} / Baud Rate: {1} / Delay: {2}", serialPortName, baudRate, delay));
            Open();
        }

        /// <summary>
        ///     Opens the serial port connection, should it be required. By default the port is
        ///     opened when the object is first created.
        /// </summary>
        private void Open() {
            Log("Opening serial port");
            try {
                _serialPort.Open();
            }
            catch (Exception e) {
                Log(String.Format("Could not open serial port. {0}", e.Message));
                if (_serialPort != null && _serialPort.IsOpen) {
                    _serialPort.Close();
                }
                throw;
            }
            Log("Serial port opened");
            Thread.Sleep(_delay);

            byte[] command = new byte[2];

            for (int i = 0; i < 6; i++) {
                command[0] = (byte) (ReportAnalog | i);
                command[1] = 1;
                _serialPort.Write(command, 0, 2);
            }

            for (int i = 0; i < 2; i++) {
                command[0] = (byte) (ReportDigital | i);
                command[1] = 1;
                _serialPort.Write(command, 0, 2);
            }

            if (_readThread != null)
                return;

            _readThread = new Thread(ProcessInput);
            _readThread.Start();
            OnBoardReady();
            IsReady = true;
        }

        /// <summary>
        ///     Closes the serial port.
        /// </summary>
        public void Close() {
            Log("Closing serial port");
            if (_readThread != null) {
                _readThread.Join(500);
                _readThread = null;
            }
            if (_serialPort != null && _serialPort.IsOpen) {
                _serialPort.Close();
            }

            Log("Serial port closed");
        }

        /// <summary>
        ///     Lists all available serial ports on current system.
        /// </summary>
        /// <returns>An array of strings containing all available serial ports.</returns>
        public IEnumerable<string> List() {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        ///     Returns the last known state of the digital pin.
        /// </summary>
        /// <param name="pin">The arduino digital input pin.</param>
        /// <returns>Arduino.HIGH or Arduino.LOW</returns>
        public int DigitalRead(int pin) {
            Log(String.Format("(digital) Reading pin {0}", pin));
            return (_digitalInputData[pin >> 3] >> (pin & 0x07)) & 0x01;
        }

        /// <summary>
        ///     Returns the last known state of the analog pin.
        /// </summary>
        /// <param name="pin">The arduino analog input pin.</param>
        /// <returns>A value representing the analog value between 0 (0V) and 1023 (5V).</returns>
        public int AnalogRead(int pin) {
            Log(String.Format("(analog) Reading pin {0}", pin));
            return _analogInputData[pin];
        }

        /// <summary>
        ///     Sets the mode of the specified pin (INPUT or OUTPUT).
        /// </summary>
        /// <param name="pin">The arduino pin.</param>
        /// <param name="mode">Mode Arduino.INPUT or Arduino.OUTPUT.</param>
        public void PinMode(int pin, PinMode mode) {
            var message = new byte[3];
            message[0] = SetPinMode;
            message[1] = (byte) (pin);
            message[2] = (byte) ((int) mode);
            _serialPort.Write(message, 0, 3);
        }

        /// <summary>
        ///     Write to a digital pin that has been toggled to output mode with pinMode() method.
        /// </summary>
        /// <param name="pin">The digital pin to write to.</param>
        /// <param name="value">Value either Arduino.LOW or Arduino.HIGH.</param>
        public void DigitalWrite(int pin, DigitalPin value) {
            var intValue = (int) value;
            int portNumber = (pin >> 3) & 0x0F;
            Log(String.Format("(digital) Writing value {0} on pin {1} on port number {2}", value, pin, portNumber));
            var message = new byte[3];

            if (intValue == 0)
                _digitalOutputData[portNumber] &= ~(1 << (pin & 0x07));
            else
                _digitalOutputData[portNumber] |= (1 << (pin & 0x07));

            message[0] = (byte) (DigitalMessage | portNumber);
            message[1] = (byte) (_digitalOutputData[portNumber] & 0x7F);
            message[2] = (byte) (_digitalOutputData[portNumber] >> 7);
            _serialPort.Write(message, 0, 3);
        }

        /// <summary>
        ///     Write to an analog pin using Pulse-width modulation (PWM).
        /// </summary>
        /// <param name="pin">Analog output pin.</param>
        /// <param name="value">PWM frequency from 0 (always off) to 255 (always on).</param>
        public void AnalogWrite(int pin, int value) {
            Log(String.Format("(analog) Writing value {0} on pin {1}", value, pin));

            var message = new byte[3];
            message[0] = (byte) (AnalogMessage | (pin & 0x0F));
            message[1] = (byte) (value & 0x7F);
            message[2] = (byte) (value >> 7);
            _serialPort.Write(message, 0, 3);
        }

        private void SetDigitalInputs(int portNumber, int portData) {
            _digitalInputData[portNumber] = portData;
        }

        private void SetAnalogInput(int pin, int value) {
            _analogInputData[pin] = value;
        }

        private void ProcessInput() {
            while (_serialPort.IsOpen) {
                if (_serialPort.BytesToRead <= 0)
                    continue;

                lock (Locked) {
                    int inputData = _serialPort.ReadByte();

                    if (_parsingSysex) {
                        if (inputData == EndSysex) {
                            _parsingSysex = false;
                        }
                        else {
                            StoredInputData[_sysexBytesRead] = inputData;
                            _sysexBytesRead++;
                        }
                    }
                    else if (_waitForData > 0 && inputData < 128) {
                        _waitForData--;
                        StoredInputData[_waitForData] = inputData;

                        if (_executeMultiByteCommand == 0 || _waitForData != 0)
                            continue;

                        //we got everything
                        switch (_executeMultiByteCommand) {
                            case DigitalMessage:
                                SetDigitalInputs(_multiByteChannel, (StoredInputData[0] << 7) + StoredInputData[1]);
                                break;
                            case AnalogMessage:
                                SetAnalogInput(_multiByteChannel, (StoredInputData[0] << 7) + StoredInputData[1]);
                                break;
                            case ReportVersion:
                                break;
                        }
                    }
                    else {
                        int command;
                        if (inputData < 0xF0) {
                            command = inputData & 0xF0;
                            _multiByteChannel = inputData & 0x0F;
                        }
                        else {
                            command = inputData;
                            // commands in the 0xF* range don't use channel data
                        }
                        switch (command) {
                            case DigitalMessage:

                            case AnalogMessage:
                            case ReportVersion:
                                _waitForData = 2;
                                _executeMultiByteCommand = command;
                                break;
                        }
                    }
                }
            }
        }

        private void Log(string message) {
            if (!Debug) {
                return;
            }
            Console.WriteLine(message);
        }
    }
}