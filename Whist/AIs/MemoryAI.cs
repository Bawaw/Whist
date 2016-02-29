using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class MemoryAI : BaseGameAI
    {
        protected Dictionary<Player, AIMemory> memory;
        protected HashSet<Card> playedCards;
        protected int trumpsPlayed;

        public MemoryAI(Player player, GameManager game) : base(player, game)
        {
            ResetMemory();
        }

        public override void ResetMemory()
        {
            memory = new Dictionary<Player, AIMemory>();
            foreach (Player oplayer in game.Players)
            {
                memory.Add(oplayer, new AIMemory());
            }
            trumpsPlayed = 0;
            playedCards = new HashSet<Card>();
        }

        public override void ProcessOtherPlayerAction(Player otherPlayer, Action action)
        {
            if (Round.GameCase == Case.UNDECIDED)
                switch (action)
                {
                    case Action.PASS:
                        {
                            memory[otherPlayer].minInitialHandStrength = 0;
                            memory[otherPlayer].maxInitialHandStrength = 4;
                            break;
                        }
                    case Action.ASK:
                        {
                            memory[otherPlayer].minInitialHandStrength = 5;
                            memory[otherPlayer].maxInitialHandStrength = 9;
                            break;
                        }
                    case Action.JOIN:
                        {
                            memory[otherPlayer].minInitialHandStrength = 3;
                            memory[otherPlayer].maxInitialHandStrength = 9;
                            break;
                        }
                    case Action.ABONDANCE:
                        {
                            memory[otherPlayer].minInitialHandStrength = 10;
                            memory[otherPlayer].maxInitialHandStrength = 13;
                            break;
                        }
                }
        }

        public override void ProcessOtherPlayerCard(Player otherPlayer, Card card)
        {
            if (otherPlayer != player)
            {
                if (Round.Pile.Count > 0)
                {
                    var leadCard = Round.Pile[0];
                    if (leadCard.Suit != card.Suit)
                        memory[otherPlayer].NoCardsOfSuitLeft(leadCard.Suit);
                }
                if (card.Suit == Round.Trump)
                    trumpsPlayed++;
            }
            if (card.Number >= Numbers.QUEEN)
                playedCards.Add(card);
        }


        public override Card GetMove()
        {
            Card card = base.GetMove();
            if (card.Suit == Round.Trump)
                trumpsPlayed++;
            return card;
        }

        protected override Card OpponentsRemainingAndHasPilesuit()
        {
            var cards = Round.CurrentPlayer.hand.Cards;
            var pile = Round.Pile;
            var pileSuit = pile[0].Suit;
            var leadCard = LeadingCard();
            var trump = Round.Trump;
            if (leadCard.Suit != pileSuit)// Leadingcard is trump while pilesuit isn't trump.
            {
                return GetLowestCardStrategic(cards, pileSuit);//Can't win, so give the lowest possible card instead.
            }
            else //leadingCard is pilesuit (which may or may not be trump)
            {
                var cardsOfSuit = cards.Where(c => c.Suit == pileSuit);
                if (IsTeamCurrentLeader())//If the team is winning, see if an opponent could possible outplay team.
                {
                    bool possibleHigherCard = false;
                    for (int i = (int)leadCard.Number + 1; i <= (int)Numbers.ACE; i++)
                    {
                        if (!playedCards.Any(c => c.Suit == pileSuit && c.Number == (Numbers)i) && cardsOfSuit.All(c => c.Number != (Numbers)i))
                        {//If the card is not already played, and all cards have a different number, the higher card is in another player's hand.
                            possibleHigherCard = true;
                        }
                    }
                    if (possibleHigherCard)
                    {
                        return HighOrLowCardSelection(cardsOfSuit, leadCard);//If an opponent can play a higher card, try winning.
                    }
                    else
                    {
                        return GetLowestCard(cardsOfSuit);//If an opponent won't be able to play a higher card, play a low card.
                    }
                }
                else//If the team is losing, try to win.
                {
                    return HighOrLowCardSelection(cardsOfSuit, leadCard);
                }
            }
        }


        protected override Card OpponentsRemainingAndHasNotPilesuit()
        {
            var cards = Round.CurrentPlayer.hand.Cards;
            var pile = Round.Pile;
            var pileSuit = pile[0].Suit;
            var leadCard = LeadingCard();
            var trump = Round.Trump;
            if (cards.Any(c => c.Suit == trump))//Hand has trump card.
            {
                var cardsOfTrump = cards.Where(c => c.Suit == trump);
                if (IsTeamCurrentLeader())
                {//Team is winning
                    if (leadCard.Suit == trump)
                    {
                        return GetLowestCardStrategic(cards, pileSuit);
                    }
                    else
                    {//Team is winning and leadcard isn't trump.
                        if (DoesRemainingOpponentHaveTrump() && !DoesRemainingOpponentHaveSuit(pileSuit))
                        {
                            return GetMiddleCarld(cardsOfTrump);
                        }
                        else
                        {//Opponents don't have trump card
                            if (leadCard.Number == Numbers.ACE || leadCard.Number == Numbers.KING)
                                return GetLowestCardStrategic(cards, pileSuit);
                            else
                                return GetLowestCard(cardsOfTrump);
                        }
                    }
                }
                else//Team is not winning.
                {
                    if (leadCard.Suit == trump)//Leadcard is trump.
                    {
                        if (GetHighestCard(cardsOfTrump).Number > leadCard.Number)//If AI can beat leadcard
                        {
                            return GetHighestCard(cardsOfTrump);//Give highest trump card
                        }
                        else//if AI can't beat leadcard
                        {
                            return GetLowestCardStrategic(cards, pileSuit);
                        }
                    }
                    else//Leadcard is not trump
                    {
                        if (DoesRemainingOpponentHaveTrump() && !DoesRemainingOpponentHaveSuit(pileSuit))
                        {
                            return GetMiddleCarld(cardsOfTrump);
                        }
                        else
                            return GetLowestCard(cardsOfTrump);//give lowest trump.
                    }
                }
            }
            else //Hand contains neither pileSuit card of trump card.
            {
                return GetLowestCard(cards);
            }
        }

        protected override Card StartPile()
        {
            var cards = player.hand.Cards;

            if (!DoesOpponentHaveTrump())//Opponents don't have trump cards left.
            {
                foreach (Suits suit in System.Enum.GetValues(typeof(Suits)))
                {
                    if (suit != Round.Trump)
                    {
                        if (cards.Any(c => c.Suit == suit) && !DoesOpponentHaveSuit(suit))//Player has cards of suit, but opponents don't.
                        {
                            return GetHighestCard(cards.Where(c => c.Suit == suit));
                        }
                    }
                }
            }
            else //Opponents have trump cards left.
            {
                if (cards.Any(c => c.Suit == Round.Trump))
                {
                    var trumps = cards.Where(c => c.Suit == Round.Trump);
                    int trumpcount = trumps.Count();
                    if ((13 - trumpsPlayed) / 4 > trumpcount)
                    {
                        Card highestTrump = GetHighestCard(cards.Where(c => c.Suit == Round.Trump));
                        if (highestTrump.Number >= Numbers.TEN)
                            return highestTrump;
                    }
                }
            }


            var htcard = HighestCardOfSuitIfNoHigherCardIsPossible(Round.Trump);
            if (htcard != null)
                return htcard;

            foreach (Suits suit in System.Enum.GetValues(typeof(Suits)))
            {
                if (suit != Round.Trump)
                {
                    var hcard = HighestCardOfSuitIfNoHigherCardIsPossible(suit);
                    if (hcard != null)
                        return hcard;
                }
            }

            return GetHighestCard(Round.CurrentPlayer.hand.Cards);
        }

        protected bool DoesRemainingOpponentHaveTrump()
        {
            return DoesRemainingOpponentHaveSuit(Round.Trump);
        }
        protected bool DoesOpponentHaveTrump()
        {
            return DoesOpponentHaveSuit(Round.Trump);
        }

        protected bool DoesRemainingOpponentHaveSuit(Suits suit)
        {
            foreach (Player pl in Round.PlayersLeft.Except(CurrentTeam().Players))
            {
                if (memory[pl].HasCardsOfSuitLeft(suit))
                    return true;
            }
            return false;
        }

        protected bool DoesOpponentHaveSuit(Suits suit)
        {
            foreach (Player pl in memory.Keys.Except(CurrentTeam().Players))
            {
                if (memory[pl].HasCardsOfSuitLeft(suit))
                    return true;
            }
            return false;
        }


        protected virtual Card HighestCardOfSuitIfNoHigherCardIsPossible(Suits suit)
        {
            var cards = player.hand.Cards;
            var cardsofsuit = cards.Where(c => c.Suit == suit);
            if (cardsofsuit.Count() == 0)
                return null;
            var highestCardOfSuit = cardsofsuit.Where(c => c.Number == cardsofsuit.Max(n => n.Number)).Single();
            bool isHigherCardPossible = false;
            for (Numbers i = highestCardOfSuit.Number + 1; i <= Numbers.ACE; i++)
            {
                if (!playedCards.Any(c => c.Suit == suit && c.Number == i))//linq querry similar to contains. if statement true if card has not been played yet.
                {
                    isHigherCardPossible = true;
                }
            }
            if (!isHigherCardPossible)
            {
                return highestCardOfSuit;
            }
            return null;
        }
    }


    public class AIMemory
    {
        private Dictionary<Suits, bool> hasCardsOfSuitLeft;
        public int minInitialHandStrength;
        public int maxInitialHandStrength;

        public AIMemory()
        {
            hasCardsOfSuitLeft = new Dictionary<Suits, bool>();
            foreach (Suits suit in System.Enum.GetValues(typeof(Suits)))
                hasCardsOfSuitLeft.Add(suit, true);
        }

        public bool HasCardsOfSuitLeft(Suits suit)
        {
            return hasCardsOfSuitLeft[suit];
        }

        public void NoCardsOfSuitLeft(Suits suit)
        {
            hasCardsOfSuitLeft[suit] = false;
        }
    }
}
