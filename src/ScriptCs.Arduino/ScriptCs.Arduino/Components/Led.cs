using System;
using ScriptCs.Arduino.Interfaces;
using ScriptCs.Arduino.Models;

namespace ScriptCs.Arduino.Components
{
    public class Led : SinglePinComponent
    {
        public Led(IArduino board, int pin, ITimer timer = null)
            : base(board, pin, timer)
        {
            State = LedState.Off;
            OnStateChanged = state => { };
        }

        public LedState State { get; private set; }
        public Action<LedState> OnStateChanged { get; set; }

        public void StrobeOn(int milliseconds = 150)
        {
            SetInterval(Toggle, milliseconds);
        }

        public void StrobeOff()
        {
            StopTimer();
        }

        public void On()
        {
            Board.DigitalWrite(Pin, DigitalPin.High);
            State = LedState.On;
            OnStateChanged(State);
        }

        public void Off()
        {
            Board.DigitalWrite(Pin, DigitalPin.Low);
            State = LedState.Off;
            OnStateChanged(State);
        }

        public void Toggle()
        {
            if (State == LedState.On)
            {
                Off();
            }
            else
            {
                On();
            }
        }
    }
}