using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman_client
{
    public class KindMessages
    {
        public enum KindMessage { Player = 0, Wall = 1 };
        public enum KindPlayerMessages { NewDirection = 0, Spawn = 1, Death = 2, PlaceBomb = 3, Connect = 4, Disconnect = 5, Location = 6, StopWalking = 7 };

    }
}
