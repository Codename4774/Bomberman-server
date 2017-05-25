using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;

namespace Bomberman_client.GameClasses
{
    public class ScriptEngine
    {
        public delegate void OnEndFunc(object sender);
        public delegate void OnChangeFunc();
        public delegate void TimerEvent(object sender, EventArgs e);
        public void StartSimpleScript(PhysicalObject obj, Image sprite, OnEndFunc onEndFunc, int delay, int countStates)
        {
            SimpleScript states = new SimpleScript(obj, sprite, onEndFunc, countStates, delay);
            states.currTimer.Enabled = true;
        }
        public void StartSimpleScript(PhysicalObject obj, Image sprite, Size onScriptSize, OnEndFunc onEndFunc, int delay, int countStates)
        {
            SimpleScript states = new SimpleScript(obj, sprite, onScriptSize, onEndFunc, countStates, delay);
            states.currTimer.Enabled = true;            
        }
        public void StartExplosion(Explosion explosion, OnEndFunc onEndFunc, int delay, int countStates)
        {
            ExplosionScript script = new ExplosionScript(explosion, onEndFunc, countStates, delay);
            script.currTimer.Start();
        }
    }
}
