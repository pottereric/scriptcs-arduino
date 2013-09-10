using ScriptCs.Contracts;

namespace ScriptCs.Arduino {
    public class ArduinoScriptPack : IScriptPack {
        public void Initialize(IScriptPackSession session) {
            session.ImportNamespace("System.IO.Ports");
            session.ImportNamespace("System.Threading");
        }

        IScriptPackContext IScriptPack.GetContext() {
            return new Arduino();
        }

        public void Terminate() {
        }
    }
}