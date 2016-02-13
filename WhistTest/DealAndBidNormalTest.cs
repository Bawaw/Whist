using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Whist.GameLogic.ControlEntities;
using Whist.GameLogic;

namespace WhistTest
{
    [TestClass]
    public class DealAndBidNormalTest
    {
        private IList<Player> players = new List<Player>();

        [TestInitialize]
        private void createPlayers()
        {
            for (int i = 1; i < 5; i++)
            {
                players.Add(new Player("P" + i));
            }
        }

        [TestCleanup]
        private void removePlayers()
        {
            foreach (Player player in players)
            {
                players.Remove(player);
            }
        }

        [TestMethod]
        public void Test_Troel_When_3_aces()
        {
            createPlayers();//de attributen van de init en cleanup methodes werken blijkbaar niet...
            IList<Card> aces = new List<Card>() { new Card(1, 14), new Card(2, 14), new Card(3, 14), new Card(4, 14) };
            players[0].hand.AddCard(aces[0]);
            players[0].hand.AddCard(aces[1]);
            players[0].hand.AddCard(aces[2]);
            for (int i = 2; i < 12; i++)
            {
                players[0].hand.AddCard(new Card(1, i));
            }
            for(int i = 12; i < 14; i++)
            {
                players[1].hand.AddCard(new Card(1, i));
            }
            for (int i = 2; i < 13; i++)
            {
                players[1].hand.AddCard(new Card(2, i));
            }
            players[2].hand.AddCard(aces[3]);
            players[2].hand.AddCard(new Card(2, 13));
            for(int i = 2; i < 13; i++)
            {
                players[2].hand.AddCard(new Card(3, i));
            }
            players[3].hand.AddCard(new Card(3, 13));
            for(int i=2; i<14; i++)
            {
                players[3].hand.AddCard(new Card(4, i));
            }
            foreach(Player player in players)
            {
                Console.Write("Player " + player.name + " cards: ");
                foreach (Card card in player.hand.Cards)
                {
                    Console.Write(card.ToString() + ", ");
                }
                Console.WriteLine();
            }
            //...
        }
    }
}
