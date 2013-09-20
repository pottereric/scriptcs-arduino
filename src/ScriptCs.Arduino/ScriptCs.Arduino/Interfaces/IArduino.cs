using ScriptCs.Arduino.Models;
using System;

namespace ScriptCs.Arduino.Interfaces
{
    public interface IArduino : IDisposable
    {
        bool Debug { get; set; }

        /// <summary>
        ///     Opens the serial port connection, should it be required. By default the port is
        ///     opened when the object is first created.
        /// </summary>
        void Open();

        /// <summary>
        ///     Closes the serial port.
        /// </summary>
        void Close();

        /// <summary>
        ///     Returns the last known state of the digital pin.
        /// </summary>
        /// <param name="pin">The arduino digital input pin.</param>
        /// <returns>Arduino.HIGH or Arduino.LOW</returns>
        int DigitalRead(int pin);

        /// <summary>
        ///     Returns the last known state of the analog pin.
        /// </summary>
        /// <param name="pin">The arduino analog input pin.</param>
        /// <returns>A value representing the analog value between 0 (0V) and 1023 (5V).</returns>
        int AnalogRead(int pin);

        /// <summary>
        ///     Sets the mode of the specified pin (INPUT or OUTPUT).
        /// </summary>
        /// <param name="pin">The arduino pin.</param>
        /// <param name="mode">Mode Arduino.INPUT or Arduino.OUTPUT.</param>
        void PinMode(int pin, PinMode mode);

        /// <summary>
        ///     Write to a digital pin that has been toggled to output mode with pinMode() method.
        /// </summary>
        /// <param name="pin">The digital pin to write to.</param>
        /// <param name="value">Value either Arduino.LOW or Arduino.HIGH.</param>
        void DigitalWrite(int pin, DigitalPin value);

        /// <summary>
        ///     Write to an analog pin using Pulse-width modulation (PWM).
        /// </summary>
        /// <param name="pin">Analog output pin.</param>
        /// <param name="value">PWM frequency from 0 (always off) to 255 (always on).</param>
        void AnalogWrite(int pin, int value);
    }
}