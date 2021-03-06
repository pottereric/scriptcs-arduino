﻿using System;
using ScriptCs.Arduino.Interfaces;
using ScriptCs.Arduino.Models;

namespace ScriptCs.Arduino.Components
{
    public class LedPwm : LedBase
    {
        private const int MinRange = 0;
        private const int MaxRange = 255;

        private int _intensity;

        public LedPwm(IArduino board, int pin, ITimer timer = null)
            : base(board, pin, timer)
        {
            SetPinMode(PinMode.Pwm);
        }

        public int Intensity
        {
            get { return _intensity; }
            set
            {
                if (_intensity == value) return;
                _intensity = value;
                OnIntensityChanged();
            }
        }

        private void OnIntensityChanged()
        {
            if (_intensity < MinRange || _intensity > MaxRange)
            {
                throw new ArgumentOutOfRangeException("intensity",
                    "Minimum range: {0} / Maximum range: {1}".FormatWith(MinRange, MaxRange));
            }
            Board.AnalogWrite(Pin, _intensity);
        }

        public void Fade(int finalValue, TimeSpan? period = null)
        {
            Fade(_intensity, finalValue, period);
        }

        public void Fade(int initialValue, int finalValue, TimeSpan? period = null)
        {
            if (period == null)
            {
                period = 1.Seconds();
            }
            var direction = finalValue > initialValue ? 1 : -1;
            SetInterval(() =>
            {
                if (Intensity == finalValue)
                {
                    StopTimer();
                    return;
                }
                Intensity += direction;
            }, (int)(period.Value.TotalMilliseconds) / ((finalValue - initialValue) * 2));
        }
    }
}