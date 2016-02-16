using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    public interface IDealCards
    {
        void DealCards(Player[] players);
    }

    public class DealCardsSimple : IDealCards
    {
        public void DealCards(Player[] players)
        {
            DeckCollection cardCollection = new DeckCollection();
            cardCollection.initialise();
            cardCollection.shuffle();
            int nCards = cardCollection.Count / players.Length;
            foreach (var player in players)
                player.hand.AddCards(cardCollection.Draw(nCards));
        }
    }
}
