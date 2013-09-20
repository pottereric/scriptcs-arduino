using System;
using ScriptCs.Arduino.Interfaces;

namespace ScriptCs.Arduino.Components
{
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
}