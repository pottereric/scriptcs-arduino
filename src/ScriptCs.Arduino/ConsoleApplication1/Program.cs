using System;
using System.Threading;
using ScriptCs.Arduino;
using ScriptCs.Arduino.Components;
using ScriptCs.Arduino.Models;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new Arduino();
            var led = new Led(a, 13);
            led.StrobeOn(500);
            led.StrobeOff();
            led.On();
            led.Off();
            led.Toggle();
            Console.WriteLine(led.State);
            led.OnStateChanged = (state) =>
            {
                Console.WriteLine(state);
            };

            var ledPwm = new LedPwm(a, 13);
            ledPwm.StrobeOn(500);
            ledPwm.StrobeOff();
            ledPwm.On();
            ledPwm.Off();
            Console.WriteLine(ledPwm.State);
            ledPwm.OnStateChanged = (state) =>
            {
                Console.WriteLine(state);
            };
            ledPwm.Intensity = 0;
            ledPwm.Fade(255, TimeSpan.FromMilliseconds(300));
            ledPwm.Fade(0, 255, 300);
            ledPwm.Fade(0, 255, 300.Milliseconds());
            ledPwm.Fade(0, 255, 300);
            
        }
    }
}
