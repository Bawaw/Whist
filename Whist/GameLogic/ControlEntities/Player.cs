using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    public class Player
    {
        public string name;
        private int tricks;
        public int Tricks { get { return tricks; } }
        public HandCollection hand;
        public int score;

        public Player(string name)
        {
            this.name = name;
            this.hand = new HandCollection();
        }

        public void addTrick() {
            ++tricks;
        }

        //reset tricks
        public void clearTricks() {
            tricks = 0;
        }

        public override string ToString()
        {
            return name;
        }

    }

    public class Team
    {
        private Player[] players;
        public Player[] Players { get { return players; }  }
        public int Tricks { get { return players.Sum(player => player.Tricks); } }
        public Team(Player[] players, int objective) {
            this.players = players;
            this.objective = objective;
        }
        public int objective;
        public void applyScore(int score)
        {
            foreach (Player player in players)
            {
                player.score += score;
            }
        }
    }
}
