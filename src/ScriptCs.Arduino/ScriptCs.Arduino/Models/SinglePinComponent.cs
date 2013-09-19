using ScriptCs.Arduino.Interfaces;

namespace ScriptCs.Arduino.Models
{
    public abstract class SinglePinComponent : Component
    {
        protected readonly int Pin;

        public SinglePinComponent(IArduino board, int pin)
            : base(board)
        {
            Pin = pin;
        }
    }
}