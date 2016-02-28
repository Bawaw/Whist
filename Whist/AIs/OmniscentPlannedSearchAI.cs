using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.Datastructures;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    class PlaySearchAI
    {

        private GameManager gameManager;
        private SearchTree<Card> searchTree;
        private IReferee Referee;

        //input: Card you want to play, output: Heuristic value
        public int TestMove(Card Card, HandCollection[] cards, HandCollection PlayerCollection)
        {
            SearchTreeNode<Card> playedCards = new SearchTreeNode<Card>(Card);
            for (int i = 0; i < 13; i++)
            {
                foreach (var hand in cards)
                {
                    var validCards = ValidateCards(Card, hand.Cards);
                    
                    foreach (var card in validCards)
                    {
                        playedCards.AddChild(card);
                    }
                    
                }
            }

            return 0;
        }

        public IList<Card> ValidateCards(Card Lead, IList<Card> Cards)
        {
            if (Lead == null) return Cards;
            return Cards.Where(x => Referee.ValidateMove(x, Lead, Cards.ToList())).ToList();
        }

    }
}
