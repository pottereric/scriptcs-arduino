using Moq;
using NUnit.Framework;
using ScriptCs.Arduino.Components;
using ScriptCs.Arduino.Interfaces;
using ScriptCs.Arduino.Models;

namespace ScriptCs.Arduino.Tests.Components
{
    public class LedRGBTests
    {
        private static readonly int[] Pins = {9, 10, 11};
        private Mock<IArduino> _arduino;
        private LedRGB _led;
        private MockedTimer _timer;

        [SetUp]
        public void Setup()
        {
            _arduino = new Mock<IArduino>(MockBehavior.Strict);
            _timer = new MockedTimer();
            MockPinModes(PinMode.Pwm);
            _led = new LedRGB(_arduino.Object, Pins, _timer);
        }

        [Test]
        public void All_Pins_should_be_PWM()
        {
            _arduino.VerifyAll();
        }

        [Test]
        public void On_should_analogwrite_255()
        {
            MockAnalogWrite(255);
            _led.On();
            MockVerifyAnalogWrite(255);
        }

        [Test]
        public void Off_should_analogwrite_0() {
            MockAnalogWrite(0);
            _led.Off();
            MockVerifyAnalogWrite(0);
        }

        [Test]
        public void Color_should_analogwrite_values()
        {
            int value = 255;
            MockAnalogWrite(value);
            _led.Color(value, value, value);
            MockVerifyAnalogWrite(value);
        }

        [Test]
        public void Color_should_not_allow_values_greater_than_255()
        {
            int value = 256;
            MockAnalogWrite(255);
            _led.Color(value, value, value);
            MockVerifyAnalogWrite(255);
        }

        [Test]
        public void Color_should_not_allow_values_lower_than_0()
        {
            int value = -1;
            MockAnalogWrite(0);
            _led.Color(value, value, value);
            MockVerifyAnalogWrite(0);
        }

        private void MockPinModes(PinMode mode)
        {
            foreach (int pin in Pins)
                _arduino.Setup(a => a.PinMode(pin, mode)).Verifiable();
        }

        

        private void MockAnalogWrite(int value)
        {
            foreach (int pin in Pins)
                _arduino.Setup(a => a.AnalogWrite(pin, value)).Verifiable();
        }

       

        private void MockVerifyAnalogWrite(int value)
        {
            foreach (int pin in Pins)
                _arduino.Verify(a => a.AnalogWrite(pin, value));
        }
    }
}