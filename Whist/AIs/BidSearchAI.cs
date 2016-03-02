using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    class BidSearchAI : IBidAI
    {
        private GameManager gameManager;

        public BidSearchAI(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public Action GetAction()
        {
            var possibleActions = gameManager.Round.BiddingGetPossibleActions();
            double heuristicHandStrength = GetHeuristicHandStrength(gameManager.Round.CurrentPlayer, gameManager.Round.Trump);
            double alternateHandStrength = heuristicHandStrength;
            Suits alternateSuit = gameManager.Round.Trump;
            foreach(Suits suit in System.Enum.GetValues(typeof(Suits)))
            {
                if(suit != gameManager.Round.Trump)
                {
                    double betterAlternateHandStrength = GetHeuristicHandStrength(gameManager.Round.CurrentPlayer, suit);
                    if(betterAlternateHandStrength > alternateHandStrength)
                    {
                        alternateHandStrength = betterAlternateHandStrength;
                        alternateSuit = suit;
                    }
                }
            }
            if (!possibleActions.Contains(Action.PASS))
            {
                switch (alternateSuit)
                {
                    case Suits.HEARTS:
                        return Action.HEARTS;
                    case Suits.CLUBS:
                        return Action.CLUBS;
                    case Suits.DIAMONDS:
                        return Action.DIAMONDS;
                    case Suits.SPADES:
                        return Action.SPADES;
                }
            }
            if (heuristicHandStrength > 12 && possibleActions.Contains(Action.SOLOSLIM)) //you only have more than 12.5 if you have all trumps, but with all trumps except the 2 and an ace, you should also make all tricks, so therefore > 12 instead of > 12.5
            {
                return Action.SOLOSLIM;
            }
            else if (alternateHandStrength > 12 && alternateHandStrength > heuristicHandStrength && possibleActions.Contains(Action.SOLO)) //same as above
            {
                return Action.SOLO;
            }
            else if (heuristicHandStrength > 8.5 || alternateHandStrength > 8.5 && possibleActions.Contains(Action.ABONDANCE))
            {
                return Action.ABONDANCE;
            }
            else if (heuristicHandStrength > 4.5 && possibleActions.Contains(Action.ASK))
            {
                return Action.ASK;
            }
            else if (heuristicHandStrength > 4.5 && possibleActions.Contains(Action.ALONE))
            {
                return Action.ALONE;
            }
            else if (heuristicHandStrength > 2.5 && possibleActions.Contains(Action.JOIN))
            {
                return Action.JOIN;
            }
            else if (heuristicHandStrength < 1 && possibleActions.Contains(Action.MISERIE)) //you can never have lower than 0.5, even with the worst hand, so therefore < 1 instead of < 0.5
            {
                return Action.MISERIE;
            }
            return Action.PASS;
        }

        private double GetHeuristicHandStrength(Player currentPlayer, Suits trump)
        {
            double predictionOfTricks = 0;

            foreach(Card card in currentPlayer.hand.Cards)
            {
                predictionOfTricks += (int)card.Number - 1;//2 is card rank 1, 3 is card rank 2, ..., king is card rank 12, ace is card rank 13
                if(card.Suit == trump)
                {
                    predictionOfTricks += 39;//trump is stronger than all other suits(13 * 3 = 39)
                }
            }
            //return predictionOfTricks / 52;//divide by total amount of cards(chance of card winning a trick)      this works better for worse cards
            return predictionOfTricks / 598 * 13;//all trumpvalues added equals 598(max value of cards in hand)     this works better for better cards

            //double heuristicHandStrength = 0;
            //for (int i = 0; i < 13; i++)
            //{
            //    heuristicHandStrength += (int)currentPlayer.hand.Cards[i].Number;
            //    if (currentPlayer.hand.Cards[i].Suit == trump)
            //    {
            //        heuristicHandStrength += 13;
            //    }
            //}
            //sum of all cardStrengths = (2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12 + 13 + 14) * 3 + (15 + 16 + 17 + 18 + 19 + 20 + 21 + 22 + 23 + 24 + 25 + 26 + 27) = 585
            //your handStrength / sum of all cardStrengths * #tricks = statistical average number of tricks you will make in unlimited # games
            //return heuristicHandStrength / 273 * 13; 273 is sum of all trump values
        }
    }
}
