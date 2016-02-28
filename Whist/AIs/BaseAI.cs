using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class BaseBidAI : IBidAI
    {
        protected Player player;
        protected GameManager game;
        protected Round Round { get { return game.Round; } }

        public BaseBidAI(Player player, GameManager game)
        {
            this.player = player;
            this.game = game;
        }

        public virtual Action GetAction()
        {
            var possibleActions = Round.BiddingGetPossibleActions();

            int handStrength = GetHandStrength(Round.Trump);

            int alternateHandStrength = 0;
            Suits alternateSuit = Round.Trump;
            foreach (Suits suit in System.Enum.GetValues(typeof(Suits)))
            {
                if (suit != Round.Trump)
                {
                    int temp = GetHandStrength(suit);
                    if (temp > alternateHandStrength)
                    {
                        alternateHandStrength = temp;
                        alternateSuit = suit;
                    }
                }
            }

            if (!possibleActions.Contains(Action.PASS))
            {
                Suits preferredSuit = Round.Trump;
                if (alternateHandStrength > handStrength)
                    preferredSuit = alternateSuit;

                switch (preferredSuit)
                {
                    case Suits.HEARTS:
                        return Action.HEARTS;
                    case Suits.DIAMONDS:
                        return Action.DIAMONDS;
                    case Suits.SPADES:
                        return Action.SPADES;
                    case Suits.CLUBS:
                        return Action.CLUBS;
                    default:
                        return 0;
                }
            }


            if (handStrength > 10 || alternateHandStrength > 10)
            {
                if (possibleActions.Contains(Action.ABONDANCE))
                    return Action.ABONDANCE;
            }
            if (handStrength >= 6)
            {
                if (possibleActions.Contains(Action.ALONE))
                    return Action.ALONE;
            }
            if (handStrength >= 5)
            {
                if (possibleActions.Contains(Action.ASK))
                    return Action.ASK;
            }
            if (handStrength >= 4)
            {
                if (possibleActions.Contains(Action.JOIN))
                    return Action.JOIN;
            }

            return Action.PASS;
        }

        protected int GetHandStrength(Suits trump)
        {
            int handStrength = 0;

            var cards = player.hand.Cards;

            foreach (Suits suit in System.Enum.GetValues(typeof(Suits)))
            {
                if (suit != trump)
                {
                    var cardsOfSuit = cards.Where(c => c.Suit == suit);
                    int hCount = 1;
                    Numbers i = Numbers.ACE;
                    while (i >= Numbers.TEN)
                    {
                        if (cardsOfSuit.Any(c => c.Number == i))
                            handStrength++;
                        else if (hCount-- <= 0)
                            break;
                        i--;
                    }
                }
            }

            var cardsOfTrump = cards.Where(c => c.Suit == trump);
            int holeCount = 2;
            Numbers n = Numbers.ACE;
            while (n >= Numbers.FIVE)
            {
                if (cardsOfTrump.Any(c => c.Number == n))
                    handStrength++;
                else
                {
                    if (holeCount-- <= 0)
                        break;
                }
                n--;
            }


            return handStrength;
        }
    }

    public class BaseGameAI : IGameAI
    {

        protected Player player;
        protected GameManager game;
        protected Round Round { get { return game.Round; } }


        public BaseGameAI(Player player, GameManager game)
        {
            this.player = player;
            this.game = game;
        }

        public virtual void ResetMemory()
        {

        }


        public virtual void ProcessOtherPlayerAction(Player otherPlayer, Action action)
        {

        }

        public virtual void ProcessOtherPlayerCard(Player otherPlayer, Card card)
        {

        }

        public virtual Card GetMove()
        {
            if (Round.GameCase == Case.MISERIE)
                return MisserieMove();
            if (Round.Pile.Count > 0)
            {
                if (OpposingPlayersLeftInTrick()) //There will still be opposing players after current player's turn.
                {
                    return OpponentsRemaining();
                }
                else //There won't be opponents after current player's turn.
                {
                    return NoOpponentsRemaining();
                }
            }
            else//AI Starts Pile.
            {
                return StartPile();
            }
        }



        protected virtual Card OpponentsRemaining()
        {
            var cards = Round.CurrentPlayer.hand.Cards;
            var pile = Round.Pile;
            var pileSuit = pile[0].Suit;

            if (cards.Any(c => c.Suit == pileSuit))//Hand contains card of same suit as pile.
            {
                return OpponentsRemainingAndHasPilesuit();
            }
            else//Hand contains no card of the same suit as pile.
            {
                return OpponentsRemainingAndHasNotPilesuit();
            }
        }

        protected virtual Card OpponentsRemainingAndHasPilesuit()
        {
            var cards = Round.CurrentPlayer.hand.Cards;
            var pile = Round.Pile;
            var pileSuit = pile[0].Suit;
            var leadCard = LeadingCard();
            var trump = Round.Trump;
            if (leadCard.Suit != pileSuit)// Leadingcard is trump while pilesuit isn't trump.
            {
                return GetLowestCardStrategic(cards, pileSuit);
            }
            else //leadingCard is pilesuit (which may or may not be trump)
            {
                var cardsOfSuit = cards.Where(c => c.Suit == pileSuit);
                if (IsTeamCurrentLeader())//If the team is winning, see if an opponent could possible outplay team.
                {
                    bool possibleHigherCard = false;
                    for (int i = (int)leadCard.Number + 1; i <= (int)Numbers.ACE; i++)
                    {
                        if (cardsOfSuit.All(c => c.Number != (Numbers)i))//If all cards have a different number, the higher card is in another player's hand.
                        {
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

        protected virtual Card OpponentsRemainingAndHasNotPilesuit()
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
                    {
                        if (leadCard.Number == Numbers.ACE || leadCard.Number == Numbers.KING)
                            return GetLowestCardStrategic(cards, pileSuit);
                        else
                            return GetLowestCard(cardsOfTrump);
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
                        return GetLowestCard(cardsOfTrump);//give lowest trump.
                    }
                }
            }
            else //Hand contains neither pileSuit card of trump card.
            {
                return GetLowestCard(cards);
            }
        }

        protected virtual Card NoOpponentsRemaining()
        {
            var cards = Round.CurrentPlayer.hand.Cards;
            var pile = Round.Pile;
            var pileSuit = pile[0].Suit;
            var leadCard = LeadingCard();
            var trump = Round.Trump;

            if (IsTeamCurrentLeader())//AI's team is already winning.
            {
                return GetLowestCardStrategic(cards, pileSuit);
            }
            else //AI's team is not already winning.
            {
                //Give lowest winning card.
                if (cards.Any(c => c.Suit == pileSuit))//Hand contains card of same suit as pile.
                {
                    if (leadCard.Suit != pileSuit)// Leadingcard is trump while pilesuit isn't trump.
                    {
                        return GetLowestCardStrategic(cards, pileSuit);
                    }
                    else //leadingCard is pilesuit (which may or may not be trump)
                    {
                        var higherCardsOfPilesuit = cards.Where(c => c.Suit == pileSuit && c.Number > leadCard.Number);
                        if (higherCardsOfPilesuit.Count() > 0)
                            return GetLowestCard(higherCardsOfPilesuit);//player has higher cards than leading card and gives the lowest of them.
                        else
                            return GetLowestCardStrategic(cards, pileSuit);//player hasn't got higher cards than leading card so returns lowest card.
                    }
                }
                else//Hand doesn't contain card of the same suit as pile.
                {
                    if (cards.Any(c => c.Suit == trump))//Hand contains trump cards.
                    {
                        if (leadCard.Suit == trump)//Leading card is trump.
                        {
                            var higherCardsOfTrump = cards.Where(c => c.Suit == trump && c.Number > leadCard.Number);
                            if (higherCardsOfTrump.Count() > 0)
                                return GetLowestCard(higherCardsOfTrump);//player has higher cards than leading card and gives the lowest of them.
                            else
                                return GetLowestCardStrategic(cards, pileSuit);//player hasn't got higher cards than leading card so returns lowest card.
                        }
                        else //leading card is not trump
                        {
                            return GetLowestCard(cards.Where(c => c.Suit == trump));//return lowest trump card.
                        }
                    }
                    else //Hand contains neither pilesuit nor trump cards.
                    {
                        return GetLowestCard(cards);
                    }
                }
            }
        }

        protected virtual Card StartPile()
        {
            return GetHighestCard(Round.CurrentPlayer.hand.Cards);
        }

        protected bool OpposingPlayersLeftInTrick()
        {
            int remainingPlayersAfterCurrent = 3 - Round.Pile.Count();
            int index = GetCurrentPlayerIndex();
            for (int i = 0; i < remainingPlayersAfterCurrent; i++)
            {
                index++;
                if (index == Round.Players.Count())
                    index = 0;
                if (!CurrentTeam().Players.Contains(Round.Players[index]))
                    return true;
            }
            return false;//No opponents will be able to play a card for the remainder of this trick.
        }

        protected Team CurrentTeam()
        {
            return Round.Teams.Where(t => t.Players.Any(p => p == Round.CurrentPlayer)).Single();
        }

        protected int GetCurrentPlayerIndex()
        {
            for (int i = 0; i < Round.Players.Count(); i++)
                if (Round.Players[i] == Round.CurrentPlayer)
                    return i;
            return -1;
        }

        protected Card GetLowestCardStrategic(IEnumerable<Card> cards, Suits pileSuit)
        {
            if (cards.Any(c => c.Suit == pileSuit))//Hand contains card of same suit as pile.
            {
                return GetLowestCard(cards.Where(c => c.Suit == pileSuit));//Return lowest possible pilesuit card.
            }
            else if (cards.Any(c => c.Suit != Round.Trump))//Hand doesn't contain card of same suit as pile and contains non-trump card.
            {
                return GetLowestCard(cards.Where(c => c.Suit != Round.Trump));//Return lowest possible non trump card.
            }
            else//Hand only contains trump cards;
            {
                return GetLowestCard(cards);//return lowest possible card.
            }
        }

        //Leading card's suit is always either trump or pileSuit.
        protected Card LeadingCard()
        {
            if (Round.Pile.Any(c => c.Suit == Round.Trump))
            {
                return GetHighestCard(Round.Pile.Where(c => c.Suit == Round.Trump));//If pile has trump cards, return highest card of trumpsuit.
            }
            else
            {
                return GetHighestCard(Round.Pile.Where(c => c.Suit == Round.Pile[0].Suit));//If pile doesn't have trump cards, return highest card of pilesuit.
            }
        }

        protected bool IsTeamCurrentLeader()
        {
            return CurrentTeam().Players.Any(p => p == Round.PileOwner);
        }

        protected Card HighOrLowCardSelection(IEnumerable<Card> hand, Card leadCard)
        {
            var HighestHandCard = GetHighestCard(hand);
            if (HighestHandCard.Number > leadCard.Number)//Player has higher card than current highest in pile.
            {
                return HighestHandCard;
            }
            else //player doesn't have a higher card than the current highest.
            {
                return GetLowestCard(hand);
            }
        }

        protected Card GetHighestCard(IEnumerable<Card> cards)
        {
            if (cards.Count() == 0)
                return null;
            Card maxCard = cards.First();
            foreach (Card card in cards)
            {
                if (card.Number > maxCard.Number)
                    maxCard = card;
            }
            return maxCard;
        }

        protected Card GetLowestCard(IEnumerable<Card> cards)
        {
            if (cards.Count() == 0)
                return null;
            Card minCard = cards.First();
            foreach (Card card in cards)
            {
                if (card.Number < minCard.Number)
                    minCard = card;
            }
            return minCard;
        }

        protected Card GetMiddleCarld(IEnumerable<Card> cards)
        {
            if (cards.Count() == 0)
                return null;
            int middle = cards.Count() / 2;
            return cards.OrderBy(c => c.Number).Skip(middle).Take(1).Single();
        }



        protected virtual Card MisserieMove()
        {
            var cards = Round.CurrentPlayer.hand.Cards;
            var pile = Round.Pile;
            if (pile.Count == 0)
            {
                return GetLowestCard(cards);
            }
            else
            {
                var pileSuit = pile[0].Suit;
                Team ownteam = CurrentTeam();
                var leadCard = LeadingCard();

                if (ownteam.objective == 0) //Is Miserie player
                {
                    if (cards.Any(c => c.Suit == pileSuit))
                    {
                        if (leadCard.Suit == Round.Trump)
                        {
                            return GetHighestCard(cards.Where(c => c.Suit == pileSuit));
                        }
                        else
                        {
                            return GetHighestCard(cards.Where(c => c.Suit == pileSuit && c.Number < leadCard.Number));//highest of pilesuit below lead.
                        }
                    }
                    else
                    {
                        if (leadCard.Suit == Round.Trump)
                        {
                            if (cards.All(c => c.Suit != Round.Trump))
                                return GetHighestCard(cards);
                            if (cards.All(c => c.Suit == Round.Trump))
                            {
                                if (cards.Any(c => c.Number < leadCard.Number))
                                    return GetHighestCard(cards.Where(c => c.Suit == Round.Trump && c.Number < leadCard.Number));
                                else
                                    return GetLowestCardStrategic(cards, pileSuit);
                            }

                            var HighestCard = GetHighestCard(cards);
                            var HighestTrumpBelowLead = GetHighestCard(cards.Where(c => c.Suit == Round.Trump && c.Number < leadCard.Number));
                            if (HighestCard == HighestTrumpBelowLead)
                                return HighestTrumpBelowLead;
                            if (HighestCard.Number > Numbers.TEN && HighestTrumpBelowLead.Number < HighestCard.Number)
                                return HighestCard;
                            else
                                return
                                    HighestTrumpBelowLead;
                        }
                        else
                        {
                            if (cards.Any(c => c.Suit != Round.Trump))
                            {
                                return GetHighestCard(cards.Where(c => c.Suit != Round.Trump));
                            }
                            else
                                return GetLowestCardStrategic(cards, pileSuit);
                        }
                    }
                }
                else // Plays against miserie.
                {
                    if (IsTeamCurrentLeader())
                    {
                        if (OpposingPlayersLeftInTrick())
                        {
                            return MiserieGetLowOrHighest();
                        }
                        else//Team is the current leader and no miserieplayers still need to play => Play a high card
                        {
                            if (cards.Any(c => c.Suit == pileSuit))
                                return GetHighestCard(cards.Where(c => c.Suit == pileSuit));
                            else if (cards.Any(c => c.Suit == Round.Trump))
                                return GetHighestCard(cards.Where(c => c.Suit == Round.Trump));
                            else
                                return GetHighestCard(cards);
                        }
                    }
                    else//One of the miserie players is the current leader.
                    {
                        return MiserieGetLowOrHighest();
                    }
                }
            }
        }

        //Either play a low card if possible, or play a highcard if not.
        private Card MiserieGetLowOrHighest()
        {
            var cards = Round.CurrentPlayer.hand.Cards;
            var pile = Round.Pile;
            var pileSuit = pile[0].Suit;
            var leadCard = LeadingCard();

            var lowest = GetLowestCardStrategic(cards, pileSuit);
            bool canStayOutOfLead = true;
            if (lowest.Suit == pileSuit)
            {
                if (!(leadCard.Suit == Round.Trump || (leadCard.Suit == pileSuit && lowest.Number < leadCard.Number)))
                    canStayOutOfLead = false;
            }
            else if (lowest.Suit == Round.Trump)//implies trump != pilesuit
            {
                if (!(leadCard.Suit == Round.Trump && lowest.Number < leadCard.Number))
                    canStayOutOfLead = false;
            }

            if (canStayOutOfLead)
            {
                return lowest;
            }
            else
            {
                if (lowest.Suit == pileSuit && lowest.Number <= Numbers.FOUR)
                    return lowest;//In case the lead is two or three, and the player is able to keep it similary low despite raising it.

                if (cards.Any(c => c.Suit == pileSuit))
                    return GetHighestCard(cards.Where(c => c.Suit == pileSuit));
                else
                    return GetHighestCard(cards.Where(c => c.Suit == Round.Trump));
            }
        }
    }

}
