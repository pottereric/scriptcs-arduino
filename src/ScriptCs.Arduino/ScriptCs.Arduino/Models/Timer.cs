using System;
using ScriptCs.Arduino.Interfaces;

namespace ScriptCs.Arduino.Models {
    public class Timer : ITimer {
        private System.Threading.Timer _timer;

        public void Start(Action action, int dueTime, int period) {
            _timer = new System.Threading.Timer(s => action(), null, dueTime, period);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}