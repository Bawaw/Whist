using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    public class GameManager
    {
        Player[] players;

        public GameManager()
        {
            players = new Player[]
            {
                new Player("Player"),
                new Player("Comp 1"),
                new Player("Comp 2"),
                new Player("Comp 3")
            };
            HumanPlayer = players[0];
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

        public bool IsRoundInProgress
        {
            get { if (Round == null) return false; else return Round.RoundInProgress; }
        }

        public void StartNewRound()
        {
            if (!IsRoundInProgress)
            {
                CyclePlayers();
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
