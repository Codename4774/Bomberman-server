using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Bomberman_client.GameClasses
{
    [Serializable]
    public class BombFactory
    {
        //private Image bombSprite;
        private Size bombSize;
        public PhysicalObject.DeleteObjectFunc deleteBombFunc;
        private Player owner;
        public Bomb GetBomb(Player.BombLevel bombLevel, Point location)
        {
            return new Bomb(location, (int)bombLevel, deleteBombFunc, owner );
        }
        public BombFactory(Size bombSize, PhysicalObject.DeleteObjectFunc deleteFunc, Player owner)
        {
            //this.bombSprite = bombSprite;
            this.bombSize = bombSize;
            deleteBombFunc = deleteFunc;
            this.owner = owner;
        }
    }
}
