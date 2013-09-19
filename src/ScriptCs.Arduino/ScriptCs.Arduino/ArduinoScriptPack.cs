using ScriptCs.Contracts;

namespace ScriptCs.Arduino
{
    public class ArduinoScriptPack : IScriptPack
    {
        public void Initialize(IScriptPackSession session)
        {
            session.ImportNamespace("System.IO.Ports");
            session.ImportNamespace("System.Threading");
            session.ImportNamespace("ScriptCs.Arduino");
            session.ImportNamespace("ScriptCs.Arduino.Components");
            session.ImportNamespace("ScriptCs.Arduino.Models");
            session.ImportNamespace("ScriptCs.Arduino.Interfaces");
        }

        IScriptPackContext IScriptPack.GetContext()
        {
            return new ArduinoContext();
        }

        public void Terminate()
        {
        }
    }
}