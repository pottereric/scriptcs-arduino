using System;
using ScriptCs.Arduino.Interfaces;

namespace ScriptCs.Arduino.Tests
{
    public class MockedTimer : ITimer
    {
        public MockedTimer()
        {
            IsDisposed = false;
        }

        private Action _action;
        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public void Start(Action action, TimeSpan dueTime, TimeSpan period)
        {
            _action = action;
            Tick();
        }

        public void Tick()
        {
            _action();
        }
    }
}