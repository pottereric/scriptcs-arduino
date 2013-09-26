using System;
using System.Threading;
using ScriptCs.Arduino;
using ScriptCs.Arduino.Components;

namespace ScriptCs.ConsoleApp
{
    public static class Program
    {
        private static readonly Random Random = new Random();

        private static void Main()
        {
            //Led();
            //LedPwm();
            LedRgb();
        }

        private static void LedRgb()
        {
            using (var board = new Arduino.Models.Arduino {Debug = true})
            {
                var led = new LedRGB(board, 9, 10, 11);
                Action wait = () => Thread.Sleep(2.Seconds());
                Action hold = () => Thread.Sleep(50.Milliseconds());
                led.On();
                wait();
                led.Off();
                wait();
                led.Color(255, 0, 0);
                wait();
                led.Color(0, 255, 0);
                wait();
                led.Color(0, 0, 255);
                wait();
                led.Off();
                wait();
                for (int i = 0; i < 100; i++)
                {
                    led.Color(Random.Next(0, 255), Random.Next(0, 255), Random.Next(0, 255));
                    hold();
                }
                led.Off();
            }
        }

        private static void LedPwm()
        {
            using (var board = new Arduino.Models.Arduino {Debug = true})
            {
                var led = new LedPwm(board, 9);
                led.Fade(255, 1.Second());
                Thread.Sleep(2.Seconds());
                led.Fade(0, 1.Second());
                led.Off();
            }
        }

        private static void Led()
        {
            using (var board = new Arduino.Models.Arduino {Debug = true})
            {
                var led = new Led(board, 13);
                led.StrobeOn(20);
                Thread.Sleep(3.Seconds());
                led.Off();
            }
        }
    }
}