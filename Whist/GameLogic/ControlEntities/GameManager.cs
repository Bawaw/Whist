﻿using System;
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
                new Player("Player",1),
                new Player("Comp 1",2),
                new Player("Comp 2",3),
                new Player("Comp 3",4)
            };
            HumanPlayer = players[0];

            RoundsToPlay = 13;
            RoundNumber = 1;
            IsGameInProgress = true;
            Round = new Round(Players);
            aiPlayers = new Dictionary<Player, AI>();
            foreach(Player player in NonHumanPlayers)
                aiPlayers.Add(player, AIFactory.CreateAI(player, this, AIBidType.BASIC, AIGameType.MEMORY));
            }

        }

        public GameManager(Player[] players, AIBidType[] bidAITypes, AIGameType[] gameAITypes)
        {
            this.players = players;

        aiPlayers = new Dictionary<Player, AI>();
        for (int i=0; i<4; i++)
            aiPlayers.Add(players[i], AIFactory.CreateAI(players[i], this, bidAITypes[i], gameAITypes[i]));
    }
    RoundsToPlay = 13;
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
            get { return Players.Except(new Player[] { HumanPlayer }); }
        }

        public AI GetAI(Player player)
        {
            return aiPlayers[player];
        }

        public bool IsRoundInProgress
        {
            get { if (Round == null) return false; else return Round.RoundInProgress; }
        }

        public void StartNewRound()
        {
            if (!IsRoundInProgress && IsGameInProgress)
            {
                if (RoundNumber < RoundsToPlay)
            {
                CyclePlayers();
                RoundNumber++;
                Round = new Round(Players);
            }
                else
                {
                    IsGameInProgress = false;
                }
            }
        }

        public bool IsGameInProgress { get; private set; }

        private void CyclePlayers()
        {
            var temp = players[0];
            for (int i = 1; i < players.Length; i++)
            {
                Players[i - 1] = Players[i];
            }
            Players[Players.Length - 1] = temp;
        }
    }
}
