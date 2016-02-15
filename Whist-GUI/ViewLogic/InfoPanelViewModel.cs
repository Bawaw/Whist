using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI
{
    public class InfoPanelViewModel : INotifyPropertyChanged
    {
        Team[] teams;

        public event PropertyChangedEventHandler PropertyChanged;

        public InfoPanelViewModel(Team[] teams, Player player, string gameCase)
        {
            this.teams = teams;
            Player = player;
            GameCase = gameCase;
        }

        public void propChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Player"));
                PropertyChanged(this, new PropertyChangedEventArgs("Teams"));
                PropertyChanged(this, new PropertyChangedEventArgs("TricksLeft"));
                PropertyChanged(this, new PropertyChangedEventArgs("TricksWon"));
                PropertyChanged(this, new PropertyChangedEventArgs("GameCase"));
            }
        }

        public Player Player
        {
            get; private set;
        }
        public int TricksLeft
        {
            get { return Player.hand.Count; }
        }
        public int TricksWon
        {
            get { return Player.Tricks; }
        }
        public string GameCase
        {
            get; private set;
        }
        public string Teams
        {
            get
            {
                string str = "";
                foreach (Team team in teams)
                {
                    str += "\n-";
                    foreach (Player p in team.Players)
                    {
                        str += "[" + p.name + "(" + p.score + ")] ";
                    }
                    str += "(" + team.Tricks + "/" + team.objective + ")";
                }
                return str;
            }
        }
    }
}
