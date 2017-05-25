using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;

namespace Bomberman_client.GameClasses
{
    public class GameCoreServer
    {
        private Graphics graphicControl;
        private BufferedGraphicsContext currentContext = new BufferedGraphicsContext();
        private BufferedGraphics buffer1, buffer2, currBuffer;
        public Bitmap buffer;
        private Color buffColor = new Color();

        public delegate void SendFunc();
        private SendFunc sendFunc;

        public PhysicalMap map;
        public List<Bomb> bombs;
        public List<Explosion> explosions;
        public List<PhysicalObject> staticWalls;
        public List<DynamicWall> dynamicWalls;

        public List<Player> players;

        public int delay;
        private Timer timer;

        private ScriptEngine scriptEngine;

        public Size playerSize;
        public Size playerOnDeathSize;
        public Size bombSize;
        public Size explosionSize;
        public Size wallSize;
       
        public Image bombTexture;
        public Image bombExplosionTexture;
        public Image playerTexture;
        public Image playerDieTexture;
        public Image staticWallTexture;
        public Image dynamicWallTexture;
        public Image dynamicWallDestroyTexture;
        public Image explosionCenterTexture;
        public Image explosionLeftEdgeTexture;
        public Image explosionRightEdgeTexture;
        public Image explosionUpEdgeTexture;
        public Image explosionBottomEdgeTexture;
        public Image explosionVerticalTexture;
        public Image explosionHorizontalTexture;


