using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    public abstract class GameController 
    {
        protected IDealer dealer;
        protected IReferee referee;

        protected DeckCollection deck;
        protected Player[] players;
        protected List<Card> pile;

        protected int currentPlayer;

        public Player CurrentPlayer { get { return players[currentPlayer]; } }
        public List<Card> Pile { get {return pile; } }

        public GameController(Player[] players, IReferee referee)
        {
            this.players = players;
            this.referee = referee;
            pile = new List<Card>();
        }

        public IList<Card> GetPlayerCards()
        {
            return CurrentPlayer.hand.Cards;
        }

        public IList<Card> GetPlayerCards(Player player)
        {
            return new ObservableCollection<Card>(player.hand.Cards);
        }

        public abstract bool PlayCard(Card card);
       
        protected void nextPlayer() {
            if (currentPlayer + 1 < players.Length)
                currentPlayer++;
            else
                currentPlayer = 0;
        }

        //is hand over? (all cards played)
        public bool isEndGame() {
            if (players[currentPlayer].hand.Count <= 0)
                return true;
            return false;
        }
    }

    public class WhistController : GameController, IPlayTricks
    {
        private Card lead;
        private Suits trump;
        public Suits Trump { get { return trump; } }
        private bool troel;
        public bool Troel { get { return troel; } }

        private int pileOwner;
        public Player PileOwner { get { return players[pileOwner]; } }

        public bool HasTrickEnded
        {
            get
            {
                if (pile.Count >= players.Length)
                    return true;
                return false;
            }
        }

        public bool InTrickPhase
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public WhistController(Player[] players, Suits trump ,IReferee referee) 
            : base(players, referee)
        {
            this.trump = trump;
        }


        //play a card for current player, returns true if valid play
        public override bool PlayCard(Card card)
        {
            if (pile.Count <= 0) {
                pileOwner = currentPlayer;
                lead = card;
                pile.Add(card);
                CurrentPlayer.hand.Play(card);
                nextPlayer();
                return true;
            }

            if (!referee.ValidateMove(card, lead, new List<Card>(players[currentPlayer].hand.Cards)))
                return false;

            //add card to pile
            pile.Add(CurrentPlayer.hand.Play(card));

            //if card is same suit
            if (card.Suit == lead.Suit)
            {
                //check cards with same suit by highest number
                var list = pile.Where(x => x.Suit == lead.Suit);
                if (list.All(crd => (int)crd.Number <= (int)card.Number))
                {
                    pileOwner = currentPlayer;
                }
            }

            //if card is of trump suit
            else if (card.Suit == trump)
                //check cards with same suit by highest number
                if (pile.Where(x => x.Suit == trump).All(crd => (int)crd.Number <= (int)card.Number))
                {
                    pileOwner = currentPlayer;
                }
            //else ignore card
            nextPlayer();
            return true;
        }

        public bool IsValidPlay(Card card) {
            if (!referee.ValidateMove(card, lead, new List<Card>(players[currentPlayer].hand.Cards)))
                return false;
            return true;
        }

        //Call if turn ends to clean up, returns trick winner
        public Player EndTrick()
        {
            players[pileOwner].addTrick();
            pile.Clear();
            lead = null;
            currentPlayer = pileOwner;
            return players[pileOwner];
        }

        
    }
}
