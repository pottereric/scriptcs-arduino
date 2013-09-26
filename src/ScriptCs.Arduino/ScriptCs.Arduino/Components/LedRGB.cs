using System;
using ScriptCs.Arduino.Interfaces;
using ScriptCs.Arduino.Models;

namespace ScriptCs.Arduino.Components
{
    public class LedRGB : MultiPinComponentcs
    {
        private const int Min = 0;
        private const int Max = 255;
        private readonly int _bluePin;
        private readonly int _greenPin;
        private readonly int _redPin;

        public LedRGB(IArduino board, int[] pins, ITimer timer = null)
            : base(board, pins, timer)
        {
            if (pins.Length != 3)
            {
                throw new ArgumentException("Expecting 3 pins", "pins");
            }
            _redPin = pins[0];
            _greenPin = pins[1];
            _bluePin = pins[2];
            SetPinMode(PinMode.Pwm);
        }

        public LedRGB(IArduino board, int redPin, int greenPin, int bluePin, ITimer timer = null)
            : this(board, new[] {redPin, greenPin, bluePin}, timer)
        {
        }

        public void On()
        {
            foreach (int pin in Pins)
            {
                Board.AnalogWrite(pin, Max);
            }
        }

        public void Off()
        {
            foreach (int pin in Pins)
            {
                Board.AnalogWrite(pin, Min);
            }
        }

        public void Color(int red, int green, int blue)
        {
            Func<int, int> check = i =>
            {
                if (i > Max)
                {
                    i = Max;
                }
                else if (i < Min)
                {
                    i = Min;
                }
                return i;
            };
            Board.AnalogWrite(_redPin, check(red));
            Board.AnalogWrite(_greenPin, check(green));
            Board.AnalogWrite(_bluePin, check(blue));
        }
    }
}