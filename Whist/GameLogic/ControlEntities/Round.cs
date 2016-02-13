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

        private Team[] teams;


        public Player[] Players { get; private set; }
        public Suits Trump { get; private set; }
        public bool RoundInProgress { get; private set; }


        public Round(Player[] players)
        {
            Players = players;
            phase1 = new DealAndBidNormal(Players);
            Trump = phase1.Trump;
            RoundInProgress = true;
        }



        public void EndBiddingRound()
        {
            if (!phase1.InBiddingPhase && phase2 == null)
            {
                var result = phase1.FinalizeBidding();
                teams = result.teams;
                Trump = result.trump;
                phase2 = new WhistController(Players, result.firstPlayer, Trump, new StandardReferee());
            }
        }

        public void EndTricksRound()
        {
            if (!phase2.InTrickPhase)
            {
                phase3 = new SimpleScoreMechanisme();
                phase3.CalculateScores(teams, GameCase);
                RoundInProgress = false;
            }
        }

        public IPlayTricks Start()
        {
            return phase2;
        }

        public Case GameCase { get { return phase1.GameCase; } }
        public bool BiddingDoAction(Action action) { if (!phase1.InBiddingPhase) return false; return phase1.DoAction(action); }
        public IEnumerable<Action> BiddingGetPossibleActions() { if(!phase1.InBiddingPhase) return null; return phase1.GetPossibleActions(); }


        public Player CurrentPlayer
        {
            get
            {
                if (phase1.InBiddingPhase) return phase1.CurrentPlayer;
                if (phase2.InTrickPhase) return phase2.CurrentPlayer;
                return null;
            }
        }

        public bool TrickInProgress { get { if (phase2 == null) return true; else return phase2.TrickInProgress; } }
        public Player PileOwner { get { return phase2?.PileOwner; } }
        public IList<Card> Pile { get { return phase2?.Pile; } }
        public bool InTrickPhase { get { if (phase2 == null) return false; else return phase2.InTrickPhase; } }

        public Player EndTrick() { return phase2?.EndTrick(); }
        public bool PlayCard(Card card) { if (phase2 == null) return false; else return phase2.PlayCard(card); }
        public IList<Card> GetPlayerCards(Player player) { return phase2?.GetPlayerCards(player); }
        public IList<Card> GetPlayerCards() { return phase2?.GetPlayerCards(); }

    }

}
