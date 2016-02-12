using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    public class Round
    {
        private IDealingAndBidding phase1;
        private IPlayTricks phase2;
        private IScoreCalculation phase3;

        protected Team[] teams;
        public Player[] Players
        {
            get;
            private set;
        }

        public Case GameCase
        {
            get { return phase1.GameCase; }
        }

        public Suits Trump
        {
            get; internal set;
        }

        public Round(Player[] players)
        {
            Players = players;
            phase1 = new DealAndBidNormal(Players);
            Trump = phase1.Trump;
            /*while (phase1.InBiddingPhase)
            {
                var possibleActions = phase1.GetPossibleActions();
                phase1.DoAction(possibleActions.First());
            }*/

            //play game testing phase
            phase2 = new WhistController(Players, Trump, new StandardReferee());


            /*
            //Biding ronde voorbij
            while (phase2.InTrickPhase)
            {
                while (!phase2.HasTrickEnded)
                {
                    phase2.PlayCard(null);
                }
                phase2.EndTrick();
            }*/
        }

        public IPlayTricks Start() {
            return phase2;
        }

        public bool BiddingDoAction(Action action)
        {
            return phase1.DoAction(action);
        }
        public IEnumerable<Action> BiddingGetPossibleActions()
        {
            return phase1.GetPossibleActions();
        }


        public bool HasTrickEnded { get { return phase2.HasTrickEnded; } }
        public Player PileOwner { get { return phase2.PileOwner; } }
        public Player CurrentPlayer { get { return phase2.CurrentPlayer; } }
        public IList<Card> Pile { get { return phase2.Pile; } }
        public bool InTrickPhase { get { return phase2.InTrickPhase; } }

        public Player EndTrick() { return phase2.EndTrick(); }
        public bool PlayCard(Card card) { return phase2.PlayCard(card); }
        public IList<Card> GetPlayerCards(Player player) { return phase2.GetPlayerCards(player); }
        public IList<Card> GetPlayerCards() { return phase2.GetPlayerCards(); }

    }

}
