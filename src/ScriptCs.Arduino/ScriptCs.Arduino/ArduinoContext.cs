using ScriptCs.Contracts;

namespace ScriptCs.Arduino
{
    public class ArduinoContext : IScriptPackContext
    {
        public Models.Arduino CreateBoard(string serialPortName = "", int baudRate = 57600, int delay = 2000)
        {
            return new Models.Arduino(serialPortName, baudRate, delay);
        }
    }
}