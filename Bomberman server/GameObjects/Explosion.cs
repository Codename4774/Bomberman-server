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
    public class Explosion
    {
        private int radius;
        public List<PartExplosion> partsExplosionLeft;
        public List<PartExplosion> partsExplosionRight;
        public List<PartExplosion> partsExplosionBottom;
        public List<PartExplosion> partsExplosionUp;
        public PartExplosion partExplosionCenter;
        private Point explosionLocation;
        private Point newLocation;
        private int countParts;
        public delegate void OnEndAllExplosionFunc(object sender);
        private OnEndAllExplosionFunc onEndAllExplosionFunc;
        Size size;
        public enum KindExplosionTexture { explosionTextureHorizontalMiddle, explosionTextureLeftEdge, explosionTextureRightEdge, explosionTextureVerticalMiddle, explosionTextureUpEdge, explosionTextureBottomEdge, explosionTextureCenter };
        private enum ExplosionDirection { UP, DOWN, LEFT, RIGHT };
        public void ChangeState()
        {
            for (int i = 0; i < partsExplosionBottom.Count; i++)
            {
                partsExplosionBottom[i].ChangeState();
            }
            for (int i = 0; i < partsExplosionUp.Count; i++)
            {
                partsExplosionUp[i].ChangeState();
            }
            for (int i = 0; i < partsExplosionLeft.Count; i++)
            {
                partsExplosionLeft[i].ChangeState();
            }
            for (int i = 0; i < partsExplosionRight.Count; i++)
            {
                partsExplosionRight[i].ChangeState();
            }
            partExplosionCenter.ChangeState();
        }
        public void onEndExplosionFunc(object sender)
        {
            countParts--;
            if (countParts == 0)
            {
                onEndAllExplosionFunc(this);
            }
        }

        public byte[] SerializeExplosion(BinaryFormatter serializer)
        {
            MemoryStream data = new MemoryStream();
            serializer.Serialize(data, this);

            return data.ToArray();
        }
        private bool canWePlaceExplosion(int X, int Y, PhysicalMap map, ExplosionDirection direction)
        {
            bool result = false;
            lock (map.MapMatrix)
            {
                switch (direction)
                {
                    case ExplosionDirection.UP:
                        {
                            for (int j = X; j < X + size.Width; j++)
                            {
                                for (int k = Y; (k < Y + size.Height) && (k < map.Height); k++)
                                {
                                    if (map.MapMatrix[k][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                    {
                                        int i = Y;
                                        while (map.MapMatrix[i][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                        {
                                            i++;
                                        }
                                        newLocation.X = X;
                                        newLocation.Y = i - 1;
                                        return true;
                                    }
                                }
                            }
                        }
                        break;
                    case ExplosionDirection.DOWN:
                        {
                            for (int j = X; j < X + size.Width; j++)
                            {
                                for (int k = Y + size.Height; (k > Y) && (k > 0); k--)
                                {
                                    if (map.MapMatrix[k][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                    {
                                        int i = Y + size.Height;
                                        while (map.MapMatrix[i][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                        {
                                            i--;
                                        }
                                        newLocation.X = X;
                                        newLocation.Y = i - size.Height + 2;
                                        return true;
                                    }
                                }
                            }
                        }
                        break;
                    case ExplosionDirection.LEFT:
                        {
                            for (int i = Y; i < Y + size.Height; i++)
                            {
                                for (int k = X; (k < X + size.Width) && (k < map.Width); k++)
                                {
                                    if (map.MapMatrix[i][k] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
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
                        }
                        break;
                    case ExplosionDirection.RIGHT:
                        {
                            for (int i = Y; i < Y + size.Height; i++)
                            {
                                for (int k = X + size.Width; (k > X) && (k > 0); k--)
                                {
                                    if (map.MapMatrix[i][k] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                    {
                                        int j = X + size.Width;
                                        while (map.MapMatrix[i][j] == (int)PhysicalMap.KindOfArea.PHYSICACOBJECT)
                                        {
                                            j--;
                                        }
                                        newLocation.X = j - size.Width + 2;
                                        newLocation.Y = Y;
                                        return true;
                                    }
                                }
                            }
                        }
                        break;
                }
                return result;
            }
        }
        private void InitExplosion
            (   
                Size size, Point location, int radius,
                PhysicalMap map
            )
        {
            const int countStates = 7;
            partsExplosionBottom = new List<PartExplosion>();
            partsExplosionLeft = new List<PartExplosion>();
            partsExplosionRight = new List<PartExplosion>();
            partsExplosionUp = new List<PartExplosion>();
            countParts = 0;
            for (int i = 0; i < radius; i++)
            {
                if ( (location.X - size.Width * (i + 1)) > 0)
                {
                    if (!canWePlaceExplosion(location.X - size.Width * (i), location.Y, map, ExplosionDirection.LEFT))
                    {
                        if (i < radius - 1)
                        {
                            partsExplosionLeft.Add(new PartExplosion(KindExplosionTexture.explosionTextureHorizontalMiddle, size, new Point((location.X - size.Width * (i + 1)), location.Y), countStates, onEndExplosionFunc));
                        }
                        else
                        {
                            partsExplosionLeft.Add(new PartExplosion(KindExplosionTexture.explosionTextureLeftEdge, size, new Point((location.X - size.Width * (i + 1)), location.Y), countStates, onEndExplosionFunc));
                        }
                        countParts++;
                    }
                    else
                    {
                        partsExplosionLeft.Add(new PartExplosion(KindExplosionTexture.explosionTextureHorizontalMiddle, size, new Point(newLocation.X, newLocation.Y), countStates, onEndExplosionFunc));
                        break;
                    }
                }
                else
                {
                    if (i < radius - 1)
                    {
                        partsExplosionLeft.Add(new PartExplosion(KindExplosionTexture.explosionTextureHorizontalMiddle, size, new Point(0, location.Y), countStates, onEndExplosionFunc));
                    }
                    else
                    {
                        partsExplosionLeft.Add(new PartExplosion(KindExplosionTexture.explosionTextureLeftEdge, size, new Point(0, location.Y), countStates, onEndExplosionFunc));
                    }
                    countParts++;
                    break;
                }
            }
            for (int i = 0; i < radius; i++)
            {
                if ((location.X + size.Width * (i + 1) ) < map.Width)
                {
                    if (!canWePlaceExplosion(location.X + size.Width * (i), location.Y, map, ExplosionDirection.RIGHT))
                    {
                        if (i < radius - 1)
                        {
                            partsExplosionRight.Add(new PartExplosion(KindExplosionTexture.explosionTextureHorizontalMiddle, size, new Point((location.X + size.Width * (i + 1)), location.Y), countStates, onEndExplosionFunc));
                        }
                        else
                        {
                            partsExplosionRight.Add(new PartExplosion(KindExplosionTexture.explosionTextureRightEdge, size, new Point((location.X + size.Width * (i + 1)), location.Y), countStates, onEndExplosionFunc));
                        }
                        countParts++;
                    }
                    else
                    {
                        partsExplosionLeft.Add(new PartExplosion(KindExplosionTexture.explosionTextureHorizontalMiddle, size, new Point(newLocation.X, newLocation.Y), countStates, onEndExplosionFunc));
                        break;
                    }
                }
                else
                {
                    if (i < radius - 1)
                    {
                        partsExplosionRight.Add(new PartExplosion(KindExplosionTexture.explosionTextureHorizontalMiddle, size, new Point((map.Width - size.Width), location.Y), countStates, onEndExplosionFunc));
                    }
                    else
                    {
                        partsExplosionRight.Add(new PartExplosion(KindExplosionTexture.explosionTextureRightEdge, size, new Point(map.Width - size.Width, location.Y), countStates, onEndExplosionFunc));
                    }
                    countParts++;
                    break;
                }
            }
            for (int i = 0; i < radius; i++)
            {
                if ((location.Y - size.Height * (i + 1)) > 0)
                {
                    if (!canWePlaceExplosion(location.X, location.Y - size.Height * (i), map, ExplosionDirection.UP))
                    {
                        if (i < radius - 1)
                        {
                            partsExplosionUp.Add(new PartExplosion(KindExplosionTexture.explosionTextureVerticalMiddle, size, new Point(location.X, location.Y - size.Height * (i + 1)), countStates, onEndExplosionFunc));

                        }
                        else
                        {
                            partsExplosionUp.Add(new PartExplosion(KindExplosionTexture.explosionTextureUpEdge, size, new Point(location.X, location.Y - size.Height * (i + 1)), countStates, onEndExplosionFunc));
                        }
                        countParts++;
                    }
                    else
                    {
                        partsExplosionUp.Add(new PartExplosion(KindExplosionTexture.explosionTextureVerticalMiddle, size, new Point(newLocation.X, newLocation.Y), countStates, onEndExplosionFunc));
                        break;
                    }

                }
                else
                {
                    if (i < radius - 1)
                    {
                        partsExplosionUp.Add(new PartExplosion(KindExplosionTexture.explosionTextureVerticalMiddle, size, new Point(location.X, 0), countStates, onEndExplosionFunc));
                    }
                    else
                    {
                        partsExplosionUp.Add(new PartExplosion(KindExplosionTexture.explosionTextureUpEdge, size, new Point(location.X, 0), countStates, onEndExplosionFunc));
                    }
                    countParts++;
                    break;
                }
            }
            for (int i = 0; i < radius; i++)
            {
                if ((location.Y + size.Height * (i + 1)) < map.Height)
                {
                    if (!canWePlaceExplosion(location.X, location.Y + size.Height * (i), map, ExplosionDirection.DOWN))
                    {
                        if (i < radius - 1)
                        {
                            partsExplosionBottom.Add(new PartExplosion(KindExplosionTexture.explosionTextureVerticalMiddle, size, new Point(location.X, location.Y + size.Height * (i + 1)), countStates, onEndExplosionFunc));
                        }
                        else
                        {
                            partsExplosionBottom.Add(new PartExplosion(KindExplosionTexture.explosionTextureBottomEdge, size, new Point(location.X, location.Y + size.Height * (i + 1)), countStates, onEndExplosionFunc));
                        }
                        countParts++;
                    }
                    else
                    {
                        partsExplosionUp.Add(new PartExplosion(KindExplosionTexture.explosionTextureVerticalMiddle, size, new Point(newLocation.X, newLocation.Y), countStates, onEndExplosionFunc));
                        break;
                    }
                }
                else
                {
                    if (i < radius - 1)
                    {
                        partsExplosionBottom.Add(new PartExplosion(KindExplosionTexture.explosionTextureVerticalMiddle, size, new Point(location.X, map.Height - size.Height), countStates, onEndExplosionFunc));
                    }
                    else
                    {
                        partsExplosionBottom.Add(new PartExplosion(KindExplosionTexture.explosionTextureBottomEdge, size, new Point(location.X, map.Height - size.Height), countStates, onEndExplosionFunc));
                    }
                    countParts++;
                    break;
                }
            }

            partExplosionCenter = new PartExplosion(KindExplosionTexture.explosionTextureCenter, size, location, countStates, onEndExplosionFunc);
            countParts++;
        }
        public void ChangePhysicalMap(PhysicalMap map)
        {
            for (int i = 0; i < partsExplosionBottom.Count; i++)
            {
                partsExplosionBottom[i].ChangePhysicalMap(map);
            }
            for (int i = 0; i < partsExplosionUp.Count; i++)
            {
                partsExplosionUp[i].ChangePhysicalMap(map);
            }
            for (int i = 0; i < partsExplosionLeft.Count; i++)
            {
                partsExplosionLeft[i].ChangePhysicalMap(map);
            }
            for (int i = 0; i < partsExplosionRight.Count; i++)
            {
                partsExplosionRight[i].ChangePhysicalMap(map);
            }
            partExplosionCenter.ChangePhysicalMap(map);
        }
        public void DrawPart(BufferedGraphics currBuffer, List<Bitmap> images, PartExplosion partExplosion)
        {
            lock (currBuffer)
            {
                lock (images)
                {
                    lock (images[(int)partExplosion.kindExplosion])
                    {
                        currBuffer.Graphics.DrawImage(images[(int)partExplosion.kindExplosion], partExplosion.location.X, partExplosion.location.Y, new Rectangle(new Point(partExplosion.currSpriteOffset, 0), partExplosion.size), GraphicsUnit.Pixel);
                    }
                }
            }
        }
        public void DrawExplosion(BufferedGraphics currBuffer, List<Bitmap> images)
        {
            for (int j = 0; j < partsExplosionBottom.Count; j++)
            {
                DrawPart(currBuffer, images, partsExplosionBottom[j]);
                //currBuffer.Graphics.DrawImage(partsExplosionBottom[j].currTexture, partsExplosionBottom[j].location);
            }/*
            for (int j = 0; j < partsExplosionBottom.Count; j++)
            {
                currBuffer.Graphics.DrawImage(partsExplosionBottom[j].currTexture, partsExplosionBottom[j].location);
            }*/
            for (int j = 0; j < partsExplosionUp.Count; j++)
            {
                DrawPart(currBuffer, images, partsExplosionUp[j]);

                //currBuffer.Graphics.DrawImage(partsExplosionUp[j].currTexture, partsExplosionUp[j].location);
            }
            for (int j = 0; j < partsExplosionLeft.Count; j++)
            {
                DrawPart(currBuffer, images, partsExplosionLeft[j]);

                //currBuffer.Graphics.DrawImage(partsExplosionLeft[j].currTexture, partsExplosionLeft[j].location);
            }
            for (int j = 0; j < partsExplosionRight.Count; j++)
            {
                DrawPart(currBuffer, images, partsExplosionRight[j]);

                //currBuffer.Graphics.DrawImage(partsExplosionRight[j].currTexture, partsExplosionRight[j].location);
            }
            DrawPart(currBuffer, images, partExplosionCenter);

            //currBuffer.Graphics.DrawImage(partExplosionCenter.currTexture, partExplosionCenter.location);
        }
        ~Explosion()
        {
            /*
            for (int i = 0; i < partsExplosionBottom.Count; i++)
            {
                partsExplosionBottom[i].texture.Dispose();
            }
            for (int i = 0; i < partsExplosionUp.Count; i++)
            {
                partsExplosionUp[i].texture.Dispose();
            }
            for (int i = 0; i < partsExplosionLeft.Count; i++)
            {
                partsExplosionLeft[i].texture.Dispose();
            }
            for (int i = 0; i < partsExplosionRight.Count; i++)
            {
                partsExplosionRight[i].texture.Dispose();
            }
            partExplosionCenter.texture.Dispose();
            */
        }
        public Explosion
            (
                Size size, Point location, int radius,
                PhysicalMap map, OnEndAllExplosionFunc funcEnd
            )
        {
            onEndAllExplosionFunc = funcEnd;
            explosionLocation = location;
            this.size = size;
            this.radius = radius;
            InitExplosion(size, location, radius, map);
            ChangeState();
        }
    }
}
