using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Threading;
using ScriptCs.Contracts;

namespace ScriptCs.Arduino {
    public class Arduino : IScriptPackContext {
        public static int INPUT = 0;
        public static int OUTPUT = 1;
        public static int LOW = 0;
        public static int HIGH = 1;

        private const int MAX_DATA_BYTES = 32;

        private const int DIGITAL_MESSAGE = 0x90; // send data for a digital port
        private const int ANALOG_MESSAGE = 0xE0; // send data for an analog pin (or PWM)
        private const int REPORT_ANALOG = 0xC0; // enable analog input by pin #
        private const int REPORT_DIGITAL = 0xD0; // enable digital input by port
        private const int SET_PIN_MODE = 0xF4; // set a pin to INPUT/OUTPUT/PWM/etc
        private const int REPORT_VERSION = 0xF9; // report firmware version
        private const int SYSTEM_RESET = 0xFF; // reset from MIDI
        private const int START_SYSEX = 0xF0; // start a MIDI SysEx message
        private const int END_SYSEX = 0xF7; // end a MIDI SysEx message

        private SerialPort _serialPort;
        private int _delay;

        private int _waitForData;
        private int _executeMultiByteCommand;
        private int _multiByteChannel;
        private readonly int[] _storedInputData = new int[MAX_DATA_BYTES];
        private bool _parsingSysex;
        private int _sysexBytesRead;

        private volatile int[] _digitalOutputData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private volatile int[] _digitalInputData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private volatile int[] _analogInputData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private Thread _readThread;

        /// <summary>
        /// Setup serial port
        /// </summary>
        /// <param name="serialPortName">String specifying the name of the serial port. eg COM4. Default: Last Serial Port Name</param>
        /// <param name="baudRate">The baud rate of the communication. Default 115200</param>
        /// <param name="delay">Time delay that may be required to allow some arduino models to reboot after opening a serial connection</param>
        public void Setup(string serialPortName = "", Int32 baudRate = 57600, int delay = 2000) {
            if (String.IsNullOrEmpty(serialPortName)) {
                serialPortName = List().Last();
            }
            _serialPort = new SerialPort(serialPortName, baudRate) {
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };
            _delay = delay;
            Console.WriteLine("Arduino configured: Serial Port Name: {0} / Baud Rate: {1} / Delay: {2}", serialPortName, baudRate, delay);
        }

        /// <summary>
        /// Opens the serial port connection, should it be required. By default the port is
        /// opened when the object is first created.
        /// </summary>
        public void Open() {
            Console.WriteLine("Opening serial port");
            _serialPort.Open();
            Console.WriteLine("Serial port opened");
            Thread.Sleep(_delay);

            byte[] command = new byte[2];

            for (int i = 0; i < 6; i++) {
                command[0] = (byte) (REPORT_ANALOG | i);
                command[1] = (byte) 1;
                _serialPort.Write(command, 0, 2);
            }

            for (int i = 0; i < 2; i++) {
                command[0] = (byte) (REPORT_DIGITAL | i);
                command[1] = (byte) 1;
                _serialPort.Write(command, 0, 2);
            }

            if (_readThread != null)
                return;

            _readThread = new Thread(ProcessInput);
            _readThread.Start();
        }

        /// <summary>
        /// Closes the serial port.
        /// </summary>
        public void Close() {
            Console.WriteLine("Closing serial port");
            _readThread.Join(500);
            _readThread = null;
            _serialPort.Close();
            Console.WriteLine("Serial port closed");
        }

