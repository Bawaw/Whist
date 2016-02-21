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
        Player[] players;
        Dictionary<Player, AI> aiPlayers;

        public GameManager()
        {
            players = new Player[]
            {
                new Player("Player",1),
                new Player("Comp 1",2),
                new Player("Comp 2",3),
                new Player("Comp 3",4)
            };
            HumanPlayer = players[0];
            RoundNumber = 1;
            Round = new Round(players);
            aiPlayers = new Dictionary<Player, AI>();
            foreach(Player player in NonHumanPlayers)
                aiPlayers.Add(player, AIFactory.CreateAI(player, this, AIType.Basic));
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
            get { return players.Except(new Player[] { HumanPlayer }); }
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
                Round = new Round(players);
            }
        }

        private void CyclePlayers()
        {
            var temp = players[0];
            for (int i=1; i<players.Length; i++)
            {
                players[i - 1] = players[i];
            }
            players[players.Length - 1] = temp;
        }
    }
}
