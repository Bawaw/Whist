﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    class Round
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

        public Round()
        {
            phase1 = new DealAndBidNormal(Players);
            while (phase1.InBiddingPhase)
            {
                var possibleActions = phase1.GetPossibleActions();
                phase1.DoAction(possibleActions.First());
            }
            //Biding ronde voorbij
            phase2 = new WhistController(Players, Trump, new StandardReferee());
            while (phase2.InTrickPhase)
            {
                while (!phase2.HasTrickEnded)
                {
                    phase2.PlayCard(null);
                }
                phase2.EndTrick();
            }
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
        Player PileOwner { get { return phase2.PileOwner; } }
        Player CurrentPlayer { get { return phase2.CurrentPlayer; } }
        List<Card> Pile { get { return phase2.Pile; } }
        bool InTrickPhase { get { return phase2.InTrickPhase; } }

        Player EndTrick() { return phase2.EndTrick(); }
        bool PlayCard(Card card) { return phase2.PlayCard(card); }
        IList<Card> GetPlayerCards(Player player) { return phase2.GetPlayerCards(player); }
        IList<Card> GetPlayerCards() { return phase2.GetPlayerCards(); }

    }

}
