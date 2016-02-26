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
            HumanPlayer = Players[0];
            RoundNumber = 1;
            Round = new Round(Players);
            aiPlayers = new Dictionary<Player, AI>();
            foreach(Player player in NonHumanPlayers)
                aiPlayers.Add(player, AIFactory.CreateAI(player, this, AIBidType.BASIC, AIGameType.MEMORY));
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

        public int RoundNumber
        {
            get; private set;
        }

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
            if (!IsRoundInProgress)
            {
                CyclePlayers();
                RoundNumber++;
                Round = new Round(Players);
            }
        }

        private void CyclePlayers()
        {
            var temp = Players[0];
            for (int i=1; i<Players.Length; i++)
            {
                Players[i - 1] = Players[i];
            }
            Players[Players.Length - 1] = temp;
        }
    }
}
