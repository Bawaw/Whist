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
            int heuristicHandStrength = GetHeuristicHandStrength(gameManager.Round.CurrentPlayer, gameManager.Round.Trump);
            int alternateHandStrength = heuristicHandStrength;
            Suits alternateSuit = gameManager.Round.Trump;
            foreach(Suits suit in System.Enum.GetValues(typeof(Suits)))
            {
                if(suit != gameManager.Round.Trump)
                {
                    int betterAlternateHandStrength = GetHeuristicHandStrength(gameManager.Round.CurrentPlayer, suit);
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
            if (heuristicHandStrength > 12.5 && possibleActions.Contains(Action.SOLOSLIM))
            {
                return Action.SOLOSLIM;
            }
            else if (alternateHandStrength > 12.5 && alternateHandStrength > heuristicHandStrength && possibleActions.Contains(Action.SOLO))
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
            else if (heuristicHandStrength < 0.5 && possibleActions.Contains(Action.MISERIE))
            {
                return Action.MISERIE;
            }
            return Action.PASS;
        }

        private int GetHeuristicHandStrength(Player currentPlayer, Suits trump)
        {
            int heuristicHandStrength = 0;
            for (int i = 0; i < 13; i++)
            {
                heuristicHandStrength += (int)currentPlayer.hand.Cards[i].Number;
                if (currentPlayer.hand.Cards[i].Suit == trump)
                {
                    heuristicHandStrength += 13;
                }
            }
            //sum of all cardStrengths = (2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12 + 13 + 14) * 3 + (15 + 16 + 17 + 18 + 19 + 20 + 21 + 22 + 23 + 24 + 25 + 26 + 27) = 585
            //your handStrength / sum of all cardStrengths * #tricks = statistical average number of tricks you will make in unlimited # games
            return heuristicHandStrength / 585 * 13;
        }
    }
}
