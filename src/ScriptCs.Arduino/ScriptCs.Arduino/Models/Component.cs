using System;
using System.Threading;
using ScriptCs.Arduino.Interfaces;

namespace ScriptCs.Arduino.Models
{
    public abstract class Component
    {
        protected readonly IArduino Board;
        private readonly ITimer _timer;

        public Component(IArduino board, ITimer timer)
        {
            Board = board;
            _timer = timer ?? new Timer();
        }

        protected void SetInterval(Action action, int milliseconds)
        {
            _timer.Start(action, 0.Milliseconds(), milliseconds.Milliseconds());
        }

        protected void SetTimeout(Action action, int milliseconds)
        {
            _timer.Start(action, milliseconds.Milliseconds(), Timeout.Infinite.Milliseconds());
        }

        protected void SetTimeoutAndInterval(Action action, int fromNow, int every)
        {
            _timer.Start(action, fromNow.Milliseconds(), every.Milliseconds());
        }

        protected void StopTimer()
        {
            _timer.Dispose();
        }
    }
}