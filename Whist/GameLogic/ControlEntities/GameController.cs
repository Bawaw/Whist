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
        protected IReferee referee;

        protected DeckCollection deck;
        protected Player[] players;
        protected ObservableCollection<Card> pile;

        protected int currentPlayer;

        public Player CurrentPlayer { get { return players[currentPlayer]; } }
        public ObservableCollection<Card> Pile { get { return pile; } }

        public GameController(Player[] players, IReferee referee)
        {
            this.players = players;
            this.referee = referee;
            pile = new ObservableCollection<Card>();
        }

        public ObservableCollection<Card> GetPlayerCards()
        {
            return CurrentPlayer.hand.Cards;
        }

        public ObservableCollection<Card> GetPlayerCards(Player player)
        {
            return player.hand.Cards;
        }

        public abstract bool PlayCard(Card card);

        protected void nextPlayer()
        {
            if (currentPlayer + 1 < players.Length)
                currentPlayer++;
            else
                currentPlayer = 0;
        }

        //is hand over? (all cards played)
        public bool isEndGame()
        {
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

        private int pileOwner;
        public Player PileOwner { get { return players[pileOwner]; } }

        public bool TrickInProgress
        {
            get
            {
                if (!InTrickPhase)
                    return false;
                if (pile.Count >= players.Length)
                    return false;
                return true;
            }
        }

        public bool InTrickPhase
        {
            get
            {
                if (players.Any(p => p.hand.Cards.Count > 0))
                    return true;
                return false;
            }
        }

        public WhistController(Player[] players, Player FirstPlayer, Suits trump, IReferee referee)
            : base(players, referee)
        {
            this.trump = trump;
            for (int i = 0; i < players.Length; i++)
                if (players[i] == FirstPlayer)
                {
                    currentPlayer = i;
                    break;
                }
            CardPlayedByPlayer = new Dictionary<Player, Card>();
            foreach (Player player in players)
                CardPlayedByPlayer.Add(player, null);
        }


        //play a card for current player, returns true if valid play
        public override bool PlayCard(Card card)
        {
            if (pile.Count <= 0)
            {
                pileOwner = currentPlayer;
                lead = card;
                pile.Add(card);
                CardPlayedByPlayer[CurrentPlayer] = card;
                CurrentPlayer.hand.Play(card);
                nextPlayer();
                return true;
            }

            if (!referee.ValidateMove(card, lead, new List<Card>(players[currentPlayer].hand.Cards)))
                return false;

            //add card to pile
            pile.Add(CurrentPlayer.hand.Play(card));
            CardPlayedByPlayer[CurrentPlayer] = card;

            var pileTrumps = pile.Where(x => x.Suit == trump);
            //if card is same suit and (there are no trump cards or lead suit is trump)
            if (card.Suit == lead.Suit && (pileTrumps.Count() == 0 || lead.Suit == Trump))
            {
                //check cards with same suit by highest number
                var list = pile.Where(x => x.Suit == lead.Suit);
                if (list.All(crd => (int)crd.Number <= (int)card.Number))
                {
                    pileOwner = currentPlayer;
                }
            }
            else if (card.Suit == trump)//if card is of trump suit
            {
                //check cards with same suit by highest number
                if (pileTrumps.Count() == 0 || pileTrumps.All(crd => (int)crd.Number <= (int)card.Number))
                {
                    pileOwner = currentPlayer;
                }
            }
            //else ignore card
            nextPlayer();
            return true;
        }

        public bool IsValidPlay(Card card)
        {
            if (!referee.ValidateMove(card, lead, new List<Card>(players[currentPlayer].hand.Cards)))
                return false;
            return true;
        }

        //Call if turn ends to clean up, returns trick winner
        public Player EndTrick()
        {
            players[pileOwner].addTrick();
            pile.Clear();
            foreach (Player player in players)
                CardPlayedByPlayer[player] = null;
            lead = null;
            currentPlayer = pileOwner;
            return players[pileOwner];
        }

        public Dictionary<Player, Card> CardPlayedByPlayer { get; private set; }

    }
}
