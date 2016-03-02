using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.AIs;

namespace Whist.GameLogic.ControlEntities
{
    public class GameManager
    {
        public Player[] Players
        {
            get; private set;
        }
        Dictionary<Player, AI> aiPlayers;


        public GameManager()
        {
            Players = new Player[]
            {
                new Player("Player",0),
                new Player("Comp 1",1),
                new Player("Comp 2",2),
                new Player("Comp 3",3)
            };
            HumanPlayer = Players[0];

            RoundsToPlay = 13;
            RoundNumber = 1;
            IsGameInProgress = true;
            Round = new Round(Players);
            aiPlayers = new Dictionary<Player, AI>();
            foreach (Player player in Players.Except(new Player[] { HumanPlayer }))
                aiPlayers.Add(player, AIFactory.CreateAI(player, this, AIBidType.OMNISCIENT, AIGameType.OMNISCIENT));
            /*
            aiPlayers.Add(Players[1], AIFactory.CreateAI(Players[1], this, AIBidType.BASIC, AIGameType.BRUTEFORCE));
            aiPlayers.Add(Players[2], AIFactory.CreateAI(Players[2], this, AIBidType.BASIC, AIGameType.MEMORY));
            aiPlayers.Add(Players[3], AIFactory.CreateAI(Players[3], this, AIBidType.BASIC, AIGameType.MEMORY));*/
        }


        public GameManager(Player[] players, int roundsToPlay, AIBidType[] bidAITypes, AIGameType[] gameAITypes)
        {
            Players = players;

            aiPlayers = new Dictionary<Player, AI>();
            for (int i = 0; i < 4; i++)
            {
                aiPlayers.Add(players[i], AIFactory.CreateAI(players[i], this, bidAITypes[i], gameAITypes[i]));
            }
            RoundsToPlay = roundsToPlay;
            RoundNumber = 1;
            IsGameInProgress = true;
            Round = new Round(players);
        }

        public Player HumanPlayer
        {
            get;
            private set;
        }

        public Round Round
        {
            get; private set;
        }

        public int RoundsToPlay { get; private set; }
        public int RoundNumber { get; private set; }

        public IEnumerable<Player> NonHumanPlayers
        {
            get { return aiPlayers.Keys; }
        }

        /// <summary>
        /// Returns the AI corresponding to the given Player.
        /// </summary>
        public AI GetAI(Player player)
        {
            return aiPlayers[player];
        }

        /// <summary>
        /// Shows whether the current round is in progress. 
        /// Not to be confused with <seealso cref="IsGameInProgress"/>, which is about the entire game.
        /// </summary>
        public bool IsRoundInProgress
        {
            get { if (Round == null) return false; else return Round.RoundInProgress; }
        }

        /// <summary>
        /// To be called after ending a round, to start the next one.
        /// When initialized the GameManager starts a first round automatically and this function shouldn't be called.
        /// </summary>
        public void StartNewRound()
        {
            if (!IsRoundInProgress && IsGameInProgress)
            {
                if (RoundNumber < RoundsToPlay)
                {
                    CyclePlayers();
                    RoundNumber++;
                    foreach (AI ai in aiPlayers.Values)
                        ai.ResetMemory();
                    Round = new Round(Players);
                }
                else
                {
                    IsGameInProgress = false;
                }
            }
        }

        /// <summary>
        /// Shows whether the game (consisting of multiple rounds) is in progress. 
        /// Not to be confused with <seealso cref="IsRoundInProgress"/>, which is about a single round.
        /// </summary>
        public bool IsGameInProgress { get; private set; }

        private void CyclePlayers()
        {
            var temp = Players[0];
            for (int i = 1; i < Players.Length; i++)
            {
                Players[i - 1] = Players[i];
            }
            Players[Players.Length - 1] = temp;
        }
    }
}
