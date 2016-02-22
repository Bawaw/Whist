using System.Collections.Generic;
using System.Linq;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class SimpleGameAI
    {
        public static Card GetMove(Round round)
        {
            var cards = round.CurrentPlayer.hand.Cards;
            var pile = round.Pile;
            var trump = round.Trump;
            var teams = round.Teams;

            if (pile.Count > 0)
            {
                var pileSuit = pile[0].Suit;
                var leadCard = LeadingCard(round);
                if (OpposingPlayersLeftInTrick(round)) //There will still be opposing players after current player's turn.
                {
                    if (cards.Any(c => c.Suit == pileSuit))//Hand contains card of same suit as pile.
                    {
                        if (leadCard.Suit != pileSuit)// Leadingcard is trump while pilesuit isn't trump.
                        {
                            return GetLowestCardStrategic(cards, pileSuit, trump);
                        }
                        else //leadingCard is pilesuit (which may or may not be trump)
                        {
                            var cardsOfSuit = cards.Where(c => c.Suit == pileSuit);
                            if (IsTeamCurrentLeader(round))//If the team is winning, see if an opponent could possible outplay team.
                            {
                                bool possibleHigherCard = false;
                                for (int i = (int)leadCard.Number; i <= (int)Numbers.ACE; i++)
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
                    else//Hand contains no card of the same suit as pile.
                    {
                        if (cards.Any(c => c.Suit == trump))//Hand has trump card.
                        {
                            var cardsOfTrump = cards.Where(c => c.Suit == trump);
                            if (IsTeamCurrentLeader(round))
                            {//Team is winning
                                if (leadCard.Suit == trump)
                                {
                                    return GetLowestCardStrategic(cards, pileSuit, trump);
                                }
                                else
                                {
                                    if (leadCard.Number == Numbers.ACE)
                                        return GetLowestCardStrategic(cards, pileSuit, trump);
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
                                        return GetLowestCardStrategic(cards, pileSuit, trump);
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
                }
                else //There won't be opponents after current player's turn.
                {
                    if (IsTeamCurrentLeader(round))//AI's team is already winning.
                    {
                        return GetLowestCardStrategic(cards, pileSuit, trump);
                    }
                    else //AI's team is not already winning.
                    {
                        //Give lowest winning card.
                        if (cards.Any(c => c.Suit == pileSuit))//Hand contains card of same suit as pile.
                        {
                            if (leadCard.Suit != pileSuit)// Leadingcard is trump while pilesuit isn't trump.
                            {
                                return GetLowestCardStrategic(cards, pileSuit, trump);
                            }
                            else //leadingCard is pilesuit (which may or may not be trump)
                            {
                                var higherCardsOfPilesuit = cards.Where(c => c.Suit == pileSuit && c.Number > leadCard.Number);
                                if (higherCardsOfPilesuit.Count() > 0)
                                    return GetLowestCard(higherCardsOfPilesuit);//player has higher cards than leading card and gives the lowest of them.
                                else
                                    return GetLowestCardStrategic(cards, pileSuit, trump);//player hasn't got higher cards than leading card so returns lowest card.
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
                                        return GetLowestCardStrategic(cards, pileSuit, trump);//player hasn't got higher cards than leading card so returns lowest card.
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
            }
            else//AI Starts Pile.
            {
                return GetHighestCard(cards);
            }
        }

        private static bool OpposingPlayersLeftInTrick(Round round)
        {
            int remainingPlayersAfterCurrent = 3 - round.Pile.Count();
            int index = GetCurrentPlayerIndex(round);
            for (int i = 0; i < remainingPlayersAfterCurrent; i++)
            {
                index++;
                if (index == round.Players.Count())
                    index = 0;
                if (!CurrentTeam(round).Players.Contains(round.Players[index]))
                    return true;
            }
            return false;//No opponents will be able to play a card for the remainder of this trick.
        }

        private static Team CurrentTeam(Round round)
        {
            return round.Teams.Where(t => t.Players.Any(p => p == round.CurrentPlayer)).Single();
        }

        private static int GetCurrentPlayerIndex(Round round)
        {
            for (int i = 0; i < round.Players.Count(); i++)
                if (round.Players[i] == round.CurrentPlayer)
                    return i;
            return -1;
        }

        private static Card GetLowestCardStrategic(IEnumerable<Card> cards, Suits pileSuit, Suits trump)
        {
            if (cards.Any(c => c.Suit == pileSuit))//Hand contains card of same suit as pile.
            {
                return GetLowestCard(cards.Where(c => c.Suit == pileSuit));//Return lowest possible pilesuit card.
            }
            else if (cards.Any(c => c.Suit != trump))//Hand doesn't contain card of same suit as pile and contains non-trump card.
            {
                return GetLowestCard(cards.Where(c => c.Suit != trump));//Return lowest possible non trump card.
            }
            else//Hand only contains trump cards;
            {
                return GetLowestCard(cards);//return lowest possible card.
            }
        }

        //Leading card's suit is always either trump or pileSuit.
        private static Card LeadingCard(Round round)
        {
            if (round.Pile.Any(c => c.Suit == round.Trump))
            {
                return GetHighestCard(round.Pile.Where(c => c.Suit == round.Trump));//If pile has trump cards, return highest card of trumpsuit.
            }
            else
            {
                return GetHighestCard(round.Pile.Where(c => c.Suit == round.Pile[0].Suit));//If pile doesn't have trump cards, return highest card of pilesuit.
            }
        }

        private static bool IsTeamCurrentLeader(Round round)
        {
            return CurrentTeam(round).Players.Any(p => p == round.PileOwner);
        }

        private static Card HighOrLowCardSelection(IEnumerable<Card> hand, Card leadCard)
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

        private static Card GetHighestCard(IEnumerable<Card> cards)
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

        private static Card GetLowestCard(IEnumerable<Card> cards)
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
    }
}
