using System;

namespace ScriptCs.Arduino.Interfaces {
    public interface ITimer : IDisposable {
        void Start(Action action, int dueTime, int period);
    }
}