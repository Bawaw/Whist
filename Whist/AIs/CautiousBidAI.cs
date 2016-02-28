using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class CautiousBidAI : BaseBidAI
    {
        public CautiousBidAI(Player player, GameManager game) : base(player, game)
        {
        }

        public override Action GetAction()
        {
            var possibleActions = Round.BiddingGetPossibleActions();

            int handStrength = GetHandStrength(Round.Trump);

            int alternateHandStrength = 0;
            Suits alternateSuit = Round.Trump;
            foreach (Suits suit in System.Enum.GetValues(typeof(Suits)))
            {
                if (suit != Round.Trump)
                {
                    int temp = GetHandStrength(suit);
                    if (temp > alternateHandStrength)
                    {
                        alternateHandStrength = temp;
                        alternateSuit = suit;
                    }
                }
            }

            if (!possibleActions.Contains(Action.PASS))
            {
                Suits preferredSuit = Round.Trump;
                if (alternateHandStrength > handStrength)
                    preferredSuit = alternateSuit;

                switch (preferredSuit)
                {
                    case Suits.HEARTS:
                        return Action.HEARTS;
                    case Suits.DIAMONDS:
                        return Action.DIAMONDS;
                    case Suits.SPADES:
                        return Action.SPADES;
                    case Suits.CLUBS:
                        return Action.CLUBS;
                    default:
                        return 0;
                }
            }

            
            if (handStrength > 10 || alternateHandStrength > 10)
            {
                if (possibleActions.Contains(Action.ABONDANCE))
                    return Action.ABONDANCE;
            }
            if (handStrength >= 7)
            {
                if (possibleActions.Contains(Action.ALONE))
                    return Action.ALONE;
            }
            if (handStrength >= 6)
            {
                if (possibleActions.Contains(Action.ASK))
                    return Action.ASK;
            }
            if (handStrength >= 4)
            {
                if (possibleActions.Contains(Action.JOIN))
                    return Action.JOIN;
            }

            return Action.PASS;
        }
    }
}
