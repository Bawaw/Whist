using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            HandVM = new HandViewModel(round.GetPlayerCards(round.CurrentPlayer), this);
        }

        public void PlayCard(Card card)
        {
            round.PlayCard(card);
        }
    }

    public class HandViewModel
    {
        private MainViewModel mainVM;
        public IList<CardViewModel> PlayerCards { get; private set; }

        public HandViewModel(IList<Card> cards, MainViewModel mainVM)
        {
            this.mainVM = mainVM;
            PlayerCards = new List<CardViewModel>();
            foreach (Card card in cards)
                PlayerCards.Add(new CardViewModel(card, this));
        }

        public void PlayCard(Card card)
        {
            mainVM.PlayCard(card);
        }
    }

    public class CardViewModel
    {
        public Card Card { get; private set; }
        private HandViewModel handVM;

        public CardViewModel(Card card, HandViewModel handVM)
        {
            Card = card;
            this.handVM = handVM;
        }


        public void PlayCard()
        {
            handVM.PlayCard(Card);
        }
    }
    
}
