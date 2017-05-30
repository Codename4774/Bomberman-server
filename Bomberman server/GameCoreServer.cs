using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;
using Bomberman_server;

namespace Bomberman_client.GameClasses
{
    [Serializable]
    public class GameCoreServer
    {
        public delegate void SendFunc();
        [NonSerialized]
        private SendFunc sendFunc;

        [NonSerialized]
        public PhysicalMap map;
        public ObjectsLists objectsList;

        [NonSerialized]
        public int delay;
        [NonSerialized]
        private Timer timer;
        [NonSerialized]
        private ScriptEngine scriptEngine;
        [NonSerialized]
        public Size playerSize;
        [NonSerialized]
        public Size playerOnDeathSize;
        [NonSerialized]
        public Size bombSize;
        [NonSerialized]
        public Size explosionSize;
        [NonSerialized]
        public Size wallSize;
       [NonSerialized]
        public Bitmap bombTexture;
        [NonSerialized]
        public Bitmap bombExplosionTexture;
        [NonSerialized]
        public Bitmap playerTexture;
        [NonSerialized]
        public Bitmap playerDieTexture;
        [NonSerialized]
        public Bitmap staticWallTexture;
        [NonSerialized]
        public Bitmap dynamicWallTexture;
        [NonSerialized]
        public Bitmap dynamicWallDestroyTexture;
        [NonSerialized]
        public List<Bitmap> explosionsTexture;
/*
        public void RedrawFunc()
        {
            currBuffer.Render();
            currBuffer.Graphics.Clear(buffColor);
            ChangeBuffer();
            CalcBuff();
        }
        public void CalcBuff()
        {

            foreach (Player player in objectsList.players)
            {
                if (!player.IsDead)
                {
                    lock (player)
                    {
                        if (!player.IsDying)
                        {
                            currBuffer.Graphics.DrawImage(player.GetAnimState(playerTexture), player.X, player.Y);
                        }
                        else
                        {
                            lock (playerDieTexture)
                            {

                                currBuffer.Graphics.DrawImage(playerDieTexture, player.X, player.Y, new Rectangle(new Point(player.currSpriteOffset, 0), player.size), GraphicsUnit.Pixel);
                            }
                            //currBuffer.Graphics.DrawImage(player.texture, player.X, player.Y);

                        }
                    }
                }
            }
            lock (objectsList.bombs)
            {
                for (int i = 0; i < objectsList.bombs.Count; i++)
                {
                    lock (objectsList.bombs[i])
                    {
                        lock (bombTexture)
                        {
                            if (objectsList.bombs[i].isBlowedUp)
                            {
                                currBuffer.Graphics.DrawImage(bombExplosionTexture, objectsList.bombs[i].X, objectsList.bombs[i].Y, new Rectangle(new Point(objectsList.bombs[i].currSpriteOffset, 0), objectsList.bombs[i].size), GraphicsUnit.Pixel);
                            }
                            else
                            {
                                currBuffer.Graphics.DrawImage(bombTexture, objectsList.bombs[i].X, objectsList.bombs[i].Y);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < objectsList.explosions.Count; i++)
            {
                lock (objectsList.explosions[i])
                {
                    objectsList.explosions[i].DrawExplosion(currBuffer, explosionsTexture);
                }
            }
            for (int i = 0; i < objectsList.dynamicWalls.Count; i++)
            {
                lock (objectsList.dynamicWalls[i])
                {
                    if (!objectsList.dynamicWalls[i].isBlowedUpNow)
                    {
                        lock (dynamicWallTexture)
                        {
                            currBuffer.Graphics.DrawImage(dynamicWallTexture, objectsList.dynamicWalls[i].X, objectsList.dynamicWalls[i].Y);
                        }
                    }
                    else
                    {
                        lock (dynamicWallDestroyTexture)
                        {
                            currBuffer.Graphics.DrawImage(dynamicWallDestroyTexture, objectsList.dynamicWalls[i].X, objectsList.dynamicWalls[i].Y, new Rectangle(new Point(objectsList.dynamicWalls[i].currSpriteOffset, 0), objectsList.dynamicWalls[i].size), GraphicsUnit.Pixel);
                        }
                    }
                }
            }
            for (int i = 0; i < objectsList.staticWalls.Count; i++)
            {
                lock (objectsList.staticWalls[i])
                {
                    lock (staticWallTexture)
                    {
                        currBuffer.Graphics.DrawImage(staticWallTexture, objectsList.staticWalls[i].X, objectsList.staticWalls[i].Y);
                    }
                }
            }
        }

        public void ChangeBuffer()
        {
            if (currBuffer == buffer1)
            {
                currBuffer = buffer2;
            }
            else
            {
                currBuffer = buffer1;
            }
        }
        */
        public void ChangePhysicalState()
        {
            foreach (Player player in objectsList.players)
            {
                if (!player.IsDead)
                {
                    player.ChangeMapMatrix(map);
                }
            }
            for (int i = 0; i < objectsList.bombs.Count; i++)
            {
                objectsList.bombs[i].ChangeMapMatrix(map);
            }
            for (int i = 0; i < objectsList.explosions.Count; i++)
            {
                objectsList.explosions[i].ChangePhysicalMap(map);
            }
            for (int i = 0; i < objectsList.dynamicWalls.Count; i++)
            {
                if ((objectsList.dynamicWalls[i].isWallBlowedUp(map)) && (!objectsList.dynamicWalls[i].isBlowedUpNow))
                {
                    objectsList.dynamicWalls[i].isBlowedUpNow = true;
                    StartDestroingDynamicWall(objectsList.dynamicWalls[i]);
                }
                objectsList.dynamicWalls[i].ChangeMapMatrix(map);
            }
            for (int i = 0; i < objectsList.staticWalls.Count; i++)
            {
                objectsList.staticWalls[i].ChangeMapMatrix(map);
            }
            foreach (Player player in objectsList.players)
            {
                if (player.isPlayerBlowedUp(map))
                {
                    onDeathPlayer(player);
                }
            }
        }



