using System;

namespace ScriptCs.Arduino.Interfaces
{
    public interface ITimer : IDisposable
    {
        void Start(Action action, TimeSpan dueTime, TimeSpan period);
    }
}