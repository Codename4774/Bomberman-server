using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Bomberman_client.GameClasses
{
    public class PhysicalMap
    {
        public enum KindOfArea { NONE = 0, PHYSICACOBJECT = 1, EXPLOSION = 2, PLAYER = 3, BOMB = 4 };

        private byte[][] mapMatrix1;
        private byte[][] mapMatrix2;
        public enum NumbOfMapMatrix { FIRST, SECOND };
        public NumbOfMapMatrix currMatrix;
        public byte[][] MapMatrix
        {
            get
            {
                switch(currMatrix)
                {
                    case NumbOfMapMatrix.FIRST:
                        {
                            return mapMatrix1;
                        }
                        break;
                    case NumbOfMapMatrix.SECOND:
                        {
                            return mapMatrix2;
                        }
                        break;
                    default:
                        {
                            return mapMatrix1;
                        }
                        break;

                }
            }
            private set
            {
            }
        }
        private int width;
        public int Width
        {
            get
            {
                return width;
            }
            private set
            {
            }
        }
        private int height;
        public int Height
        {
            get
            {
                return height;
            }
            private set
            {
            }

        }
        public PhysicalMap(byte[][] mapMatrix)
        {
            this.mapMatrix1 = mapMatrix;
        }

        public void ClearCurrMatrix()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    MapMatrix[i][j] = (int)KindOfArea.NONE;
                }
            }
        }

        public void SwitchMatrix()
        {
            if (currMatrix == NumbOfMapMatrix.FIRST)
            {
                currMatrix = NumbOfMapMatrix.SECOND;
            }
            else
            {
                currMatrix = NumbOfMapMatrix.FIRST;
            }
        }

        public PhysicalMap(int width, int height)
        {
            mapMatrix1 = new byte[height][];
            mapMatrix2 = new byte[height][];
            for (int i = 0; i < height; i++)
            {
                mapMatrix1[i] = new byte[width];
            }
            for (int i = 0; i < height; i++)
            {
                mapMatrix2[i] = new byte[width];
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mapMatrix1[i][j] = (int)KindOfArea.NONE;
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mapMatrix1[i][j] = (int)KindOfArea.NONE;
                }
            }
            currMatrix = NumbOfMapMatrix.FIRST;
            this.width = width;
            this.height = height;
        }
    }
}
