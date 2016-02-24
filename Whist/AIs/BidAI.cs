using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class BidAI : AI, IBidAI {

        public BidAI(Player player, GameManager game): base(player, game){ }

        public virtual Action GetAction()
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

            if (alternateHandStrength > 9)
                if (possibleActions.Contains(Action.ABONDANCE))
                    return Action.ABONDANCE;

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

        protected int GetHandStrength(Suits trump)
        {
            int handStrength = 0;

            var cards = player.hand.Cards;
            var kingsAndAces = cards.Where(c => c.Number == Numbers.ACE || c.Number == Numbers.KING);
            handStrength += kingsAndAces.Count();

            foreach (Suits suit in System.Enum.GetValues(typeof(Suits)))
            {
                if (suit != trump)
                {
                    var cardsOfSuit = cards.Where(c => c.Suit == suit);
                    Numbers i = Numbers.ACE;
                    while (i >= Numbers.TEN)
                    {
                        if (cardsOfSuit.Any(c => c.Number == i))
                            handStrength++;
                        else
                            break;
                        i--;
                    }
                }
            }

            var cardsOfTrump = cards.Where(c => c.Suit == trump);
            int holeCount = 2;
            Numbers n = Numbers.ACE;
            while (n >= Numbers.FIVE)
            {
                if (cardsOfTrump.Any(c => c.Number == n))
                    handStrength++;
                else
                    if (holeCount-- <= 0)
                    break;
                n--;
            }


            return handStrength;
        }
    }
}
