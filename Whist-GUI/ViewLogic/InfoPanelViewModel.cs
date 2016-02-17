using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI
{
    public class InfoPanelViewModel : INotifyPropertyChanged
    {
        private Round round;
        public event PropertyChangedEventHandler PropertyChanged;

        public InfoPanelViewModel(Round round, Player player)
        {
            this.round = round;
            Player = player;
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
                PropertyChanged(this, new PropertyChangedEventArgs("Trump"));
            }
        }

        public Player Player
        {
            get; private set;
        }
        public int TricksLeft
        {
            get { if (Player == null) return -1; return Player.hand.Count; }
        }
        public int TricksWon
        {
            get { if (Player == null) return -1; return Player.Tricks; }
        }
        public Suits? Trump { get { return round?.Trump; } }
        public Case? GameCase
        {
            get { return round?.GameCase; }
        }
        public string Teams
        {
            get
            {
                string str = "";
                Team[] teams = round?.Teams;
                if (teams == null)
                {
                    foreach (Player p in round.Players)
                    {
                        str += "\n-" + p.name + " (" + p.score + ") ";
                    }
                }
                else
                {
                    foreach (Team team in teams)
                    {
                        str += "\n-";
                        foreach (Player p in team.Players)
                        {
                            str += "[" + p.name + "(" + p.score + ")] ";
                        }
                        str += "(" + team.Tricks + "/" + team.objective + ")";
                    }
                }
                return str;
            }
        }
    }
}