        public void TimerEvent(object sender, EventArgs e)
        {
            foreach (Player player in objectsList.players)
            {
                player.OnMove(map);
            }
            //RedrawFunc();
            sendFunc();
            map.ClearCurrMatrix();
            map.SwitchMatrix();
            ChangePhysicalState();
        }

        public void onDeathPlayer(PhysicalObject player)
        {
            var temp = player as Player;

            scriptEngine.StartSimpleScript(temp, playerOnDeathSize, DeletePlayerFromField, 200, 6);
            temp.IsDying = true;
            temp.isMoved = false;
        }

        public void DeletePlayerFromField(object player)
        {
            var temp = player as Player;

            temp.IsDead = true;
            temp.currSpriteOffset = 0;
        }

        public void ExplosionBomb(PhysicalObject bomb)
        {
            var temp = bomb as Bomb;

            temp.isBlowedUp = true;
            scriptEngine.StartSimpleScript(temp, DeleteBombFromField, 100, 3);
        }

        public void DeleteBombFromField(object bomb)
        {
            var temp = bomb as Bomb;

            if (objectsList.bombs.IndexOf(temp) >= 0)
            {
                temp.owner.CurrCountBombs--;
                objectsList.bombs.Remove(temp);

                Explosion tempExpl = new Explosion(explosionSize, new Point(temp.X, temp.Y), (int)temp.owner.bombLevel, map, DeleteExplosionFromField);

                objectsList.explosions.Add(tempExpl);
                scriptEngine.StartExplosion(tempExpl, DeleteExplosionFromField, 100, 7);

            }
        }

        public void DeleteExplosionFromField(object explosion)
        {
            var temp = explosion as Explosion;

            objectsList.explosions.Remove(temp);
        }

        public void StartDestroingDynamicWall(DynamicWall wall)
        {
            scriptEngine.StartSimpleScript(wall, DeleteDynamicWallFromField, 200, 6);
        }

        public void DeleteDynamicWallFromField(object wall)
        {
            var temp = wall as DynamicWall;

            if (objectsList.dynamicWalls.IndexOf(temp) >= 0)
            {
                objectsList.dynamicWalls.Remove(temp);
            }
        }

        private void spawnWalls()
        {
            objectsList.staticWalls.Add(new PhysicalObject(new Point(50, 50), wallSize));
            objectsList.dynamicWalls.Add(new DynamicWall(new Point(100, 100), wallSize));
            objectsList.dynamicWalls.Add(new DynamicWall(new Point(124, 100), wallSize));
            objectsList.dynamicWalls.Add(new DynamicWall(new Point(148, 100), wallSize));
        }

        private void LoadImages(string resDir)
        {
            this.bombTexture = new Bitmap(resDir + "Bomb\\bomb.png");
            this.bombExplosionTexture = new Bitmap(resDir + "Bomb\\BombExplosion.png");
            this.playerTexture = new Bitmap(resDir + "Player\\bomberman_new.png");
            this.playerDieTexture = new Bitmap(resDir + "Player\\bomberman_death.png");
            //public enum KindExplosionTexture { explosionTextureHorizontalMiddle, explosionTextureLeftEdge, explosionTextureRightEdge, explosionTextureVerticalMiddle, explosionTextureUpEdge, explosionTextureBottomEdge, explosionTextureCenter };

            this.explosionsTexture.Add(new Bitmap(resDir + "Explosion\\ExplosionHorizontalMiddle.png"));
            this.explosionsTexture.Add(new Bitmap(resDir + "Explosion\\ExplosionLeftEdge.png"));
            this.explosionsTexture.Add(new Bitmap(resDir + "Explosion\\ExplosionRightEdge.png"));
            this.explosionsTexture.Add(new Bitmap(resDir + "Explosion\\ExplosionVerticalMiddle.png"));
            this.explosionsTexture.Add(new Bitmap(resDir + "Explosion\\ExplosionUpEdge.png"));
            this.explosionsTexture.Add(new Bitmap(resDir + "Explosion\\ExplosionBottomEdge.png"));
            this.explosionsTexture.Add(new Bitmap(resDir + "Explosion\\ExplosionCenter.png"));
            this.staticWallTexture = new Bitmap(resDir + "Walls\\StaticWall.png");
            this.dynamicWallTexture = new Bitmap(resDir + "Walls\\DynamicWall.png");
            this.dynamicWallDestroyTexture = new Bitmap(resDir + "Walls\\DynamicWallDestroy.png");
        }

        public void startCore()
        {
            timer.Start();
        }
        public GameCoreServer
            (
                int width, int height, Size playerSize, Size playerOnDeathSize,
                Size bombSize, Size explosionSize, Size wallSize, string dirResources, SendFunc sendFunc
            )
        {
            this.map = new PhysicalMap(width, height);
            objectsList = new ObjectsLists();
            this.sendFunc = sendFunc;

            timer = new Timer();
            delay = 60;
            timer.Interval = delay;
            timer.Elapsed += TimerEvent;

            scriptEngine = new ScriptEngine();
            this.explosionsTexture = new List<Bitmap>();

            this.playerSize = playerSize;
            this.bombSize = bombSize;
            this.explosionSize = explosionSize;
            this.wallSize = wallSize;
            this.playerOnDeathSize = playerOnDeathSize;

            LoadImages(dirResources);
            spawnWalls();
        }
    }
}
