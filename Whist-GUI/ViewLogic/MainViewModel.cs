using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Whist.AI;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI.ViewLogic
{
    public class BaseGameViewModel : INotifyPropertyChanged
    {
        private Round round;
        private InfoPanelViewModel infoPanelVM;

        public ObservableCollection<Card> Pile { get { return whistController.Pile; } }
        public HandViewModel HandVM { get; private set; }
        public Suits Trump { get { return round.Trump; } }

        private IPlayTricks whistController;

        public event PropertyChangedEventHandler PropertyChanged;

        public BaseGameViewModel(Round round, InfoPanelViewModel infoPanelVM)
        {
            this.round = round;
            this.infoPanelVM = infoPanelVM;
            HandVM = new HandViewModel(this);
            whistController = round.Start();
        }

        public void PlayCard(Card card) {

            whistController.PlayCard(card);
            if (!round.TrickInProgress)
            {
                //MessageBoxResult result = MessageBox.Show(round.PileOwner.name + " won the trick", "Trick End", MessageBoxButton.OK, MessageBoxImage.None);
                round.EndTrick();
            }


            var AI = new SimpleGameAI();
            while (round.CurrentPlayer != null && round.CurrentPlayer != round.Players[0])
            {
                while (round.TrickInProgress && round.CurrentPlayer != null && round.CurrentPlayer != round.Players[0])
                {
                    var aiCard = AI.GetMove(round.CurrentPlayer, round.Pile, round.Trump);
                    round.PlayCard(aiCard);
                }
                if (!round.TrickInProgress)
                {
                    //MessageBoxResult result = MessageBox.Show(round.PileOwner.name + " won the trick", "Trick End", MessageBoxButton.OK, MessageBoxImage.None);
                    round.EndTrick();
                }
            }

            if (!round.InTrickPhase)
            {
                round.EndTricksRound();
                string str = "";
                foreach (var player in round.Players)
                    str += player.name + " (" + player.Tricks + ") - " + player.score + "\n";
                
                MessageBoxResult result = MessageBox.Show(str, "Round End", MessageBoxButton.OK, MessageBoxImage.None);
                StartNewRound();
            }
            infoPanelVM.propChanged();
        }

        public bool IsValidPlay(Card card) {
            return whistController.IsValidPlay(card);
        }

        public ObservableCollection<Card> GetCurrentPlayerCards() {
            return whistController.GetPlayerCards();
        }

        public void StartNewRound()
        {

            round = new Round(round.Players);
            whistController = round.Start();
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Pile"));
                PropertyChanged(this, new PropertyChangedEventArgs("HandVM"));
                PropertyChanged(this, new PropertyChangedEventArgs("Trump"));
                PropertyChanged(this, new PropertyChangedEventArgs("Round"));
                PropertyChanged(this, new PropertyChangedEventArgs("infoPanelVM"));
            }
        }
    }

    public class HandViewModel
    {
        private readonly PlayCommand playCmd;
        public ICommand PlayCmd { get { return playCmd; } }

        private BaseGameViewModel mainViewModel;

        public ObservableCollection<Card> PlayerCards { get { return mainViewModel.GetCurrentPlayerCards(); } }

        public HandViewModel(BaseGameViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.playCmd = new PlayCommand(this);
        }

        public void playCard(Card card) {
            mainViewModel.PlayCard(card);
        }


        private class PlayCommand : ICommand
        {
            HandViewModel handViewModel;

            public PlayCommand(HandViewModel handViewModel)
            {
                this.handViewModel = handViewModel;
            }

            public bool CanExecute(object parameter)
            {
                Card card = parameter as Card;
                if (card == null) return false;
                return handViewModel.mainViewModel.IsValidPlay(card);
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                Card card = parameter as Card;
                if (card == null) return;
                handViewModel.playCard(card);
            }
        }

    }
}
