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
    [Serializable]
    public class Bomb : PhysicalObject
    {
        private int areaOfExplosion;
        public Player owner;
        public bool isBlowedUp;

        public void TimerEvent(object sender, EventArgs e)
        {
            (sender as Timer).Enabled = false;
            (sender as Timer).Dispose();
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
        public Bomb(Point location, int areaOfExplosion, DeleteObjectFunc deleteBombFunc, Player owner)
            : base(location)
        {

            Timer timerExplosion = new Timer();
            timerExplosion.Interval = 3000;
            this.deleteObjectFunc = deleteBombFunc;
            timerExplosion.Elapsed += TimerEvent;
            this.areaOfExplosion = areaOfExplosion;
            timerExplosion.Enabled = true;
            this.isBlowedUp = false;
            this.owner = owner;
        }
    }
}
