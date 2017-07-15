using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseColab.GameBusHere
{
    class MessageMouseDragged : IGameMessage
    {
        public int ChangeX { get; set; }
        public int ChangeY { get; set; }
    }
}
