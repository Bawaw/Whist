using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.AIs;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace WhistTest
{
    [TestClass]
    public class MemoryAITest
    {
        private GameManager InitialiseTest()
        {
            var players = new Player[]
            {
                new Player("C1", 0),
                new Player("C2", 1),
                new Player("C3", 2),
                new Player("C4", 3)
            };

            var gameManager = new GameManager(players, 13, new AIBidType[]
            {
                AIBidType.BASIC,
                AIBidType.BASIC,
                AIBidType.BASIC,
                AIBidType.BASIC
            }, new AIGameType[]
            {
                AIGameType.MEMORY,
                AIGameType.MEMORY,
                AIGameType.MEMORY,
                AIGameType.MEMORY,
            });

            return gameManager;
        }

        private void SetHandsToTestConfiguration(Player[] players)
        {
            players[0].hand.Cards.Clear();
            players[1].hand.Cards.Clear();
            players[2].hand.Cards.Clear();
            players[3].hand.Cards.Clear();

            players[0].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.KING));
            players[0].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.JACK));
            players[0].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.THREE));
            players[0].hand.Cards.Add(new Card(Suits.SPADES, Numbers.THREE));
            players[1].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.FOUR));
            players[1].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.NINE));
            players[1].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.SEVEN));
            players[1].hand.Cards.Add(new Card(Suits.SPADES, Numbers.FOUR));
            players[2].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.ACE));
            players[2].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.FIVE));
            players[2].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.QUEEN));
            players[2].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.TWO));
            players[3].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.TEN));
            players[3].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.EIGHT));
            players[3].hand.Cards.Add(new Card(Suits.HEARTS, Numbers.SIX));
            players[3].hand.Cards.Add(new Card(Suits.SPADES, Numbers.FIVE));
        }

        [TestMethod]
        public void MemoryTestConserveGoodCard()
        {
            var gameManager = InitialiseTest();

            var players = gameManager.Players;

            SetHandsToTestConfiguration(players);
            gameManager.Round.BiddingDoAction(Whist.GameLogic.ControlEntities.Action.ASK);
            gameManager.Round.BiddingDoAction(Whist.GameLogic.ControlEntities.Action.PASS);
            gameManager.Round.BiddingDoAction(Whist.GameLogic.ControlEntities.Action.JOIN);
            gameManager.Round.BiddingDoAction(Whist.GameLogic.ControlEntities.Action.PASS);
            //Teans are player 1 and player 3 vs player 2 and player 4.

            gameManager.Round.EndBiddingRound();

            Assert.IsFalse(gameManager.Round.InBiddingPhase);
            Assert.IsTrue(gameManager.Round.InTrickPhase);

            var cardToPlay = players[0].hand.Cards[0];//King of hearts.
            gameManager.Round.PlayCard(cardToPlay);
            gameManager.GetAI(players[2]).ProcessOtherPlayerCard(players[0], cardToPlay);//Player 3 should now know king of hearts was played.

            gameManager.Round.PlayCard(gameManager.GetAI(players[1]).GetMove());//Player 2 plays a card.

            var cardPlayer3 = gameManager.GetAI(players[2]).GetMove();
            Assert.IsFalse(cardPlayer3.Number == Numbers.ACE);//Players three's card should not be the ace.
            gameManager.Round.PlayCard(cardPlayer3);

            gameManager.Round.PlayCard(gameManager.GetAI(players[3]).GetMove());//Player 4 plays a card.

            gameManager.Round.EndTrick();

            cardToPlay = players[0].hand.Cards[0];//Jack of hearts.
            gameManager.Round.PlayCard(cardToPlay);
            gameManager.GetAI(players[2]).ProcessOtherPlayerCard(players[0], cardToPlay);

            gameManager.Round.PlayCard(gameManager.GetAI(players[1]).GetMove());//Player 2 plays a card.

            cardPlayer3 = gameManager.GetAI(players[2]).GetMove();
            Assert.IsFalse(cardPlayer3.Number == Numbers.ACE);//Players three's card should still not be the ace.
            gameManager.Round.PlayCard(cardPlayer3);

            gameManager.Round.PlayCard(gameManager.GetAI(players[3]).GetMove());//Player 4 plays a card.

        }

        private void PlayRound(GameManager gameManager)
        {
            Round round = gameManager.Round;

            while (round.InBiddingPhase)
            {
                var action = gameManager.GetAI(round.CurrentPlayer).GetAction();
                round.BiddingDoAction(action);
            }
            round.EndBiddingRound();

            while (round.InTrickPhase)
            {
                while (round.TrickInProgress)
                {
                    var card = gameManager.GetAI(round.CurrentPlayer).GetMove();
                    foreach (Player player in gameManager.NonHumanPlayers)
                    {
                        gameManager.GetAI(player).ProcessOtherPlayerCard(round.CurrentPlayer, card);
                    }
                    round.PlayCard(card);
                }
                round.EndTrick();
            }
            round.EndTricksRound();
        }

        [TestMethod]
        public void ResetMemoryOnNewRound()
        {
            var gameManager = InitialiseTest();
            PlayRound(gameManager);

            gameManager.StartNewRound();

            var players = gameManager.Players;

            SetHandsToTestConfiguration(players);
            gameManager.Round.BiddingDoAction(Whist.GameLogic.ControlEntities.Action.ASK);
            gameManager.Round.BiddingDoAction(Whist.GameLogic.ControlEntities.Action.PASS);
            gameManager.Round.BiddingDoAction(Whist.GameLogic.ControlEntities.Action.JOIN);
            gameManager.Round.BiddingDoAction(Whist.GameLogic.ControlEntities.Action.PASS);
            //Teans are player 1 and player 3 vs player 2 and player 4.

            gameManager.Round.EndBiddingRound();

            Assert.IsFalse(gameManager.Round.InBiddingPhase);
            Assert.IsTrue(gameManager.Round.InTrickPhase);

            var cardToPlay = players[0].hand.Cards[1];//Jack of hearts.
            gameManager.Round.PlayCard(cardToPlay);
            gameManager.GetAI(players[2]).ProcessOtherPlayerCard(players[0], cardToPlay);

            gameManager.Round.PlayCard(gameManager.GetAI(players[1]).GetMove());//Player 2 plays a card.

            var cardPlayer3 = gameManager.GetAI(players[2]).GetMove();
            Assert.IsTrue(cardPlayer3.Number == Numbers.ACE);//Players three's card should be the ace, unless it still thinks that King of Hearts got played (which happened last round).
            gameManager.Round.PlayCard(cardPlayer3);
        }
    }
}
