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
        protected int trumpsPlayed;

        public MemoryAI(Player player, GameManager game) : base(player, game)
        {
            memory = new Dictionary<Player, AIMemory>();
            foreach (Player oplayer in game.Players)
            {
                if (oplayer != player)
                {
                    memory.Add(oplayer, new AIMemory());
                }
            }
            trumpsPlayed = 0;
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
            if (Round.Pile.Count > 0)
            {
                var leadCard = Round.Pile[0];
                if (leadCard.Suit != card.Suit)
                    memory[otherPlayer].NoCardsOfSuitLeft(leadCard.Suit);
            }
            if (card.Suit == Round.Trump)
                trumpsPlayed++;
        }
        

        public override Card GetMove()
        {
            Card card = base.GetMove();
            if (card.Suit == Round.Trump)
                trumpsPlayed++;
            return card;
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
