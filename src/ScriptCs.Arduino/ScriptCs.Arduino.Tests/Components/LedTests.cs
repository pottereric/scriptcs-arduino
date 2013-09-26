using Moq;
using NUnit.Framework;
using ScriptCs.Arduino.Components;
using ScriptCs.Arduino.Interfaces;
using ScriptCs.Arduino.Models;
using Should.Fluent;

namespace ScriptCs.Arduino.Tests.Components
{
    public class LedTests
    {
        private const int Pin = 13;
        private Mock<IArduino> _arduino;
        private Led _led;
        private MockedTimer _timer;

        [SetUp]
        public void Setup()
        {
            _arduino = new Mock<IArduino>(MockBehavior.Strict);
            _timer = new MockedTimer();
            _arduino.Setup(a => a.DigitalWrite(Pin, DigitalPin.Low)).Verifiable();
            _arduino.Setup(a => a.PinMode(Pin, PinMode.Output)).Verifiable();
            _led = new Led(_arduino.Object, Pin, _timer);
        }

        [Test]
        public void Off_should_digitalwrite_low()
        {
            _arduino.Setup(a => a.DigitalWrite(Pin, DigitalPin.Low)).Verifiable();
            var ledState = LedState.On;
            _led.OnStateChanged = state => { ledState = state; };

            _led.Off();

            _led.State.Should().Equal(LedState.Off);
            _arduino.VerifyAll();
            ledState.Should().Equal(LedState.Off);
        }

        [Test]
        public void On_should_digitalwrite_high()
        {
            _arduino.Setup(a => a.DigitalWrite(Pin, DigitalPin.High)).Verifiable();
            var ledState = LedState.Off;
            _led.OnStateChanged = state => { ledState = state; };

            _led.On();

            _led.State.Should().Equal(LedState.On);
            _arduino.VerifyAll();
            ledState.Should().Equal(LedState.On);
        }

        [Test]
        public void Toggle_should_toggle_LedState_and_digitalwrite()
        {
            _arduino.Setup(a => a.DigitalWrite(Pin, DigitalPin.High)).Verifiable();
            _arduino.Setup(a => a.DigitalWrite(Pin, DigitalPin.Low)).Verifiable();
            _led.On();

            _led.Toggle();
            _led.State.Should().Equal(LedState.Off);
            _led.Toggle();
            _led.State.Should().Equal(LedState.On);
            _arduino.Verify(a => a.DigitalWrite(Pin, DigitalPin.High), Times.Exactly(2));
            _arduino.Verify(a => a.DigitalWrite(Pin, DigitalPin.Low), Times.Exactly(2));
        }

        [Test]
        public void StrobeOn_should_start_timer()
        {
            _arduino.Setup(a => a.DigitalWrite(Pin, DigitalPin.High)).Verifiable();
            _arduino.Setup(a => a.DigitalWrite(Pin, DigitalPin.Low)).Verifiable();
            _led.On();

            _led.StrobeOn();
            _led.State.Should().Equal(LedState.Off);
            _timer.Tick();
            _led.State.Should().Equal(LedState.On);
            _arduino.Verify(a => a.DigitalWrite(Pin, DigitalPin.High), Times.Exactly(2));
            _arduino.Verify(a => a.DigitalWrite(Pin, DigitalPin.Low), Times.Exactly(2));
        }

        [Test]
        public void StrobeOff_should_stop_timer()
        {
            _led.StrobeOff();
            _timer.IsDisposed.Should().Equal(true);
        }
    }
}