        /// <summary>
        /// Lists all available serial ports on current system.
        /// </summary>
        /// <returns>An array of strings containing all available serial ports.</returns>
        public static IEnumerable<string> List() {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// Returns the last known state of the digital pin.
        /// </summary>
        /// <param name="pin">The arduino digital input pin.</param>
        /// <returns>Arduino.HIGH or Arduino.LOW</returns>
        public int DigitalRead(int pin) {
            Console.WriteLine("(digital) Reading pin {0}", pin);
            return (_digitalInputData[pin >> 3] >> (pin & 0x07)) & 0x01;
        }

        /// <summary>
        /// Returns the last known state of the analog pin.
        /// </summary>
        /// <param name="pin">The arduino analog input pin.</param>
        /// <returns>A value representing the analog value between 0 (0V) and 1023 (5V).</returns>
        public int AnalogRead(int pin) {
            Console.WriteLine("(analog) Reading pin {0}", pin);
            return _analogInputData[pin];
        }

        /// <summary>
        /// Sets the mode of the specified pin (INPUT or OUTPUT).
        /// </summary>
        /// <param name="pin">The arduino pin.</param>
        /// <param name="mode">Mode Arduino.INPUT or Arduino.OUTPUT.</param>
        public void PinMode(int pin, int mode) {
            var message = new byte[3];
            message[0] = SET_PIN_MODE;
            message[1] = (byte) (pin);
            message[2] = (byte) (mode);
            _serialPort.Write(message, 0, 3);
        }

        /// <summary>
        /// Write to a digital pin that has been toggled to output mode with pinMode() method.
        /// </summary>
        /// <param name="pin">The digital pin to write to.</param>
        /// <param name="value">Value either Arduino.LOW or Arduino.HIGH.</param>
        public void DigitalWrite(int pin, int value) {
            int portNumber = (pin >> 3) & 0x0F;
            Console.WriteLine("(digital) Writing value {0} on pin {1} on port number {2}", value, pin, portNumber);
            var message = new byte[3];

            if (value == 0)
                _digitalOutputData[portNumber] &= ~(1 << (pin & 0x07));
            else
                _digitalOutputData[portNumber] |= (1 << (pin & 0x07));

            message[0] = (byte) (DIGITAL_MESSAGE | portNumber);
            message[1] = (byte) (_digitalOutputData[portNumber] & 0x7F);
            message[2] = (byte) (_digitalOutputData[portNumber] >> 7);
            _serialPort.Write(message, 0, 3);
        }

        /// <summary>
        /// Write to an analog pin using Pulse-width modulation (PWM).
        /// </summary>
        /// <param name="pin">Analog output pin.</param>
        /// <param name="value">PWM frequency from 0 (always off) to 255 (always on).</param>
        public void AnalogWrite(int pin, int value) {
            Console.WriteLine("(analog) Writing value {0} on pin {1}", value, pin);
            var message = new byte[3];
            message[0] = (byte) (ANALOG_MESSAGE | (pin & 0x0F));
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

        private int Available() {
            return _serialPort.BytesToRead;
        }

        public void ProcessInput() {
            while (_serialPort.IsOpen) {
                if (_serialPort.BytesToRead > 0) {
                    lock (this) {

                        int inputData = _serialPort.ReadByte();
                        int command;

                        if (_parsingSysex) {
                            if (inputData == END_SYSEX) {
                                _parsingSysex = false;
                                //processSysexMessage();
                            }
                            else {
                                _storedInputData[_sysexBytesRead] = inputData;
                                _sysexBytesRead++;
                            }
                        }
                        else if (_waitForData > 0 && inputData < 128) {
                            _waitForData--;
                            _storedInputData[_waitForData] = inputData;

                            if (_executeMultiByteCommand != 0 && _waitForData == 0) {
                                //we got everything
                                switch (_executeMultiByteCommand) {
                                    case DIGITAL_MESSAGE:
                                        SetDigitalInputs(_multiByteChannel, (_storedInputData[0] << 7) + _storedInputData[1]);
                                        break;
                                    case ANALOG_MESSAGE:
                                        SetAnalogInput(_multiByteChannel, (_storedInputData[0] << 7) + _storedInputData[1]);
                                        break;
                                    case REPORT_VERSION:
                                        break;
                                }
                            }
                        }
                        else {
                            if (inputData < 0xF0) {
                                command = inputData & 0xF0;
                                _multiByteChannel = inputData & 0x0F;
                            }
                            else {
                                command = inputData;
                                // commands in the 0xF* range don't use channel data
                            }
                            switch (command) {
                                case DIGITAL_MESSAGE:

                                case ANALOG_MESSAGE:
                                case REPORT_VERSION:
                                    _waitForData = 2;
                                    _executeMultiByteCommand = command;
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
