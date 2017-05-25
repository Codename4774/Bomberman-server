using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Bomberman_client.GameClasses
{
    public class PartExplosion
    {
        private int countStates;
        private int currState;
        private int currSpriteOffset;
        public Bitmap texture;
        public Bitmap currTexture;
        public Size size;
        public Point location;
        private Explosion.OnEndAllExplosionFunc onEndFunc;
        public void ChangeState()
        {
            currTexture = new Bitmap(texture.Clone(new Rectangle(new Point(currSpriteOffset, 0), size), texture.PixelFormat));
            currSpriteOffset += size.Width;
            currState++;
        }
        public void ChangePhysicalMap(PhysicalMap map)
        {
            int firstBorder;
            if (location.Y + size.Height > map.Height)
            {
                firstBorder = map.Height;
            }
            else
            {
                firstBorder = location.Y + size.Height;
            }
            int secondBorder;
            if (location.X + size.Width > map.Width)
            {
                secondBorder = map.Width;
            }
            else
            {
                secondBorder = location.X + size.Width;
            }
            for (int i = location.Y; i < firstBorder; i++)
            {
                for (int j = location.X; j < secondBorder; j++)
                {
                    map.MapMatrix[i][j] = (int)PhysicalMap.KindOfArea.EXPLOSION;
                }
            }
        }
        public PartExplosion(Image texture, Size size, Point location, int countStates, Explosion.OnEndAllExplosionFunc onEndFunc)
        {
            this.texture = new Bitmap((Bitmap)(texture as Bitmap).Clone());
            this.size = size;
            this.countStates = countStates;
            this.onEndFunc = onEndFunc;
            this.location = location;
            currState = 0;
            currSpriteOffset = 0;
        }
    }
}
