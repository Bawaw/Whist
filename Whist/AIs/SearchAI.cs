using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    class OmniscentSearchAI
    {
        /*private Round round;

        public OmniscentSearchAI(Round round)
        {
            this.round = round;
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
                if (highestCard.Suit == card.Suit || highestCard.Suit == round.Trump && (int)card.Number > (int)highestCard.Number)
                {
                    highestCard = card;
                    heuristicScore = (int)card.Number;
                }

            if (highestCard.Suit == round.Trump)
                heuristicScore += 10;

            //trump 2 is value 15, ace lead is value 14, not of type lead = 0;
            return (highestCard.Suit == round.Trump) ? (int)highestCard.Number : (int)highestCard.Number + 13;
        }

        private Team CurrentTeam()
        {
            return round.Teams.Where(t => t.Players.Any(p => p == round.CurrentPlayer)).Single();
        }

        public Card GetMove(IReferee referee)
        {
            var playableCards = round.CurrentPlayer.hand.Cards.Where(x => referee.ValidateMove(x, round.Pile[0], round.CurrentPlayer.hand.Cards.ToList())).ToList();
            foreach (var card in playableCards)
                if (TestMove(card, referee) == round.CurrentPlayer.Number)
                    return card;
            return GetLowestCard(round.CurrentPlayer.hand.Cards);
        }

        //returns the player that will win if this card is played
        public int TestMove(Card card, IReferee referee)
        {
            IList<Player> playersToGo = round.PlayersLeft;
            int[] HeuristicScores = new int[4];

            Suits suit = card.Suit;
            if (round.Pile.Count > 0)
                suit = round.Pile[0].Suit;

            foreach (var player in playersToGo) {
                var playableCards = player.hand.Cards.Where(x => referee.ValidateMove(x, round.Pile[0], player.hand.Cards.ToList())).ToList();
                HeuristicScores[player.Number] = GetHeuristicalHandChances(playableCards, suit);
            }

            if (round.Pile.Count > 0)
                HeuristicScores[round.PileOwner.Number] = GetHeuristicalHandChances(round.Pile, suit);

            HeuristicScores[round.CurrentPlayer.Number] = GetHeuristicalHandChances(new List<Card>() { card }, suit);


            return HeuristicScores.ToList().IndexOf(HeuristicScores.Max());
        }

        private Card GetLowestCard(IEnumerable<Card> cards)
        {
            if (cards.Count() == 0)
                return null;
            Card minCard = cards.First();
            foreach (Card card in cards)
            {
                if (card.Number < minCard.Number)
                    minCard = card;
            }
            return minCard;
        }
        */

    }
}
