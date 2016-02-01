using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    public interface IReferee {
        bool ValidateMove(Card card, Card lead, List<Card> currentPlayerCards);
    }
    public class StandardReferee : IReferee
    {
        public bool ValidateMove(Card card,Card lead, List<Card> currentPlayerCards)
        {
            //if did not play same suit
            if (card.Suit != lead.Suit) {
                int index = currentPlayerCards.FindIndex(x => x.Suit == lead.Suit);

                //and hand still contains card with valid suit => invalid move
                if (index >= 0)
                    return false;
            }
            return true;
        }
    }
}
