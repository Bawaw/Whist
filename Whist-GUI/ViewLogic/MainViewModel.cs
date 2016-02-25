
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Whist.AIs;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI.ViewLogic
{
    public enum GameState { BIDDING, PLAYING, ENDTRICK }
    public class BaseGameViewModel : INotifyPropertyChanged
    {
        private GameState currentGameState;
        public GameState CurrentGameState
        {
            get { return currentGameState; }
            private set
            {
                if (value != currentGameState)
                {
                    currentGameState = value;
                    if (GameStateChanged != null)
                        GameStateChanged(value);
                }
            }
        }

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
        public Player LatestTrickWinner { get { return Round.PileOwner; } }

        private IPlayTricks whistController;

        public ObservableCollection<Card> Comp1Cards { get { return Round.Players.Where(p => p.name == "Comp 1").Single().hand.Cards; } }
        public ObservableCollection<Card> Comp2Cards { get { return Round.Players.Where(p => p.name == "Comp 2").Single().hand.Cards; } }
        public ObservableCollection<Card> Comp3Cards { get { return Round.Players.Where(p => p.name == "Comp 3").Single().hand.Cards; } }

        public bool Comp1InPlayerTeam
        {
            get { if (!Round.InTrickPhase) return false; return Round.Teams.Where(t => t.Players.Any(p => p == gameManager.HumanPlayer)).Single().Players.Contains(Round.Players.Where(p => p.name == "Comp 1").Single()); }
        }
        public bool Comp2InPlayerTeam
        {
            get { if (!Round.InTrickPhase) return false; return Round.Teams.Where(t => t.Players.Any(p => p == gameManager.HumanPlayer)).Single().Players.Contains(Round.Players.Where(p => p.name == "Comp 2").Single()); }
        }
        public bool Comp3InPlayerTeam
        {
            get { if (!Round.InTrickPhase) return false; return Round.Teams.Where(t => t.Players.Any(p => p == gameManager.HumanPlayer)).Single().Players.Contains(Round.Players.Where(p => p.name == "Comp 3").Single()); }
        }

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
            infoPanelVM.AddLineToActionLog(Round.CurrentPlayer.name + ": " + action.ToString());
            Round.BiddingDoAction(action);
            while (Round.InBiddingPhase && Round.CurrentPlayer != gameManager.HumanPlayer)
                await AsyncBidAI();

            if (!Round.InBiddingPhase)
            {
                EndBiddingRound();
                CurrentGameState = GameState.PLAYING;
            }
            NotifyUI();
        }

        Task AsyncBidAI()
        {
            return Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() => AIBid());
            });
        }

        private void AIBid()
        {
            if (Round.CurrentPlayer == gameManager.HumanPlayer)
                return;
            var action = gameManager.GetBidAI(Round.CurrentPlayer).GetAction();
            //not a place to do this
            //foreach (Player otherAI in gameManager.NonHumanPlayers.Except(new Player[] { Round.CurrentPlayer }))
                //gameManager.GetBidAI(otherAI).ProcessOtherPlayerAction(Round.CurrentPlayer, action);
            infoPanelVM.AddLineToActionLog(Round.CurrentPlayer.name + ": " + action.ToString());
            Round.BiddingDoAction(action);
        }

        public async void EndBiddingRound()
        {
            Round.EndBiddingRound();
            whistController = Round.Start();
            CurrentPlayer = Round.CurrentPlayer;
            while (Round.TrickInProgress && Round.CurrentPlayer != null && Round.CurrentPlayer != gameManager.HumanPlayer)
            {
                await AsyncPlayCardAI();
            }
            NotifyUI();
        }

        public IEnumerable<Action> BiddingActions
        {
            get { return Round.BiddingGetPossibleActions(); }
        }

        public async void AI_PlaysCards()
        {
            var AI = new SimpleGameAI();
            while (Round.TrickInProgress && Round.CurrentPlayer != null && Round.CurrentPlayer != gameManager.HumanPlayer)
            {
                await AsyncPlayCardAI();
            }
            if (!Round.TrickInProgress)
            {
                //MessageBoxResult result = MessageBox.Show(Round.PileOwner.name + " won the trick", "Trick End", MessageBoxButton.OK, MessageBoxImage.None);
                //trickEndVM.Winner = Round.PileOwner.name + " won the trick";
                //trickEndVM.Visibility = "Visible";
                //Round.EndTrick();
                CurrentGameState = GameState.ENDTRICK;
            }
            NotifyUI();
        }

        public void PlayCard(Card card)
        {
            infoPanelVM.AddLineToActionLog(Round.CurrentPlayer.name + ": " + card.ToString());
            CurrentPlayer = Round.CurrentPlayer;
            whistController.PlayCard(card);
            if (!Round.TrickInProgress)
                CurrentGameState = GameState.ENDTRICK;

            NotifyUI();
            AI_PlaysCards();
        }

        public void StartNewTrick()
        {
            Round.EndTrick();
            CurrentGameState = GameState.PLAYING;
            if (!Round.InTrickPhase)
            {
                Round.EndTricksRound();
                string str = "";
                foreach (var player in Round.Players)
                    str += player.name + " (" + player.Tricks + ") - " + player.score + "\n";

                MessageBoxResult result = MessageBox.Show(str, "Round End", MessageBoxButton.OK, MessageBoxImage.None);
                StartNewRound();
            }
        }

        Task AsyncPlayCardAI()
        {
            return Task.Run(() =>
            {
                Thread.Sleep(300);
                CurrentPlayer = Round.CurrentPlayer;
                var aiCard = AIPlay();
                App.Current.Dispatcher.Invoke(() => Round.PlayCard(aiCard));
            });
        }
        //TODO: use referee
        private Card AIPlay()
        {
            if (Round.CurrentPlayer == gameManager.HumanPlayer)
                return null;
            var aiCard = gameManager.GetGameAI(Round.CurrentPlayer).GetMove(new StandardReferee());
            //foreach (Player otherAI in gameManager.NonHumanPlayers.Except(new Player[] { Round.CurrentPlayer }))
                //gameManager.GetAI(otherAI).ProcessOtherPlayerCard(Round.CurrentPlayer, aiCard);
            infoPanelVM.AddLineToActionLog(CurrentPlayer.name + ": " + aiCard.ToString());
            return aiCard;
        }

        public bool IsValidPlay(Card card)
        {
            if (Round.CurrentPlayer != gameManager.HumanPlayer)
                return false;
            if (!Round.InTrickPhase || !Round.TrickInProgress)
                return false;
            return whistController.IsValidPlay(card);
        }

        public ObservableCollection<Card> GetCurrentPlayerCards()
        {
            return gameManager.HumanPlayer.hand.Cards;
        }

        public void StartNewRound()
        {
            gameManager.StartNewRound();
            infoPanelVM.ClearActionLog();
            CurrentGameState = GameState.BIDDING;
            whistController = null;
            CurrentGameState = GameState.BIDDING;
            NotifyUI();

            while (Round.InBiddingPhase && Round.CurrentPlayer != gameManager.HumanPlayer)
            {
                AIBid();
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
                PropertyChanged(this, new PropertyChangedEventArgs("Comp1InPlayerTeam"));
                PropertyChanged(this, new PropertyChangedEventArgs("Comp2InPlayerTeam"));
                PropertyChanged(this, new PropertyChangedEventArgs("Comp3InPlayerTeam"));

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

        public void playCard(Card card)
        {
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
