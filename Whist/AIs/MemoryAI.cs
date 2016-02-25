using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class MemoryAI : AI
    {
        public MemoryAI(Player player, GameManager game) : base(player, game)
        {

        }

        public override void ProcessOtherPlayerAction(Player otherPlayer, Action action)
        {

        }

        public override void ProcessOtherPlayerCard(Player otherPlayer, Card card)
        {

        }
        /*
        public override Action GetAction()
        {
            return base.GetAction();
        }

        public override Card GetMove()
        {
            return base.GetMove();
        }*/
    }
}
