using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    public interface IDealer{
        Card Deal(Player[] players, DeckCollection cardCollection);
    }
    public class StandardDealer : IDealer
    {
        //shuffles cards and gives each member equal amount of cards, returns first card
        public Card Deal(Player[] players, DeckCollection cardCollection) {
            cardCollection.initialise();
            cardCollection.shuffle();
            Card firstCard = cardCollection.peep();
            int nCards = cardCollection.Count / players.Length;
            foreach (var player in players)
                player.hand.AddCards(cardCollection.Draw(nCards));
            return firstCard;
        }
    }
}
