using System;
using ScriptCs.Arduino.Interfaces;
using ScriptCs.Arduino.Models;

namespace ScriptCs.Arduino.Components
{
    public class Led : SinglePinComponent
    {
        private DigitalPin _value;
        public Led(IArduino board, int pin) : base(board, pin) { }
        public LedState State { get; set; }
        public Action<LedState> OnStateChanged { get; set; }

        public void StrobeOn(int milliseconds = 150)
        {
            SetInterval(() =>
            {
                if (_value == DigitalPin.Low)
                {
                    _value = DigitalPin.High;
                    Board.DigitalWrite(Pin, DigitalPin.High);
                }
                else
                {
                    _value = DigitalPin.Low;
                    Board.DigitalWrite(Pin, DigitalPin.Low);
                }
            }, milliseconds);
        }

        public void StrobeOff()
        {
            StopTimer();
        }

        public void On()
        {
            Board.DigitalWrite(Pin, DigitalPin.High);
        }

        public void Off()
        {
            Board.DigitalWrite(Pin, DigitalPin.Low);
        }

        public void Toggle()
        {
            Board.DigitalWrite(Pin, State == LedState.Off ? DigitalPin.High : DigitalPin.Low);
        }
    }

    public class LedPwm : Led
    {
        public LedPwm(IArduino board, int pin) : base(board, pin){}
        public int Intensity { get; set; }

        public void Fade(int finalValue, TimeSpan fromMilliseconds)
        {
            throw new NotImplementedException();
        }

        public void Fade(int initialValue, int finalValue, int milliseconds)
        {
            throw new NotImplementedException();
        }

        public void Fade(int initialValue, int finalValue, TimeSpan milliseconds)
        {
            throw new NotImplementedException();
        }
    }

    public enum LedState
    {
        On,
        Off
    }
}