using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class AI
    {
        protected IBidAI BidAI { get; private set; }
        protected IGameAI GameAI { get; private set; }


        protected Player player;


        public AI(Player player, IBidAI bidAI, IGameAI gameAI)
        {
            this.player = player;
            BidAI = bidAI;
            GameAI = gameAI;
        }

        public void ResetMemory()
        {
            GameAI.ResetMemory();
        }

        public void ProcessOtherPlayerAction(Player otherPlayer, Action action)
        {

        }

        public void ProcessOtherPlayerCard(Player otherPlayer, Card card)
        {
            GameAI.ProcessOtherPlayerCard(otherPlayer, card);
        }

        public Action GetAction()
        {
            return BidAI.GetAction();
        }

        
        public Card GetMove()
        {
            return GameAI.GetMove();
        }
    }
}
