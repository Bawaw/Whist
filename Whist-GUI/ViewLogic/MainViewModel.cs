
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

        private GameManager gameManager;
        private Round Round
        {
            get { return gameManager.Round; }
        }

        private InfoPanelViewModel infoPanelVM;

        public ObservableCollection<Card> Pile { get { return whistController?.Pile; } }
        public HandViewModel HandVM { get; private set; }
        public Suits Trump { get { return Round.Trump; } }
        public Player CurrentPlayer { private set; get; }

        private IPlayTricks whistController;

        public ObservableCollection<Card> Comp1Cards { get { return Round.Players.Where(p => p.name == "Comp 1").Single().hand.Cards; } }
        public ObservableCollection<Card> Comp2Cards { get { return Round.Players.Where(p => p.name == "Comp 2").Single().hand.Cards; } }
        public ObservableCollection<Card> Comp3Cards { get { return Round.Players.Where(p => p.name == "Comp 3").Single().hand.Cards; } }

        public delegate void IsInMode(GameState gameState);

        public event PropertyChangedEventHandler PropertyChanged;
        public event IsInMode GameStateChanged;

        public BaseGameViewModel(GameManager gameManager, InfoPanelViewModel infoPanelVM)
        {
            this.gameManager = gameManager;
            this.infoPanelVM = infoPanelVM;
            HandVM = new HandViewModel(this);
            CurrentGameState = GameState.BIDDING;
        }

        public async void ChooseAction(Action action)
        {
            Round.BiddingDoAction(action);
            while (Round.InBiddingPhase && Round.CurrentPlayer != gameManager.HumanPlayer)
                await AsyncBidAI();

            if (!Round.InBiddingPhase) {
                EndBiddingRound();
                    CurrentGameState = GameState.PLAYING;
                }
            infoPanelVM.PropChanged();
        }

        Task AsyncBidAI()
        {
            return Task.Run(() => {
                Round.BiddingDoAction(SimpleBiddingAI.GetAction(Round.CurrentPlayer, Round.BiddingGetPossibleActions(), Round.Trump));
            }); 
        }

        public void EndBiddingRound()
        {
            Round.EndBiddingRound();
            whistController = Round.Start();
            while (Round.TrickInProgress && Round.CurrentPlayer != null && Round.CurrentPlayer != gameManager.HumanPlayer)
            {
                var aiCard = SimpleGameAI.GetMove(Round);
                Round.PlayCard(aiCard);
            }
            NotifyUI();
        }

        public IEnumerable<Action> BiddingActions
        {
            get { return Round.BiddingGetPossibleActions(); }
        }

        public async void PlayCard(Card card) {
            if (Round.CurrentPlayer.Number != 1) return; //TEMP fix for play out of turn
            CurrentPlayer = Round.CurrentPlayer;
            whistController.PlayCard(card);
            if (!Round.TrickInProgress)
            {
                MessageBoxResult result = MessageBox.Show(Round.PileOwner.name + " won the trick", "Trick End", MessageBoxButton.OK, MessageBoxImage.None);
                Round.EndTrick();
            }


            var AI = new SimpleGameAI();
            while (Round.CurrentPlayer != null && Round.CurrentPlayer != gameManager.HumanPlayer)
            {
                while (Round.TrickInProgress && Round.CurrentPlayer != null && Round.CurrentPlayer != gameManager.HumanPlayer)
                {
                    await AsyncPlayCardAI();
                }
                if (!Round.TrickInProgress)
                {
                    MessageBoxResult result = MessageBox.Show(Round.PileOwner.name + " won the trick", "Trick End", MessageBoxButton.OK, MessageBoxImage.None);
                    Round.EndTrick();
                }
            }

            if (!Round.InTrickPhase)
            {
                Round.EndTricksRound();
                string str = "";
                foreach (var player in Round.Players)
                    str += player.name + " (" + player.Tricks + ") - " + player.score + "\n";
                
                MessageBoxResult result = MessageBox.Show(str, "Round End", MessageBoxButton.OK, MessageBoxImage.None);
                StartNewRound();
            }
            NotifyUI();
        }

        Task AsyncPlayCardAI()
        {
            return Task.Run(() => {
                Thread.Sleep(600);
                var aiCard = SimpleGameAI.GetMove(Round);
                CurrentPlayer = Round.CurrentPlayer;
                App.Current.Dispatcher.Invoke(() => Round.PlayCard(aiCard));
            });
        }

        public bool IsValidPlay(Card card) {
            if (!Round.InTrickPhase)
                return false;
            return whistController.IsValidPlay(card);
        }

        public ObservableCollection<Card> GetCurrentPlayerCards() {
            return gameManager.HumanPlayer.hand.Cards;
        }

        public void StartNewRound()
        {
            gameManager.StartNewRound();
            CurrentGameState = GameState.BIDDING;
            whistController = null;
            CurrentGameState = GameState.BIDDING;
            NotifyUI();
            while (Round.InBiddingPhase && Round.CurrentPlayer != gameManager.HumanPlayer)
            {
                Round.BiddingDoAction(SimpleBiddingAI.GetAction(Round.CurrentPlayer, Round.BiddingGetPossibleActions(), Round.Trump));
            }
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
            infoPanelVM.PropChanged();
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
