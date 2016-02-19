using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI.ViewLogic
{
    public class TrickEndViewModel
    {
        public string Winner { get; set; }

        public TrickEndViewModel()
        {
            Winner = "";
        }
    }
}
