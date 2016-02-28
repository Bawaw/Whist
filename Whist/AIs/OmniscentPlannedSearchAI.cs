using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Whist.Datastructures;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    class PlaySearchAI
    {
        private IReferee Referee;


        public PlaySearchAI(IReferee referee) {
            this.Referee = referee;
        }

        //input: Card you want to play, output: Heuristic value
        public int TestMove(Card Card, HandCollection[] Cards, int StartPlayer)
        {
            var branch = BranchPlayer(Card, Cards, StartPlayer);
            /*int rounds = cards[++StartPlayer].Count;

            SearchTreeNode<Card> baseTree = new SearchTreeNode<Card>(Card);
            SearchTreeNode<Card> branch = new SearchTreeNode<Card>(Card);

            for (int i = 0; i < rounds; i++)
            {
                //for all hands that have not yet played this round

                foreach (var hand in cards.Where(x => x.Cards.Count == rounds)) {

                    var PossiblePlays = new SearchTreeNode<Card>(Card);

                    foreach (var card in hand.Cards)
                    {
                        PossiblePlays.AddChild(card);
                    }
                    branch.AddChild(PossiblePlays);
                    baseTree.AddChild(branch);
                    branch = PossiblePlays;
                }
            }*/

            return 0;
        }

        public SearchTreeNode<Card> BranchPlayer(Card card, HandCollection[] cards, int startPlayer) {

            cards[startPlayer].Play(card);

            SearchTreeNode<Card> playerBranch = new SearchTreeNode<Card>(card);
            HandCollection[] copyCards = Shorts.DeepClone(cards); 

            foreach (var child in BranchPlay(card, copyCards[nextPlayer(startPlayer)]).Children)
                playerBranch.AddChild(BranchPlayer(child.Data, copyCards, nextPlayer(startPlayer)));

            return playerBranch;
        }

        public SearchTreeNode<Card> BranchPlay(Card lead, HandCollection hand) {
            var playableHand = ValidateCards(lead, hand.Cards);
            SearchTreeNode<Card> playableCards = new SearchTreeNode<Card>(lead);

            foreach (var card in playableHand)
                playableCards.AddChild(hand.Play(card));
            
            return playableCards;
        }

        protected int nextPlayer(int currentPlayer)
        {
            if (currentPlayer + 1 < 4)
                return ++currentPlayer;
            else
                return 0;
        }


        public IList<Card> ValidateCards(Card Lead, IList<Card> Cards)
        {
            if (Lead == null) return Cards;
            return Cards.Where(x => Referee.ValidateMove(x, Lead, Cards.ToList())).ToList();
        }

    }
}
