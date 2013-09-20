using System;
using Moq;
using NUnit.Framework;
using ScriptCs.Arduino.Components;
using ScriptCs.Arduino.Interfaces;
using Should.Fluent;

namespace ScriptCs.Arduino.Tests.Components
{
    public class LedPwmTests
    {
        private LedPwm _led;
        private Mock<IArduino> _arduino;
        private MockedTimer _timer;
        private const int Pin = 13;

        [SetUp]
        public void Setup()
        {
            _arduino = new Mock<IArduino>(MockBehavior.Strict);
            _timer = new MockedTimer();
            _led = new LedPwm(_arduino.Object, Pin, _timer);
            _led.Intensity.Should().Equal(0);
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void Cant_set_intensity_over_255()
        {
            _led.Intensity = 256;
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void Cant_set_intensity_below_0()
        {
            _led.Intensity = -1;
        }

        [Test]
        public void Fade_with_high_value_should_fadeIn()
        {
            var times = 0;
            const int toIntensity = 255;
            _arduino.Setup(a => a.AnalogWrite(Pin, It.IsAny<int>())).Callback(() => { times++; }).Verifiable();
            _led.Fade(toIntensity);
            for (int i = 0; i < toIntensity - 1; i++)
            {
                _timer.Tick();
            }
            _led.Intensity.Should().Equal(toIntensity);
            times.Should().Equal(255);
        }

        [Test]
        public void Fade_with_low_value_should_fadeIn()
        {
            var times = 0;
            const int toIntensity = 0;
            _arduino.Setup(a => a.AnalogWrite(Pin, It.IsAny<int>())).Callback(() => { times++; }).Verifiable();
            _led.Intensity = 255;

            _led.Fade(toIntensity);
            for (int i = 255; i > toIntensity - +1; i--)
            {
                _timer.Tick();
            }

            _led.Intensity.Should().Equal(toIntensity);
            times.Should().Equal(255 + 1); //we have to +1 here because we set the intensity
        }
    }
}