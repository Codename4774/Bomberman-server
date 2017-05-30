using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Bomberman_client.GameClasses
{
    [Serializable]
    public class DynamicWall : PhysicalObject
    {
        public bool isBlowedUpNow = false;
        public bool isWallBlowedUp(PhysicalMap map)
        {
            for (int i = Y; i < Y + size.Height; i++)
            {
                for (int j = X; j < X + size.Width; j++)
                {
                    if (map.MapMatrix[i][j] == (int)PhysicalMap.KindOfArea.EXPLOSION)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public DynamicWall(Point location, Size size)
            : base(location, size)
        { 
        }
    }
}
