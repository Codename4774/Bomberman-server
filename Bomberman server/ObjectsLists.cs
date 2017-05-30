using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman_client.GameClasses;

namespace Bomberman_server
{
    [Serializable]
    public class ObjectsLists
    {
        public List<Bomb> bombs;
        public List<Explosion> explosions;
        public List<PhysicalObject> staticWalls;
        public List<DynamicWall> dynamicWalls;
        public List<Player> players;
        public ObjectsLists()
        {
            bombs = new List<Bomb>();
            explosions = new List<Explosion>();
            staticWalls = new List<PhysicalObject>();
            dynamicWalls = new List<DynamicWall>();
            players = new List<Player>();
        }
        public ObjectsLists(List<Bomb> bombs, List<Explosion> explosions, List<PhysicalObject> staticWalls, List<DynamicWall> dynamicWalls, List<Player> players)
        {
            this.bombs = bombs;
            this.explosions = explosions;
            this.staticWalls = staticWalls;
            this.dynamicWalls = dynamicWalls;
            this.players = players;
        }
    }
}
