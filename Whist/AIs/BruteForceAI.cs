using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class BruteForceAI : IGameAI
    {
        private Player player;
        private GameManager game;
        private Round Round { get { return game.Round; } }

        public BruteForceAI(Player player, GameManager game)
        {
            this.player = player;
            this.game = game;
        }

        public Card GetMove()
        {
            return BruteForceRound();
        }

        public Card BruteForceRound()
        {
            var cards = player.hand.Cards;
            var possibleCards = cards.Where(c => Round.IsValidPlay(c));

            var trickPossibilities = new List<TrickSimulator>();
            var initialTrick = new TrickSimulator(Round);

            var possibles = new Dictionary<Card, int>();

            Parallel.ForEach(possibleCards, (card) => { DoSingleTree(possibles, card); });
            return possibles.Where(p => p.Value == possibles.Values.Max()).First().Key;
        }

        public void DoSingleTree(Dictionary<Card, int> possibles, Card card)
        {
            var newTrick = new TrickSimulator(Round);
            newTrick.AddCardToPile(card, player.Number);
            var hands = GetHands();
            hands[player.Number].Remove(card);
            int result = RecBruteForceRound(new int[4], hands, GetNextPlayer(player.Number), newTrick, 0);
            lock (possibles)
            {
                possibles.Add(card, result);
            }
        }

        public int RecBruteForceRound(int[] trickWins, List<Card>[] hands, int currentPlayer, TrickSimulator currentTrick, int depth)
        {
            if (depth > 3 || hands.All(h => h.Count == 0))//round is over.
            {
                int teamTricks = 0;
                foreach (Player p in Round.Teams.Where(t => t.Players.Any(p => p == player)).Single().Players)
                {
                    teamTricks += trickWins[p.Number];
                }
                return teamTricks;
            }

            if (currentTrick.TrickEnded())
            {
                trickWins[currentTrick.TrickWinner()]++;
                currentTrick = new TrickSimulator(Round.Trump);
            }

            var possibles = new List<int>();

            var copyOfHand = hands[currentPlayer].ToArray();
            foreach (Card card in copyOfHand)
            {
                if (currentTrick.IsValidCard(card, hands[currentPlayer]))
                {
                    var newTrick = currentTrick.Copy();
                    newTrick.AddCardToPile(card, currentPlayer);
                    hands[currentPlayer].Remove(card);
                    int[] newTrickwins = new int[4];
                    trickWins.CopyTo(newTrickwins, 0);
                    possibles.Add(RecBruteForceRound(newTrickwins, hands, GetNextPlayer(currentPlayer), newTrick, depth++));

                    hands[currentPlayer].Add(card);
                }
            }

            if (Round.Teams.Where(t => t.Players.Any(p => p == player)).Single().Players.Any(p => p.Number == currentPlayer))
                return possibles.Max();
            else
                return possibles.Min();
        }

        public void RecBruteForceTrick(List<TrickSimulator> tricks, TrickSimulator trick, List<Card>[] hands, int currentPlayer)
        {
            if (trick.TrickEnded())
            {
                tricks.Add(trick);// trick.TrickWinner();
            }

            foreach (var card in hands[currentPlayer])
            {
                if (trick.IsValidCard(card, hands[currentPlayer]))
                {
                    var newTrick = trick.Copy();
                    newTrick.AddCardToPile(card, currentPlayer);
                    RecBruteForceTrick(tricks, newTrick, hands, GetNextPlayer(currentPlayer));
                }
            }
        }



        public List<Card>[] GetHands()
        {
            var hands = new List<Card>[4];
            foreach (Player player in Round.Players)
            {
                hands[player.Number] = new List<Card>();
                foreach (Card card in player.hand.Cards)
                    hands[player.Number].Add(card);
            }
            return hands;
        }

        public int GetNextPlayer(int currentPlayer)
        {
            if (currentPlayer == 3)
                return 0;
            return currentPlayer + 1;
        }


        public void ProcessOtherPlayerAction(Player otherPlayer, GameLogic.ControlEntities.Action action) { }
        public void ProcessOtherPlayerCard(Player otherPlayer, Card card) { }
        public void ResetMemory() { }
    }

    public class TrickSimulator
    {
        Suits trump;
        Card[] pile;
        int[] players;

        public TrickSimulator(Suits trump)
        {
            pile = new Card[4];
            players = new int[4];
            this.trump = trump;
        }

        public TrickSimulator(Round round)
        {
            pile = new Card[4];
            players = new int[4];
            trump = round.Trump;

            for (int i = 0; i < round.Pile.Count; i++)
            {
                pile[i] = round.Pile[i];
                players[i] = round.PlayerWhoPlayedCard(pile[i]).Number;
            }
        }

        public TrickSimulator Copy()
        {
            var copy = new TrickSimulator(trump);
            for (int i = 0; i < pile.Count(); i++)
            {
                copy.pile[i] = pile[i];
                copy.players[i] = players[i];
            }
            return copy;
        }

        public bool TrickEnded()
        {
            return pile.All(c => c != null);
        }

        public bool IsValidCard(Card card, List<Card> hand)
        {
            if (TrickEnded())
                return false;
            if (pile[0] == null)
                return true;
            if (hand.Any(c => c.Suit == pile[0].Suit))
                return card.Suit == pile[0].Suit;
            return true;
        }

        public void AddCardToPile(Card card, int player)
        {
            for (int i = 0; i < 4; i++)
            {
                if (pile[i] == null)
                {
                    pile[i] = card;
                    players[i] = player;
                    return;
                }
            }
        }

        public int TrickWinner()
        {
            var wincard = WinningCard();
            for (int i = 0; i < 4; i++)
            {
                if (pile[i] == wincard)
                    return players[i];
            }
            throw new ApplicationException();
        }

        public Card WinningCard()
        {
            if (pile.Any(c => c.Suit == trump))
            {
                var trumps = pile.Where(c => c.Suit == trump);
                return trumps.Where(c => c.Number == trumps.Max(tr => tr.Number)).Single();
            }
            else
            {
                var lsuitcards = pile.Where(c => c.Suit == pile[0].Suit);
                return lsuitcards.Where(c => c.Number == lsuitcards.Max(tr => tr.Number)).Single();
            }
        }
    }

}
