using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.AIs;

namespace Whist.GameLogic.ControlEntities
{
    public class Round
    {
        private IDealingAndBidding phase1;
        private IPlayTricks phase2;
        private IScoreCalculation phase3;

        private Team[] teams;


        public Player[] Players { get; private set; }
        public Suits Trump { get { return phase1.Trump; } }
        public bool RoundInProgress { get; private set; }
        public Team[] Teams { get { return teams; } }

        public Round(Player[] players)
        {
            Players = players;
            foreach (Player player in Players)
                player.clearTricks();
            phase1 = new DealAndBidNormal(Players);
            RoundInProgress = true;

            //LetAIHandleFirstPhase();
        }

        /// <summary>
        /// Once the bidding is over, this function ends the bidding phase and starts the tricks phase.
        /// </summary>
        public void EndBiddingRound()
        {
            if (!phase1.InBiddingPhase && phase2 == null)
            {
                var result = phase1.FinalizeBidding();
                teams = result.teams;
                phase2 = new WhistController(Players, result.firstPlayer, Trump, new StandardReferee());
            }
        }

        /// <summary>
        /// Once the tricks phase is over, this function calculates and applies the scores and ends the round
        /// </summary>
        public void EndTricksRound()
        {
            if (!phase2.InTrickPhase)
            {
                phase3 = new BasicScoreMechanism();
                phase3.CalculateScores(teams, GameCase);
                RoundInProgress = false;
            }
        }

        public IPlayTricks Start()
        {
            return phase2;
        }

        /// <summary>
        /// The GameCase that was chosen in the bidding phase.
        /// If the bidding phase isn't over yet, it will be on Undecided.
        /// </summary>
        public Case GameCase { get { return phase1.GameCase; } }
        /// <summary>
        /// Submits the selected action (<seealso cref="BiddingGetPossibleActions"/> for possible choices). If succesfull the <seealso cref="CurrentPlayer"/> is updated automatically.
        /// If this was the last action of the bidding phase <seealso cref="InBiddingPhase"/> will be updated to return false.
        /// </summary>
        /// <param name="action">action to submit</param>
        /// <returns>true if the submitted action is accepted</returns>
        public bool BiddingDoAction(Action action) { if (!phase1.InBiddingPhase) return false; return phase1.DoAction(action); }
        /// <summary>
        /// Retrieves all possible choices that can be made in the bidding phase.
        /// One of them can be chosen and submitted via <seealso cref="BiddingDoAction"/>.
        /// </summary>
        /// <returns>List of possible actions</returns>
        public IEnumerable<Action> BiddingGetPossibleActions() { if(!phase1.InBiddingPhase) return null; return phase1.GetPossibleActions(); }
        public bool InBiddingPhase { get { return phase1.InBiddingPhase; } }

        public Player CurrentPlayer
        {
            get
            {
                if (phase1.InBiddingPhase) return phase1.CurrentPlayer;
                if (phase2 == null) return null;
                if (phase2.InTrickPhase) return phase2.CurrentPlayer;
                return null;
            }
        }

        /// <summary>
        /// Shows whether a trick is currently in progress or not.
        /// Not to be confused with <seealso cref="InTrickPhase"/>, which shows whether the trick phase is currently in progress.
        /// </summary>
        public bool TrickInProgress { get { if (phase2 == null) return true; else return phase2.TrickInProgress; } }
        /// <summary>
        /// Returns the player who is currently winning the trick.
        /// </summary>
        public Player PileOwner { get { return phase2?.PileOwner; } }
        /// <summary>
        /// The Pile is the cards in the current trick.
        /// </summary>
        public IList<Card> Pile { get { return phase2?.Pile; } }
        /// <summary>
        /// Shows whether the trick phase is currently in progress or not.
        /// Not to be confused with <seealso cref="TrickInProgress"/>, which is about the current trick, and not the entire phase.
        /// </summary>
        public bool InTrickPhase { get { if (phase2 == null) return false; else return phase2.InTrickPhase; } }
        /// <summary>
        /// Returns a list of players who still have to play a card in the current trick.
        /// </summary>
        public IList<Player> PlayersLeft { get { return phase2.PlayersLeft; } }
        /// <summary>
        /// When a trick has ended (see <seealso cref="TrickInProgress"/>), the trick needs to be ended for the new trick to begin.
        /// </summary>
        /// <returns>Winning player</returns>
        public Player EndTrick() { return phase2?.EndTrick(); }
        /// <summary>
        /// Returns whether or not it is legal to play a certain card.
        /// </summary>
        public bool IsValidPlay(Card card) { if (phase2 == null) return false; return phase2.IsValidPlay(card); }
        /// <summary>
        /// Plays a card on the pile. If accepted the <seealso cref="CurrentPlayer"/> will be changed automatically.
        /// </summary>
        public bool PlayCard(Card card) { if (phase2 == null) return false; else return phase2.PlayCard(card); }
        /// <summary>
        /// Returns a list of the cards in a player's hand.
        /// </summary>
        /// <returns>Player's hand</returns>
        public IList<Card> GetPlayerCards(Player player) { return phase2?.GetPlayerCards(player); }
        /// <summary>
        /// Returns the current player's hand.
        /// </summary>
        public IList<Card> GetPlayerCards() { return phase2?.GetPlayerCards(); }
        /// <summary>
        /// In the trick phase, returns which card was played by a certain player.
        /// </summary>
        public Card CardPlayedByPlayer(Player player) { return phase2?.CardPlayedByPlayer[player]; }
        /// <summary>
        /// In the trick phase, returns which player played a certain card.
        /// </summary>
        public Player PlayerWhoPlayedCard(Card card) { foreach (var i in phase2.CardPlayedByPlayer) { if (i.Value == card) return i.Key; } return null; }

    }

}
