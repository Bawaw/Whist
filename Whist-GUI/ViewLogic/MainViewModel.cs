using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI.ViewLogic
{
    class MainViewModel
    {
        private Round round;

        public IList<Card> GetPlayerCards { get { return round.GetPlayerCards(); } }

        public MainViewModel()
        {
            Player[] players = new Player[]
            {
                new Player("P1"),
                new Player("P2"),
                new Player("P3"),
                new Player("P4")
            };
            round = new Round(players);
        }

    }
}
