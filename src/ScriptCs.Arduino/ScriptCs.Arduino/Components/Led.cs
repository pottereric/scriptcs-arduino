using System.Threading;

namespace ScriptCs.Arduino.Components
{
    public class Led
    {
        private readonly Arduino _board;
        private readonly int _pin;
        private bool _shouldStop;

        public Led(Arduino board, int pin)
        {
            _board = board;
            _pin = pin;
            _board.PinMode(pin, PinMode.Output);
        }

        public void Strobe(int milliseconds = 150)
        {
            while (!_shouldStop)
            {
                _board.DigitalWrite(_pin, DigitalPin.High);
                Thread.Sleep(milliseconds);
                _board.DigitalWrite(_pin, DigitalPin.Low);
                Thread.Sleep(milliseconds);
            }
        }
        public void Stop()
        {
            _shouldStop = true;
        }
    }
}