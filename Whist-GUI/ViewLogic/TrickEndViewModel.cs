using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI.ViewLogic
{
    public class TrickEndViewModel : INotifyPropertyChanged
    {
        public string Winner { get; set; }

        public string Visibility { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public TrickEndViewModel()
        {
            Winner = "";
            Visibility = "Hidden";
        }
        
        public void PropChanged()
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Winner"));
                PropertyChanged(this, new PropertyChangedEventArgs("Visibility"));
            }
        }
    }
}
