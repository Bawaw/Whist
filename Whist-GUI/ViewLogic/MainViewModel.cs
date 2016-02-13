using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI.ViewLogic
{
    public class MainViewModel
    {
        private Round round;

        public HandViewModel HandVM { get; private set; }

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
            HandVM = new HandViewModel(round.Start());
        }
    }

    public class HandViewModel : INotifyPropertyChanged
    {
        private class PlayCommand : ICommand
        {
            HandViewModel handViewModel;

            public PlayCommand(HandViewModel handViewModel) {
                this.handViewModel = handViewModel;
            }

            public bool CanExecute(object parameter)
            {
                Card card = parameter as Card;
                if (card == null) return false;
                return handViewModel.whistController.IsValidPlay(card);
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                Card card = parameter as Card;
                if (card == null) return;
                handViewModel.playCard(card);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly PlayCommand playCmd;
        public ICommand PlayCmd { get { return playCmd; } }

        private IPlayTricks whistController;

        public IList<Card> PlayerCards { get { return whistController.GetPlayerCards(); } }

        public HandViewModel(IPlayTricks controller)
        {
            this.playCmd = new PlayCommand(this);
            this.whistController = controller;
        }

        public void playCard(Card card) {
            whistController.PlayCard(card);

            NotifyPropertyChanged("Hand");
        }

        public void NotifyPropertyChanged(String propName)
        {
            if (propName != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    } 
}
