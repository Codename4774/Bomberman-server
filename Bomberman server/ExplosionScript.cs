using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;

namespace Bomberman_client.GameClasses
{
    public class ExplosionScript
    {
        private int countStates;
        private int currState;
        private Explosion obj;
        private ScriptEngine.OnEndFunc onEnd;
        public Timer currTimer;

        public void OnTimerEvent(object sender, EventArgs e)
        {
            currState--;
            if (currState == 0)
            {
                currTimer.Enabled = false;
                currTimer.Dispose();
                currTimer = null;
                onEnd(obj);
            }
            else
            {
                obj.ChangeState();
            } 
        }

        public ExplosionScript(Explosion explosion, ScriptEngine.OnEndFunc onEndFunc, int countStates, int delay)
        {
            currTimer = new Timer();

            currTimer.Interval = delay;
            this.onEnd = onEndFunc;
            this.countStates = countStates;
            this.currState = this.countStates;
            this.obj = explosion;
            currTimer.Elapsed += OnTimerEvent;
        }
    }
}
