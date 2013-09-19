using System;
using System.Threading;
using ScriptCs.Arduino.Interfaces;

namespace ScriptCs.Arduino.Models
{
    public abstract class Component
    {
        protected readonly IArduino Board;
        private Timer _timer;

        public Component(IArduino board)
        {
            Board = board;
        }

        protected void SetInterval(Action action, int milliseconds)
        {
            _timer = new Timer(state => action(), null, 0, milliseconds);
        }
        protected void SetTimeout(Action action, int milliseconds)
        {
            _timer = new Timer(state => action(), null, milliseconds, Timeout.Infinite);
        }
        protected void SetTimeoutAndInterval(Action action, int fromNow, int every)
        {
            _timer = new Timer(state => action(), null, fromNow, every);
        }

        protected void StopTimer()
        {
            _timer.Dispose();
        }
    }
}