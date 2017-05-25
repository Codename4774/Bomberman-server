using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;

namespace Bomberman_client.GameClasses
{
    class SimpleScript
    {
        private int countStates;
        private int currState;
        private dynamic obj;
        private ScriptEngine.OnEndFunc onEnd;
        private Bitmap sprite;
        public Timer currTimer;
        private Bitmap temp;
        Size onScriptSize;

        public void OnTimerEvent(object sender, EventArgs e)
        {
            currState--;
            if (currState == 0)
            {
                currTimer.Stop();
                currTimer.Dispose();
                currTimer = null;
                temp.Dispose();
                sprite.Dispose();

                onEnd(obj);
            }
            else
            {
                SetImage();
            }
        }
        private void SetImage()
        {
            temp = new Bitmap(sprite.Clone(new Rectangle(new Point(((countStates - 1) - (currState - 1)) * onScriptSize.Width, 0), onScriptSize), sprite.PixelFormat));
            obj.texture = temp;
        }
        public SimpleScript(PhysicalObject obj, Image sprite, ScriptEngine.OnEndFunc onEndFunc, int countStates, int delay )
        {
            currTimer = new Timer();

            currTimer.Interval = delay;

            this.obj = obj;

            this.onEnd = onEndFunc;
            this.countStates = countStates;
            this.currState = this.countStates;
            this.sprite = new Bitmap (sprite as Bitmap);
            this.onScriptSize = obj.size;
            
            currTimer.Elapsed += OnTimerEvent;
        }
        public SimpleScript(PhysicalObject obj, Image sprite, Size onScriptSize, ScriptEngine.OnEndFunc onEndFunc, int countStates, int delay)
        {
            currTimer = new Timer();

            currTimer.Interval = delay;

            this.obj = obj;

            this.onEnd = onEndFunc;
            this.countStates = countStates;
            this.currState = this.countStates;
            this.sprite = new Bitmap(sprite as Bitmap);
            this.onScriptSize = onScriptSize;

            OnTimerEvent(currTimer, new EventArgs());
            currTimer.Elapsed += OnTimerEvent; 
        }
    }
}
