using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    class OmniscentSearchAI : IGameAI
    {
        private GameManager gameManager;
        private IReferee referee;

        public OmniscentSearchAI(GameManager gameManager, IReferee referee)
        {
            this.gameManager = gameManager;
            this.referee = referee;
        }

        //how far is player from winning all tricks
        public int GetPlayerHeuristic()
        {
            return 13 - CurrentTeam().Tricks;
        }

        public int GetHeuristicalHandChances(IList<Card> hand, Suits lead)
        {
            var heuristicScore = 0;

            var highestCard = hand[0];
            foreach (var card in hand)
            {
                if (card.Suit == lead || card.Suit == gameManager.Round.Trump)
                {
                    if (card.Suit == lead && highestCard.Suit != gameManager.Round.Trump && (int)card.Number > (int)highestCard.Number)
                    {
                        highestCard = card;
                        heuristicScore = (int)card.Number;
                    }
                    else if (card.Suit == gameManager.Round.Trump)
                    {
                        if (highestCard.Suit != gameManager.Round.Trump)
                        {
                            highestCard = card;
                            heuristicScore = (int)card.Number + 13;
                        }
                        else if (card.Number > highestCard.Number)
                        {
                            highestCard = card;
                            heuristicScore = (int)card.Number + 13;
                        }
                    }
                }
            }

            //trump 2 is value 15, ace lead is value 14, not of type lead = 0;
            return heuristicScore;
        }

        private Team CurrentTeam()
        {
            return gameManager.Round.Teams.Where(t => t.Players.Any(p => p == gameManager.Round.CurrentPlayer)).Single();
        }

        public Card GetMove()
        {
            var playableCards = (gameManager.Round.Pile.Count > 0) ? gameManager.Round.CurrentPlayer.hand.Cards.Where(x => referee.ValidateMove(x, gameManager.Round.Pile[0], gameManager.Round.CurrentPlayer.hand.Cards.ToList())).ToList() : gameManager.Round.CurrentPlayer.hand.Cards.ToList();
            if(gameManager.Round.Pile.Count > 0 && playableCards.Where(c => c.Suit == gameManager.Round.Pile[0].Suit || c.Suit == gameManager.Round.Trump).Count() == 0)
            {
                //if you don't have cards that can follow the lead and you don't have trumps
                return GetLowestCard(playableCards);
                //return immediately the lowestCard without checking whether you can win with the TestMove method(because you can't win)
            }
            foreach (var card in playableCards)
                if (TestMove(card, referee) == gameManager.Round.CurrentPlayer.Number)
                    return card;
            return GetLowestCard(playableCards);
        }

        //returns the player that will win if this card is played
        public int TestMove(Card card, IReferee referee)
        {
            IList<Player> playersToGo = gameManager.Round.PlayersLeft;
            int[] HeuristicScores = new int[4];
            
            Suits suit = card.Suit;
            if (gameManager.Round.Pile.Count > 0)
                suit = gameManager.Round.Pile[0].Suit;

            Card compCard = (gameManager.Round.Pile.Count > 0) ? gameManager.Round.Pile[0] : card;
            foreach (var player in playersToGo) {
                var playableCards = player.hand.Cards.Where(x => referee.ValidateMove(x, compCard, player.hand.Cards.ToList())).ToList();
                HeuristicScores[player.Number-1] = GetHeuristicalHandChances(playableCards, suit);
            }

            if (gameManager.Round.Pile.Count > 0)
                HeuristicScores[gameManager.Round.PileOwner.Number-1] = GetHeuristicalHandChances(gameManager.Round.Pile, suit);

            HeuristicScores[gameManager.Round.CurrentPlayer.Number-1] = GetHeuristicalHandChances(new List<Card>() { card }, suit);


            return HeuristicScores.ToList().IndexOf(HeuristicScores.Max()) + 1;
        }

        private Card GetLowestCard(IEnumerable<Card> cards)
        {
            if (cards.Count() == 0)
                return null;
            Card minCard;
            IEnumerable<Card> trumps = cards.Where(c => c.Suit == gameManager.Round.Trump);
            if(trumps.Count() == cards.Count())
            {
                //if you only have trumps, you don't have to exlude the trumps
                minCard = cards.First();
                foreach (Card card in cards)
                {
                    if (card.Number < minCard.Number)
                        minCard = card;
                }
            }
            else
            {
                //else you have to exclude the trumps
                minCard = cards.Where(c => c.Suit != gameManager.Round.Trump).First();
                foreach (Card card in cards.Except(trumps))
                {
                    if (card.Number < minCard.Number)
                        minCard = card;
                }
            }
            return minCard;
        }

        public void ProcessOtherPlayerAction(Player otherPlayer, GameLogic.ControlEntities.Action action)
        {

        }

        public void ProcessOtherPlayerCard(Player otherPlayer, Card card)
        {

        }
    }
}
