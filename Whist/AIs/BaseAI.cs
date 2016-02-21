using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class BaseAI : AI
    {
        public BaseAI(Player player, GameManager game) :base(player, game)
        {

        }
    }
}
