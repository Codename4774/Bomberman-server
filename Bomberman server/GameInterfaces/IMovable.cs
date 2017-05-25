using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman_client.GameClasses;

namespace Bomberman_client.GameInterfaces
{
    public interface IMovable
    {
        void OnMove(PhysicalMap PhysicalMap);
    }
}
