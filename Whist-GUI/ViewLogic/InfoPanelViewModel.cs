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
        private GameManager gameManager;
        public event PropertyChangedEventHandler PropertyChanged;
        private List<string> actionLog;

        public InfoPanelViewModel(GameManager gameManager)
        {
            this.gameManager = gameManager;
            Player = gameManager.HumanPlayer;
            actionLog = new List<string>();
        }

        public void PropChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Player"));
                PropertyChanged(this, new PropertyChangedEventArgs("Teams"));
                PropertyChanged(this, new PropertyChangedEventArgs("TricksLeft"));
                PropertyChanged(this, new PropertyChangedEventArgs("TricksWon"));
                PropertyChanged(this, new PropertyChangedEventArgs("GameCase"));
                PropertyChanged(this, new PropertyChangedEventArgs("Trump"));
                PropertyChanged(this, new PropertyChangedEventArgs("Round"));
                PropertyChanged(this, new PropertyChangedEventArgs("ActionLog"));
                PropertyChanged(this, new PropertyChangedEventArgs("RoundsToPlay"));
                PropertyChanged(this, new PropertyChangedEventArgs("RoundNumber"));
            }
        }

        public void AddLineToActionLog(string line)
        {
            actionLog.Add(line);
            if (actionLog.Count > 8)
                actionLog.RemoveAt(0);
            PropertyChanged(this, new PropertyChangedEventArgs("ActionLog"));
        }
        public void ClearActionLog()
        {
            actionLog.Clear();
            PropertyChanged(this, new PropertyChangedEventArgs("ActionLog"));
        }

        private Round Round
        {
            get { return gameManager?.Round; }
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
        public Suits? Trump { get { return Round?.Trump; } }
        public Case? GameCase
        {
            get { return Round?.GameCase; }
        }
        public string Teams
        {
            get
            {
                string str = "";
                Team[] teams = Round?.Teams;
                if (teams == null || !gameManager.IsGameInProgress)
                {
                    str += "Players:";
                    foreach (Player p in Round.Players)
                    {
                        str += "\n-" + p.name + " (" + p.score + ") ";
                    }
                }
                else
                {
                    str += "Teams:";
                    foreach (Team team in teams)
                    {
                        str += "\n-";
                        for (int i=0; i<team.Players.Length-1; i++)
                        {
                            str += team.Players[i].name + "(" + team.Players[i].score + ") + ";
                        }
                        str += team.Players[team.Players.Length-1].name + "(" + team.Players[team.Players.Length - 1].score + ")";
                        str += " - (" + team.Tricks + "/" + team.objective + ")";
                    }
                }
                return str;
            }
        }
        public string ActionLog
        {
            get
            {
                string result = "";
                foreach (string str in actionLog)
                {
                    result += str + "\n";
                }
                return result;
            }
        }
        public int RoundsToPlay { get { return gameManager.RoundsToPlay; } }
        public int RoundNumber { get { return gameManager.RoundNumber; } }
    }
}
