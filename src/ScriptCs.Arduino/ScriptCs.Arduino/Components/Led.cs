using ScriptCs.Arduino.Interfaces;
using ScriptCs.Arduino.Models;

namespace ScriptCs.Arduino.Components
{
    public class Led : LedBase
    {
        public Led(IArduino board, int pin, ITimer timer = null)
            : base(board, pin, timer)
        {
            SetPinMode(PinMode.Output);
        }
    }
}