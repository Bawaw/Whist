using System.Collections.Generic;
using System.Linq;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AI
{
    class SimpleGameAI
    {
        public Card GetMove(Player player, IList<Card> pile, Suits trump)
        {
            var cards = player.hand.Cards;
            var pileSuit = pile[0].Suit;
            if (cards.Any(c => c.Suit == pileSuit))//Hand contains card of same suit as pile.
            {
                var cardsOfSuit = cards.Where(c => c.Suit == pileSuit);
                var pileCardsOfSuit = pile.Where(c => c.Suit == pileSuit);
                return HighOrLowCardSelection(cardsOfSuit, pileCardsOfSuit);
            }
            else//Hand contains no card of the same suit as pile.
            {
                if (cards.Any(c => c.Suit == trump))//Hand has trump card.
                {
                    var cardsOfTrump = cards.Where(c => c.Suit == pileSuit);
                    var pileCardsOfTrump = pile.Where(c => c.Suit == pileSuit);
                    return HighOrLowCardSelection(cardsOfTrump, pileCardsOfTrump);
                }
                else //Hand contains neither pileSuit card of trump card.
                {
                    return GetLowestCard(cards);
                }
            }
        }

        private Card HighOrLowCardSelection(IEnumerable<Card> hand, IEnumerable<Card> pile)
        {
            var HighestPileCard = GetHighestCard(hand);
            var HighestHandCard = GetHighestCard(pile);
            if (HighestHandCard.Number > HighestPileCard.Number)//Player has higher card than current highest in pile.
            {
                return HighestHandCard;
            }
            else //player doesn't have a higher card than the current highest.
            {
                return GetLowestCard(hand);
            }
        }

        private Card GetHighestCard(IEnumerable<Card> cards)
        {
            if (cards.Count() == 0)
                return null;
            Card maxCard = cards.First();
            foreach (Card card in cards)
            {
                if (card.Number > maxCard.Number)
                    maxCard = card;
            }
            return maxCard;
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
    }
}
