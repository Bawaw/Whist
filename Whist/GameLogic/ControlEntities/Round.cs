using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    class Round
    {
        protected Team[] teams;
        protected DeckCollection deck;
        internal Player[] players;
        protected List<Card> pile;

        internal Case gameCase;
        private DealAndBidNormal phase1;
        public Suits Trump
        {
            get; internal set;
        }

        public Round()
        {
            phase1 = new DealAndBidNormal(this);
            while (phase1.InBiddingPhase)
            {
                var possibleActions = phase1.GetPossibleActions();
                phase1.DoAction(possibleActions.First());
            }
        }
    }

    public enum Case
    {
        FFA,
        TEAM,
        ALONE,
        TROEL,
        ABONDANCE,
        MISERIE,
        SOLO,
        SOLOSLIM
    }
}
