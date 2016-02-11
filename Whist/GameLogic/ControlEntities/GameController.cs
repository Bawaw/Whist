using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    public enum GameMode { SIMPLE, ABONDANCE, MISERIE, TROEL, SOLO, SOLOSLIM };
    public abstract class GameController
    {
        public static Random rng;

        protected IDealer dealer;
        private IScoreMechanisme scoreMechanism;
        protected IReferee referee;

        protected Team[] teams;
        protected DeckCollection deck;
        protected Player[] players;
        protected List<Card> pile;

        protected int currentPlayer;

        protected GameMode gameMode;

        public Player CurrentPlayer { get { return players[currentPlayer]; } }
        public List<Card> Pile { get {return pile; } }

        public GameController(Player[] players, Team[] teams , IDealer dealer, IScoreMechanisme scoreMechanism, IReferee referee, DeckCollection deck)
        {
            this.players = players;
            this.teams = teams;
            this.dealer = dealer;
            this.scoreMechanism = scoreMechanism;
            this.referee = referee;
            this.deck = deck;
            rng = new Random();
            pile = new List<Card>();
        }

        public void start()
        {
            dealer.Deal(players, deck);
            currentPlayer = rng.Next(0, 4);
        }

        public IList<Card> GetPlayerCards()
        {
            return CurrentPlayer.hand.Cards;
        }

        public IList<Card> GetPlayerCards(Player player)
        {
            return player.hand.Cards;
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

        //reset player tricks and returns winning team
        public Team EndGame() {
            return scoreMechanism.distributeScore(teams, gameMode);
        }
    }

    public class WhistController : GameController
    {
        private Card lead;
        private Suits trump;
        public Suits Trump { get { return trump; } }
        private bool troel;
        public bool Troel { get { return troel; } }

        private int pileOwner;
        public Player PileOwner { get { return players[pileOwner]; } }

        public WhistController(Player[] players, Team[] teams, IDealer dealer, IScoreMechanisme scoreMechanisme, IReferee referee, DeckCollection deck) 
            : base(players, teams, dealer, scoreMechanisme, referee, deck) { }

        public new void start() {
            trump = dealer.Deal(players, deck).Suit;
            currentPlayer = rng.Next(0, 4);
            troel = checkForTroel();
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

        //have all players played their card?
        public bool isEndTrick()
        {
            if (pile.Count >= players.Length)
                return true;
            return false;
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

        private bool checkForTroel()
        {
            List<Player> troelPlayers = new List<Player>();
            for (int i=0; i<players.Count(); i++)
            {
                int aces = 0;
                foreach(Card card in players[i].hand.Cards)
                {
                    if(card.Number == Numbers.ACE)
                    {
                        aces++;
                    }
                }
                if(aces == 3 || aces == 4)
                {
                    if (aces == 3)
                    {
                        troelPlayers.Add(players[i]);
                        bool teamPlayerFound = false;
                        for (int j = 0; j < players.Count(); j++)
                        {
                            //if it is another player than current player
                            if(j != i)
                            {
                                foreach (Card card in players[j].hand.Cards)
                                {
                                    if (card.Number == Numbers.ACE)
                                    {
                                        teamPlayerFound = true;
                                        troelPlayers.Add(players[j]);
                                        //set trump and currentPlayer
                                        trump = card.Suit;
                                        currentPlayer = j;
                                        //card found, break out of loop of cards
                                        break;
                                    }
                                }
                                if (teamPlayerFound)
                                {
                                    //player found, break out of loop of players
                                    break;
                                }
                            }
                        }
                    }
                    else if (aces == 4)
                    {
                        troelPlayers.Add(players[i]);
                        //search for player with king of hearts, or if player with 4 aces also contains king of hearts, search for player with queen of hearts and so on
                        Card highestHeart = new Card(Suits.HEARTS, Numbers.KING);
                        while (players[i].hand.Cards.Contains(highestHeart))
                        {
                            //highestHeart--;
                            int number = (int)highestHeart.Number;
                            highestHeart = new Card(1, number--);
                        }
                        //teamplayer is player with highestHeart
                        Player teamPlayer = players.Where(p => p.hand.Cards.Contains(highestHeart)).First();
                        troelPlayers.Add(teamPlayer);
                        //set trump and currentPlayer
                        trump = Suits.HEARTS;
                        currentPlayer = Array.IndexOf(players, teamPlayer);
                    }
                    teams[0] = new Team(troelPlayers.ToArray(), teams[0].TeamName);
                    teams[1] = new Team((players.Except(troelPlayers)).ToArray(), teams[1].TeamName);
                    return true;
                }
            }
            return false;
        }
    }
}
