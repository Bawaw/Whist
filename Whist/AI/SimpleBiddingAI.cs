using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.ControlEntities;
using Whist.GameLogic;

namespace Whist.AI
{
    class SimpleBiddingAI
    {
        public Action GetAction(Player player, IEnumerable<Action> possibleActions, Suits trump)
        {
            int handStrength = GetHandStrength(player, trump);


            if (handStrength >= 9)
            {
                if (possibleActions.Contains(Action.ABONDANCE))
                    return Action.ABONDANCE;
            }
            if (handStrength >= 5)
            {
                if (possibleActions.Contains(Action.ASK))
                    return Action.ASK;
                if (possibleActions.Contains(Action.ALONE))
                    return Action.ALONE;
            }
            if (handStrength >= 3)
            {
                if (possibleActions.Contains(Action.JOIN))
                    return Action.JOIN;
            }

            return Action.PASS;
        }

        private int GetHandStrength(Player player, Suits trump)
        {
            int handStrength = 0;

            var cards = player.hand.Cards;
            var kingsAndAces = cards.Where(c => c.Number == Numbers.ACE || c.Number == Numbers.KING);
            handStrength += kingsAndAces.Count();
            var trumps = cards.Where(c => c.Suit == trump);
            handStrength += trumps.Except(kingsAndAces).Count();

            return handStrength;
        }
    }
}
