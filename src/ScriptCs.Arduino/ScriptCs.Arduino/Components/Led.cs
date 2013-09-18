using System.Threading;

namespace ScriptCs.Arduino.Components
{
    public class Led
    {
        private readonly Arduino _board;
        private readonly int _pin;
        private bool _shouldStop;
        private Timer _timer;
        private DigitalPin _value;

        public Led(Arduino board, int pin)
        {
            _board = board;
            _pin = pin;
            _value = DigitalPin.Low;
            _board.PinMode(pin, PinMode.Output);
        }

        public void Strobe(int milliseconds = 150)
        {
            _timer = new Timer(state =>
            {
                if (_value == DigitalPin.Low)
                {
                    _value = DigitalPin.High;
                    _board.DigitalWrite(_pin, DigitalPin.High);
                }
                else
                {
                    _value = DigitalPin.Low;
                    _board.DigitalWrite(_pin, DigitalPin.Low);
                }
            }, null, 0, milliseconds);
        }

        public void Stop()
        {
            _timer.Dispose();
        }
    }
}