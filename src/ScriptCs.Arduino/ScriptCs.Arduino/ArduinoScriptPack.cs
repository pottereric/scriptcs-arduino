using System;
using ScriptCs.Contracts;

namespace ScriptCs.Arduino
{
    public class ArduinoScriptPack : IScriptPack
    {
        private readonly Arduino _board = new Arduino();

        public ArduinoScriptPack()
        {
            _board.Setup();
        }

        public void Initialize(IScriptPackSession session)
        {
            session.ImportNamespace("System.IO.Ports");
            session.ImportNamespace("System.Threading");
            session.ImportNamespace("ScriptCs.Arduino");
            session.ImportNamespace("ScriptCs.Arduino.Components");
        }

        IScriptPackContext IScriptPack.GetContext()
        {
            return _board;
        }

        public void Terminate()
        {
            _board.Dispose();
        }
    }
}