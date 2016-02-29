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

        /// <summary>
        /// For AIs that utilize memory of the round, the memory needs to be reset between rounds.
        /// </summary>
        public void ResetMemory()
        {
            GameAI.ResetMemory();
        }

        /// <summary>
        /// When a player (including this one) submits an action, this function should be called so it is made aware of the other player's action.
        /// </summary>
        public void ProcessOtherPlayerAction(Player otherPlayer, Action action)
        {
            GameAI.ProcessOtherPlayerAction(otherPlayer, action);
        }

        /// <summary>
        /// When a player (including this one) plays a card, this function should be called so it is made aware of the other player's move.
        /// </summary>
        public void ProcessOtherPlayerCard(Player otherPlayer, Card card)
        {
            GameAI.ProcessOtherPlayerCard(otherPlayer, card);
        }

        /// <summary>
        /// Ask the AI what action it wants to submit in the bidding phase.
        /// The actual submission of the action needs to be done trough <seealso cref="Round.BiddingDoAction(Action)"/>.
        /// </summary>
        /// <returns></returns>
        public Action GetAction()
        {
            return BidAI.GetAction();
        }

        /// <summary>
        /// Ask the AI what card it wants to play in the tricks phase.
        /// The actual playing of the card needs to be done trough <seealso cref="Round.PlayCard(Card)"/>.
        /// </summary>
        public Card GetMove()
        {
            return GameAI.GetMove();
        }
    }
}