        public void RedrawFunc()
        {
            currBuffer.Render();
            currBuffer.Graphics.Clear(buffColor);
            ChangeBuffer();
            CalcBuff();
        }
        public void CalcBuff()
        {
            foreach (Player player in players)
            {
                if (!player.IsDead)
                {
                    lock (player)
                    {
                        if (!player.IsDying)
                        {
                            currBuffer.Graphics.DrawImage(player.GetAnimState(), player.X, player.Y);
                        }
                        else
                        {
                            currBuffer.Graphics.DrawImage(player.texture, player.X, player.Y);
                        }
                    }
                }
            }
            lock (bombs)
            {
                for (int i = 0; i < bombs.Count; i++)
                {
                    lock (bombs[i])
                    {
                        currBuffer.Graphics.DrawImage(bombs[i].texture, bombs[i].X, bombs[i].Y);
                    }
                }
            }
            for (int i = 0; i < explosions.Count; i++)
            {
                lock (explosions[i])
                {
                    explosions[i].DrawExplosion(currBuffer);
                }
            }
            for (int i = 0; i < dynamicWalls.Count; i++)
            {
                lock (dynamicWalls[i])
                {
                    currBuffer.Graphics.DrawImage(dynamicWalls[i].texture, dynamicWalls[i].X, dynamicWalls[i].Y);
                }
            }
            for (int i = 0; i < staticWalls.Count; i++)
            {
                lock (staticWalls[i])
                {
                    currBuffer.Graphics.DrawImage(staticWalls[i].texture, staticWalls[i].X, staticWalls[i].Y);
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

        public void ChangePhysicalState()
        {
            foreach (Player player in players)
            {
                if (!player.IsDead)
                {
                    player.ChangeMapMatrix(map);
                }
            }
            for (int i = 0; i < bombs.Count; i++)
            {
                bombs[i].ChangeMapMatrix(map);
            }
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].ChangePhysicalMap(map);
            }
            for (int i = 0; i < dynamicWalls.Count; i++)
            {
                if ((dynamicWalls[i].isWallBlowedUp(map)) && (!dynamicWalls[i].isBlowedUpNow))
                {
                    dynamicWalls[i].isBlowedUpNow = true;
                    StartDestroingDynamicWall(dynamicWalls[i]);
                }
                dynamicWalls[i].ChangeMapMatrix(map);
            }
            for (int i = 0; i < staticWalls.Count; i++)
            {
                staticWalls[i].ChangeMapMatrix(map);
            }
            foreach (Player player in players)
            {
                if (player.isPlayerBlowedUp(map))
                {
                    onDeathPlayer(player);
                }
            }
        }



        public void TimerEvent(object sender, EventArgs e)
        {
            foreach (Player player in players)
            {
                player.OnMove(map);
            }
            RedrawFunc();
            sendFunc();
            map.ClearCurrMatrix();
            map.SwitchMatrix();
            ChangePhysicalState();
        }

        public void onDeathPlayer(PhysicalObject player)
        {
            var temp = player as Player;

            lock (player.texture)
            {
                scriptEngine.StartSimpleScript(temp, playerDieTexture, playerOnDeathSize, DeletePlayerFromField, 200, 6);
            }
            temp.IsDying = true;
            temp.isMoved = false;
        }

        public void DeletePlayerFromField(object player)
        {
            var temp = player as Player;

            temp.IsDead = true;
        }

        public void ExplosionBomb(PhysicalObject bomb)
        {
            var temp = bomb as Bomb;

            scriptEngine.StartSimpleScript(temp, bombExplosionTexture, DeleteBombFromField, 100, 3);
        }

        public void DeleteBombFromField(object bomb)
        {
            var temp = bomb as Bomb;

            if (bombs.IndexOf(temp) >= 0)
            {
                temp.owner.CurrCountBombs--;
                bombs.Remove(temp);

                Explosion tempExpl = new Explosion(explosionCenterTexture, explosionUpEdgeTexture, explosionBottomEdgeTexture, explosionLeftEdgeTexture,
                   explosionRightEdgeTexture, explosionVerticalTexture, explosionHorizontalTexture, explosionSize,
                   new Point(temp.X, temp.Y), (int)temp.owner.bombLevel, map, DeleteExplosionFromField);

                explosions.Add(tempExpl);
                scriptEngine.StartExplosion(tempExpl, DeleteExplosionFromField, 100, 7);

            }
        }

        public void DeleteExplosionFromField(object explosion)
        {
            var temp = explosion as Explosion;

            explosions.Remove(temp);
        }

        public void StartDestroingDynamicWall(DynamicWall wall)
        {
            scriptEngine.StartSimpleScript(wall, dynamicWallDestroyTexture, DeleteDynamicWallFromField, 200, 6);
        }

        public void DeleteDynamicWallFromField(object wall)
        {
            var temp = wall as DynamicWall;

            if (dynamicWalls.IndexOf(temp) >= 0)
            {
                dynamicWalls.Remove(temp);
            }
        }

        private void spawnWalls()
        {
            staticWalls.Add(new PhysicalObject(new Point(50, 50), staticWallTexture, wallSize));
            dynamicWalls.Add(new DynamicWall(new Point(100, 100), dynamicWallTexture, wallSize));
            dynamicWalls.Add(new DynamicWall(new Point(124, 100), dynamicWallTexture, wallSize));
            dynamicWalls.Add(new DynamicWall(new Point(148, 100), dynamicWallTexture, wallSize));
        }

        private void LoadImages(string resDir)
        {
            this.bombTexture = new Bitmap(resDir + "Bomb\\bomb.png");
            this.bombExplosionTexture = new Bitmap(resDir + "Bomb\\BombExplosion.png");
            this.playerTexture = new Bitmap(resDir + "Player\\bomberman_new.png");
            this.playerDieTexture = new Bitmap(resDir + "Player\\bomberman_death.png");
            this.explosionCenterTexture = new Bitmap(resDir + "Explosion\\ExplosionCenter.png");
            this.explosionUpEdgeTexture= new Bitmap(resDir + "Explosion\\ExplosionUpEdge.png");
            this.explosionBottomEdgeTexture = new Bitmap(resDir + "Explosion\\ExplosionBottomEdge.png");
            this.explosionLeftEdgeTexture = new Bitmap(resDir + "Explosion\\ExplosionLeftEdge.png");
            this.explosionRightEdgeTexture = new Bitmap(resDir + "Explosion\\ExplosionRightEdge.png");
            this.explosionVerticalTexture = new Bitmap(resDir + "Explosion\\ExplosionVerticalMiddle.png");
            this.explosionHorizontalTexture = new Bitmap(resDir + "Explosion\\ExplosionHorizontalMiddle.png");
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
            bombs = new List<Bomb>();
            explosions = new List<Explosion>();
            staticWalls = new List<PhysicalObject>();
            dynamicWalls = new List<DynamicWall>();
            this.sendFunc = sendFunc;

            timer = new Timer();
            delay = 60;
            timer.Interval = delay;
            timer.Elapsed += TimerEvent;

            scriptEngine = new ScriptEngine();
            this.players = new List<Player>();
            this.buffer = new Bitmap(width, height);
            graphicControl = Graphics.FromImage(buffer);
            buffer1 = currentContext.Allocate(graphicControl, new Rectangle(0, 0, width, height));
            buffer2 = currentContext.Allocate(graphicControl, new Rectangle(0, 0, width, height));
            currBuffer = buffer1;
            buffColor = Color.ForestGreen;


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
