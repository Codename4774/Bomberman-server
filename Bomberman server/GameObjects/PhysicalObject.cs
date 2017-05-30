using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Bomberman_client.GameClasses
{
    [Serializable]
    public class PhysicalObject : Cell
    {
        public readonly Size size;
        public void ClearPrevPlace(PhysicalMap PhysicalMap)
        {
            for (int i = Y; i < Y + size.Height; i++)
            {
                for (int j = X; j < X + size.Width; j++)
                {
                    PhysicalMap.MapMatrix[i][j] = 0;
                }
            }
        }
        public int currSpriteOffset;
        public virtual void ChangeMapMatrix(PhysicalMap PhysicalMap)
        {
            for (int i = Y; i < Y + size.Height; i++)
            {
                for (int j = X; j < X + size.Width; j++)
                {
                    PhysicalMap.MapMatrix[i][j] = (int)PhysicalMap.KindOfArea.PHYSICACOBJECT;
                }
            }
        }
        public delegate void DeleteObjectFunc(PhysicalObject obj);
        protected DeleteObjectFunc deleteObjectFunc;

        public PhysicalObject(Point location)
            : base(location)
        {
           // size = texture.Size;
            currSpriteOffset = 0;
        }
        public PhysicalObject(Point location, Size size)
            : base(location)
        {
            this.size = size;
            currSpriteOffset = 0;
        }
        public PhysicalObject(Point location, Size spriteSize, DeleteObjectFunc deleteObjectFunc )
            : base(location)
        {
            this.size = spriteSize;
            this.deleteObjectFunc = deleteObjectFunc;
            currSpriteOffset = 0;
        }
    }
}
