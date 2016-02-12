using System;
using System.Collections.Generic;
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
            HandVM = new HandViewModel(round.CurrentPlayer, this);
        }

        public bool PlayCard(Card card)
        {
            return true;// round.PlayCard(card);
        }

        internal IList<Card> PlayerCards(Player player)
        {
            return round.GetPlayerCards(player);
        }
    }

    public class HandViewModel : INotifyPropertyChanged
    {
        private MainViewModel mainVM;
        private Player player;

        public event PropertyChangedEventHandler PropertyChanged;

        public IList<CardViewModel> PlayerCards { get; private set; }

        public HandViewModel(Player player, MainViewModel mainVM)
        {
            this.mainVM = mainVM;
            this.player = player;
            PlayerCards = new List<CardViewModel>();
            foreach (Card card in mainVM.PlayerCards(player))
                PlayerCards.Add(new CardViewModel(card, this));
        }

        public void PlayCard(CardViewModel card)
        {
            if (mainVM.PlayCard(card.Card))
            {
                PlayerCards.Remove(card);
                //Notify view somehow
            }
        }
    }

    public class CardViewModel
    {
        public Card Card { get; private set; }
        private HandViewModel handVM;

        private readonly PlayCommand play;
        public ICommand Play { get { return play; } }

        public CardViewModel(Card card, HandViewModel handVM)
        {
            Card = card;
            this.handVM = handVM;
            play = new PlayCommand(this);
        }


        public void PlayCard()
        {
            handVM.PlayCard(this);
        }

        public bool IsValidMove
        {
            get { return true; }
        }

        private class PlayCommand : ICommand
        {
            CardViewModel card;

            public PlayCommand(CardViewModel card)
            {
                this.card = card;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                card.PlayCard();
            }
        }
    }
    
}
