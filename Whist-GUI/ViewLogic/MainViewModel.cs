
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
    public enum GameState { BIDDING, PLAYING }
    public class BaseGameViewModel : INotifyPropertyChanged
    {
        private GameState currentGameState;
        public GameState CurrentGameState { get { return currentGameState; }
            private set {
                if (value != currentGameState)
                {
                    currentGameState = value;
                    if(GameStateChanged != null)
                        GameStateChanged(value);
                }
        } }

        private Round round;
        private InfoPanelViewModel infoPanelVM;

        public ObservableCollection<Card> Pile { get { return whistController?.Pile; } }
        public HandViewModel HandVM { get; private set; }
        public Suits Trump { get { return round.Trump; } }

        private IPlayTricks whistController;

        public ObservableCollection<Card> Comp1Cards { get { return round.Players.Where(p => p.name == "Comp 1").Single().hand.Cards; } }
        public ObservableCollection<Card> Comp2Cards { get { return round.Players.Where(p => p.name == "Comp 2").Single().hand.Cards; } }
        public ObservableCollection<Card> Comp3Cards { get { return round.Players.Where(p => p.name == "Comp 3").Single().hand.Cards; } }

        public delegate void IsInMode(GameState gameState);

        public event PropertyChangedEventHandler PropertyChanged;
        public event IsInMode GameStateChanged;

        public BaseGameViewModel(Round round, InfoPanelViewModel infoPanelVM)
        {
            this.round = round;
            this.infoPanelVM = infoPanelVM;
            HandVM = new HandViewModel(this);
            CurrentGameState = GameState.BIDDING;
        }


        public void ChooseAction(Action action)
        {
            round.BiddingDoAction(action);
            infoPanelVM.propChanged();
            if (!round.InBiddingPhase) {
                    EndBiddingRound();
                    CurrentGameState = GameState.PLAYING;
                }
        }

        public void EndBiddingRound()
        {
            round.EndBiddingRound();
            whistController = round.Start();
            NotifyUI();
        }

        public IEnumerable<Action> BiddingActions
        {
            get { return round.BiddingGetPossibleActions(); }
        }

        public void PlayCard(Card card) {

            whistController.PlayCard(card);
            if (!round.TrickInProgress)
            {
                MessageBoxResult result = MessageBox.Show(round.PileOwner.name + " won the trick", "Trick End", MessageBoxButton.OK, MessageBoxImage.None);
                round.EndTrick();
            }


            var AI = new SimpleGameAI();
            while (round.CurrentPlayer != null && round.CurrentPlayer != round.Players[0])
            {
                while (round.TrickInProgress && round.CurrentPlayer != null && round.CurrentPlayer != round.Players[0])
                {
                    var aiCard = AI.GetMove(round.CurrentPlayer, round.Pile, round.Trump);
                    round.PlayCard(aiCard);

                    if (UpdateView != null) UpdateView();

                }
                if (!round.TrickInProgress)
                {
                    MessageBoxResult result = MessageBox.Show(round.PileOwner.name + " won the trick", "Trick End", MessageBoxButton.OK, MessageBoxImage.None);
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

        public event System.Action UpdateView;

        public bool IsValidPlay(Card card) {
            if (!round.InTrickPhase)
                return false;
            return whistController.IsValidPlay(card);
        }

        public ObservableCollection<Card> GetCurrentPlayerCards() {
            return round.CurrentPlayer.hand.Cards;
        }

        public void StartNewRound()
        {
            round = new Round(round.Players);
            whistController = null;
            CurrentGameState = GameState.BIDDING;
            NotifyUI();
        }

        public void NotifyUI()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Pile"));
                PropertyChanged(this, new PropertyChangedEventArgs("HandVM"));
                PropertyChanged(this, new PropertyChangedEventArgs("Trump"));
                PropertyChanged(this, new PropertyChangedEventArgs("Round"));
                PropertyChanged(this, new PropertyChangedEventArgs("infoPanelVM"));
            }
            infoPanelVM.propChanged();
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

                handViewModel.mainViewModel.PropertyChanged += (sender, args) =>
                {
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, new System.EventArgs());
                    }
                };
            }

            public bool CanExecute(object parameter)
            {
                Card card = parameter as Card;
                if (card == null) return false;
                return handViewModel.mainViewModel.IsValidPlay(card);
            }

            public event System.EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                Card card = parameter as Card;
                if (card == null) return;
                handViewModel.playCard(card);
            }
        }

    }
}
