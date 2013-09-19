namespace ScriptCs.Arduino.Interfaces
{
    public interface IBoard
    {
        int DigitalRead(int pin);
        int AnalogRead(int pin);
        void PinMode(int pin, PinMode mode);
        void DigitalWrite(int pin, DigitalPin value);
        void AnalogWrite(int pin, int value);
    }
}