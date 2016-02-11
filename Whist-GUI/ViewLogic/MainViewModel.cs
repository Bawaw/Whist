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

        public IList<Card> GetPlayerCards { get {return round.GetPlayerCards(); } }

        public MainViewModel() {
            round = new Round();
        }

    }
}
