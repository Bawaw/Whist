using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Whist.AI;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist_GUI.ViewLogic
{
    public class BaseGameViewModel
    {
        private Round round;

        public ObservableCollection<Card> Pile { get { return whistController.Pile; } }
        public HandViewModel HandVM { get; private set; }
        public Suits Trump { get { return round.Trump; } }

        private IPlayTricks whistController;

        public BaseGameViewModel()
        {
            HandVM = new HandViewModel(this);

            Player[] players = new Player[]
            {
                new Player("Player"),
                new Player("Comp 1"),
                new Player("Comp 2"),
                new Player("Comp 3")
            };
            round = new Round(players);

            whistController = round.Start();
        }

        public void PlayCard(Card card) {
            whistController.PlayCard(card);

            var AI = new SimpleGameAI();
            while (round.CurrentPlayer != null && round.CurrentPlayer != round.Players[0])
            {
                while (round.CurrentPlayer != null && round.CurrentPlayer != round.Players[0])
                {
                    var aiCard = AI.GetMove(round.CurrentPlayer, round.Pile, round.Trump);
                    round.PlayCard(aiCard);
                }
                if (!round.TrickInProgress)
                    round.EndTrick();
            }
        }

        public bool IsValidPlay(Card card) {
            return whistController.IsValidPlay(card);
        }

        public ObservableCollection<Card> GetCurrentPlayerCards() {
            return whistController.GetPlayerCards();
        }
    }

    public class HandViewModel
    {
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
    } 
}
