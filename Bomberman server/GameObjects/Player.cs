using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace Bomberman_client.GameClasses
{
    [Serializable]
    public class Player : PhysicalObject, GameInterfaces.IMovable
    {
        public enum Direction { UP, DOWN, LEFT, RIGHT };

        public Direction direction { get; set; }
        public Direction prevDirection { get; set; }
        public enum AnimState { TURNUP, TURNUP1, TURNUP2, TURNDOWN, TURNDOWN1, TURNDOWN2, TURNLEFT, TURNLEFT1, TURNLEFT2, TURNRIGHT, TURNRIGHT1, TURNRIGHT2 };
        public AnimState currAnimState;
        public enum BombLevel { low = 3, medium = 4, high = 5 };

        public BombLevel bombLevel = BombLevel.low;

        private int step = 4;
        public readonly int maxCountBombs;
        private int currCountBombs;
        public int CurrCountBombs
        {
            get
            {
                return currCountBombs;
            }
            set
            {
                currCountBombs = value;
            }
        }
        public delegate void SpawnPlayerFunc();

        private bool isDead;
        public bool IsDead 
        {
            set
            {
                isDead = value;
            }
            get
            {
                return isDead;
            }
        }
        private bool isDying;
        public bool IsDying
        {
            set
            {
                isDying = value;
            }
            get
            {
                return isDying;
            }
        }
        public BombFactory bombFactory;

        public bool isMoved;
        private Point newLocation = new Point();
        public readonly int id;



        private bool isObjectOnDirection(byte point)
        {
            return( (point == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT) || (point == (int)PhysicalMap.KindOfArea.PLAYER) || (point == (int)PhysicalMap.KindOfArea.BOMB) );
        }

        public bool isObjectOnWay(PhysicalMap map)
        {
            lock (map.MapMatrix)
            {
                bool result = false;
                switch (direction)
                {
                    case Direction.UP:
                        {
                            for (int j = X; j < X + size.Width; j++)
                            {
                                if (map.MapMatrix[Y - 1][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT )
                                {
                                    int i = Y;
                                    while (map.MapMatrix[i][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                    {
                                        i++;
                                    }
                                    newLocation.X = X;
                                    newLocation.Y = i;
                                    return true;
                                }
                            }
                        }
                        break;
                    case Direction.DOWN:
                        {
                            for (int j = X; j < X + size.Width; j++)
                            {
                                if (map.MapMatrix[Y + size.Height][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                {
                                    int i = Y + size.Height;
                                    while (map.MapMatrix[i][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                    {
                                        i--;
                                    }
                                    newLocation.X = X;
                                    newLocation.Y = i - size.Height + 1;
                                    return true;
                                }
                            }
                        }
                        break;
                    case Direction.LEFT:
                        {
                            for (int i = Y; i < Y + size.Height; i++)
                            {
                                if (map.MapMatrix[i][X - 1] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                {
                                    int j = X;
                                    while (map.MapMatrix[i][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                    {
                                        j++;
                                    }
                                    newLocation.X = j;
                                    newLocation.Y = Y;
                                    return true;
                                }
                            }
                        }
                        break;
                    case Direction.RIGHT:
                        {
                            for (int i = Y; i < Y + size.Height; i++)
                            {
                                if (map.MapMatrix[i][X + size.Width] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                {
                                    int j = X + size.Width;
                                    while (map.MapMatrix[i][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                    {
                                        j--;
                                    }
                                    newLocation.X = j - size.Width + 1;
                                    newLocation.Y = Y;
                                    return true;
                                }
                            }
                        }
                        break;
                }
                return result;
            }
        }
        public override void ChangeMapMatrix(PhysicalMap PhysicalMap)
        {
            for (int i = Y; i < Y + size.Height; i++)
            {
                for (int j = X; j < X + size.Width; j++)
                {
                    PhysicalMap.MapMatrix[i][j] = (int)PhysicalMap.KindOfArea.PLAYER;
                }
            }
        }
        public void OnMove(PhysicalMap map)
        {
            if (!isDying)
            {
                if (isMoved)
                {
                    switch (direction)
                    {
                        case Player.Direction.UP:
                            {
                                if (Y - step > 0)
                                {
                                    if (!isObjectOnWay(map))
                                    {
                                        Y -= step;
                                        if (currAnimState == AnimState.TURNUP)
                                        {
                                            currAnimState = AnimState.TURNUP1;
                                        }
                                        else
                                        {
                                            if (currAnimState == AnimState.TURNUP1)
                                            {
                                                currAnimState = AnimState.TURNUP2;
                                            }
                                            else
                                            {
                                                currAnimState = AnimState.TURNUP1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        X = newLocation.X;
                                        Y = newLocation.Y;
                                    }
                                }
                                else
                                {
                                    Y = 0;
                                }
                            }
                            break;
                        case Player.Direction.DOWN:
                            {
                                if ((map.Height > (Y + size.Height + step)))
                                {
                                    if (!isObjectOnWay(map))
                                    {
                                        Y += step;
                                        if (currAnimState == AnimState.TURNDOWN)
                                        {
                                            currAnimState = AnimState.TURNDOWN1;
                                        }
                                        else
                                        {
                                            if (currAnimState == AnimState.TURNDOWN1)
                                            {
                                                currAnimState = AnimState.TURNDOWN2;
                                            }
                                            else
                                            {
                                                currAnimState = AnimState.TURNDOWN1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        X = newLocation.X;
                                        Y = newLocation.Y;
                                    }
                                }
                                else
                                {
                                    Y = map.Height - size.Height;
                                }
                            }
                            break;
                        case Player.Direction.LEFT:
                            {
                                if (X - step > 0)
                                {
                                    if (!isObjectOnWay(map))
                                    {
                                        X -= step;
                                        if (currAnimState == AnimState.TURNLEFT)
                                        {
                                            currAnimState = AnimState.TURNLEFT1;
                                        }
                                        else
                                        {
                                            if (currAnimState == AnimState.TURNLEFT1)
                                            {
                                                currAnimState = AnimState.TURNLEFT2;
                                            }
                                            else
                                            {
                                                currAnimState = AnimState.TURNLEFT1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        X = newLocation.X;
                                        Y = newLocation.Y;
                                    }
                                }
                                else
                                {
                                    X = 0;
                                }

                            }
                            break;
                        case Player.Direction.RIGHT:
                            {
                                if ((map.Width > (X + size.Width + step)))
                                {
                                    if (!isObjectOnWay(map))
                                    {
                                        X += step;
                                        if (currAnimState == AnimState.TURNRIGHT)
                                        {
                                            currAnimState = AnimState.TURNRIGHT1;
                                        }
                                        else
                                        {
                                            if (currAnimState == AnimState.TURNRIGHT1)
                                            {
                                                currAnimState = AnimState.TURNRIGHT2;
                                            }
                                            else
                                            {
                                                currAnimState = AnimState.TURNRIGHT1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        X = newLocation.X;
                                        Y = newLocation.Y;
                                    }
                                }
                                else
                                {
                                    X = map.Width - size.Width;
                                }
                            }
                            break;
                    }

                }
                else
                {
                    switch (direction)
                    {
                        case Direction.UP:
                            {
                                currAnimState = AnimState.TURNUP;
                            }
                            break;
                        case Direction.DOWN:
                            {
                                currAnimState = AnimState.TURNDOWN;
                            }
                            break;
                        case Direction.LEFT:
                            {
                                currAnimState = AnimState.TURNLEFT;
                            }
                            break;
                        case Direction.RIGHT:
                            {
                                currAnimState = AnimState.TURNRIGHT;
                            }
                            break;
                    }

                }
            }

        }

        public bool isPlayerBlowedUp(PhysicalMap map)
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

        private string thisName;
        public string PlayerName
        {
            set
            {
                thisName = value;
            }
            get
            {
                return thisName;
            }
        }

        public Bitmap GetAnimState(Bitmap texture)
        {
            lock (texture)
            {
                Bitmap result = null;
                Bitmap currBitmap = texture as Bitmap;
                switch (currAnimState)
                {
                    case AnimState.TURNUP:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(0, size.Height), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNUP1:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(size.Width, size.Height), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNUP2:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(size.Width * 2, size.Height), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNDOWN:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(0, 0), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNDOWN1:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(size.Width, 0), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNDOWN2:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(size.Width * 2, 0), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNLEFT:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(size.Width * 3, size.Height), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNLEFT1:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(size.Width * 4, size.Height), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNLEFT2:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(size.Width * 5, size.Height), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNRIGHT:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(size.Width * 3, 0), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNRIGHT1:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(size.Width * 4, 0), size), texture.PixelFormat);
                        }
                        break;
                    case AnimState.TURNRIGHT2:
                        {
                            result = currBitmap.Clone(new Rectangle(new Point(size.Width * 5, 0), size), texture.PixelFormat);
                        }
                        break;
                }
                return result;
            }
        }


        public Player(Point location, Size spriteSize, string name, DeleteObjectFunc deletePlayerFunc, Image bombSprite, Size bombSize, DeleteObjectFunc deleteBombFunc, int id)
            : base(location, spriteSize, deletePlayerFunc)
        {
            thisName = name;
            isMoved = false;

            isDead = false;
            isDying = false;
            maxCountBombs = 2;
            currCountBombs = 0;
            this.id = id;
            this.currSpriteOffset = 0;

            bombFactory = new BombFactory(bombSize, deleteBombFunc, this);
        }
    }
}
