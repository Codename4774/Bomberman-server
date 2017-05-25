using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman_client.GameClasses;
using System.Drawing;
using System.Timers;

namespace Bomberman_client.GameClasses
{
    public class Bomb : PhysicalObject
    {
        private int areaOfExplosion;
        private Timer timerExplosion;
        public Player owner;

        public void TimerEvent(object sender, EventArgs e)
        {
            deleteObjectFunc(this);            
        }
        public virtual void ChangeMapMatrix(PhysicalMap PhysicalMap)
        {
            for (int i = Y; i < Y + size.Height; i++)
            {
                for (int j = X; j < X + size.Width; j++)
                {
                    PhysicalMap.MapMatrix[i][j] = (int)PhysicalMap.KindOfArea.BOMB;
                }
            }
        }
        public Bomb(Point location, Image texture, int areaOfExplosion, DeleteObjectFunc deleteBombFunc, Player owner)
            : base(location, texture)
        {
            timerExplosion = new Timer();
            timerExplosion.Interval = 3000;
            this.deleteObjectFunc = deleteBombFunc;
            timerExplosion.Elapsed += TimerEvent;
            this.areaOfExplosion = areaOfExplosion;
            timerExplosion.Enabled = true;
            this.owner = owner;
        }
    }
}
