using System.Threading;
using ScriptCs.Arduino;
using ScriptCs.Arduino.Components;

namespace ScriptCs.ConsoleApp
{
    public static class Program
    {
        static void Main()
        {
            //Led();
            LedPwm();
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
