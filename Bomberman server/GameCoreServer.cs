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
        public List<Point> spawnPoints;
        [NonSerialized]
        public Random randomGen;
        public void ChangePhysicalState()
        {
            lock (objectsList)
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
                        OnDeathPlayer(player);
                    }
                }
            }
        }

        public void TimerEvent(object sender, EventArgs e)
        {
            foreach (Player player in objectsList.players)
            {
                player.OnMove(map);
            }
            sendFunc();
            map.ClearCurrMatrix();
            map.SwitchMatrix();
            ChangePhysicalState();
        }

        public void OnDeathPlayer(PhysicalObject player)
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

        private void GenerateWalls()
        {
            const int sizeCell = 24;

            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(2 * sizeCell, 0 * sizeCell), 4, MapGenerator.LineDirection.Horizontal, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(3 * sizeCell, 1 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(1 * sizeCell, 2 * sizeCell), 1, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(2 * sizeCell, 2 * sizeCell), 2, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(0 * sizeCell, 3 * sizeCell), 1, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(1 * sizeCell, 5 * sizeCell), 1, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(3 * sizeCell, 7 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(3 * sizeCell, 11 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(4 * sizeCell, 13 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(5 * sizeCell, 13 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(5 * sizeCell, 5 * sizeCell), 5, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(6 * sizeCell, 5 * sizeCell), 2, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(7 * sizeCell, 1 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(7 * sizeCell, 8 * sizeCell), 2, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(8 * sizeCell, 9 * sizeCell), 1, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(9 * sizeCell, 0 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(9 * sizeCell, 10 * sizeCell), 2, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(10 * sizeCell, 8 * sizeCell), 2, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(10 * sizeCell, 14 * sizeCell), 2, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(11 * sizeCell, 0 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(12 * sizeCell, 3 * sizeCell), 1, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(12 * sizeCell, 6 * sizeCell), 1, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(12 * sizeCell, 12 * sizeCell), 4, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(13 * sizeCell, 0 * sizeCell), 4, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(13 * sizeCell, 5 * sizeCell), 2, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(13 * sizeCell, 11 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(15 * sizeCell, 4 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.dynamicWalls.AddRange(MapGenerator.GenerateLineDynamicWall(new Point(15 * sizeCell, 8 * sizeCell), 3, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(1 * sizeCell, 3 * sizeCell), 1, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(0 * sizeCell, 8 * sizeCell), 5, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(1 * sizeCell, 8 * sizeCell), 5, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(2 * sizeCell, 1 * sizeCell), 1, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(4 * sizeCell, 7 * sizeCell), 6, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(4 * sizeCell, 4 * sizeCell), 4, MapGenerator.LineDirection.Horizontal, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(5 * sizeCell, 1 * sizeCell), 1, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(7 * sizeCell, 7 * sizeCell), 11, MapGenerator.LineDirection.Horizontal, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(8 * sizeCell, 10 * sizeCell), 4, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(11 * sizeCell, 3 * sizeCell), 4, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(11 * sizeCell, 9 * sizeCell), 5, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(17 * sizeCell, 8 * sizeCell), 6, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(14 * sizeCell, 11 * sizeCell), 2, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(15 * sizeCell, 11 * sizeCell), 2, MapGenerator.LineDirection.Vertical, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(14 * sizeCell, 2 * sizeCell), 3, MapGenerator.LineDirection.Horizontal, wallSize));
            objectsList.staticWalls.AddRange(MapGenerator.GenerateLineStaticWall(new Point(14 * sizeCell, 3 * sizeCell), 3, MapGenerator.LineDirection.Horizontal, wallSize));
            spawnPoints.Add(new Point(0 * sizeCell, 0 * sizeCell));
            spawnPoints.Add(new Point(0 * sizeCell, 14 * sizeCell));
            spawnPoints.Add(new Point(17 * sizeCell, 0 * sizeCell));
            spawnPoints.Add(new Point(14 * sizeCell, 13 * sizeCell));

        }

        public void StartCore()
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
            spawnPoints = new List<Point>();
            this.sendFunc = sendFunc;

            timer = new Timer();
            delay = 60;
            timer.Interval = delay;
            timer.Elapsed += TimerEvent;
            randomGen = new Random();

            scriptEngine = new ScriptEngine();

            this.playerSize = playerSize;
            this.bombSize = bombSize;
            this.explosionSize = explosionSize;
            this.wallSize = wallSize;
            this.playerOnDeathSize = playerOnDeathSize;

            GenerateWalls();
        }
    }
}
