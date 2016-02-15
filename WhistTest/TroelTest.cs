using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Whist.GameLogic.ControlEntities;
using Whist.GameLogic;
using Whist.GameLogic.GameCases;

namespace WhistTest
{
    [TestClass]
    public class TroelTest
    {
        private List<Player> players = new List<Player>();

        //TODO: initialize en cleanup checken hoe dat werkt, en als trump voorzien is in GameCase, testen op trump!

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
        public void Test_Troel_When_3_Aces()
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
            SpecialGameCase troel = new Troel();
            Team[] teams = troel.Teams(players.ToArray());
            Assert.IsTrue(troel.AfterDealCheck(players.ToArray()));//troel
            Assert.AreEqual(players[0], teams[0].Players[0]);//troelplayers, player with 3 aces
            Assert.AreEqual(players[2], teams[0].Players[1]);//troelplayers, player with 4th ace
            Assert.AreEqual(players[1], teams[1].Players[0]);//otherplayers
            Assert.AreEqual(players[3], teams[1].Players[1]);//otherplayers
        }

        [TestMethod]
        public void Test_Troel_When_4_Aces_And_No_King_Of_Hearts()
        {
            createPlayers();//de attributen van de init en cleanup methodes werken blijkbaar niet...
            IList<Card> aces = new List<Card>() { new Card(1, 14), new Card(2, 14), new Card(3, 14), new Card(4, 14) };
            players[0].hand.AddCard(aces[0]);
            players[0].hand.AddCard(aces[1]);
            players[0].hand.AddCard(aces[2]);
            players[0].hand.AddCard(aces[3]);
            for (int i = 2; i < 11; i++)
            {
                players[0].hand.AddCard(new Card(1, i));
            }
            for (int i = 11; i < 14; i++)
            {
                players[1].hand.AddCard(new Card(1, i));
            }
            for (int i = 2; i < 12; i++)
            {
                players[1].hand.AddCard(new Card(2, i));
            }
            players[2].hand.AddCard(new Card(2, 12));
            players[2].hand.AddCard(new Card(2, 13));
            for (int i = 2; i < 13; i++)
            {
                players[2].hand.AddCard(new Card(3, i));
            }
            players[3].hand.AddCard(new Card(3, 13));
            for (int i = 2; i < 14; i++)
            {
                players[3].hand.AddCard(new Card(4, i));
            }
            foreach (Player player in players)
            {
                Console.Write("Player " + player.name + " cards: ");
                foreach (Card card in player.hand.Cards)
                {
                    Console.Write(card.ToString() + ", ");
                }
                Console.WriteLine();
            }
            SpecialGameCase troel = new Troel();
            Team[] teams = troel.Teams(players.ToArray());
            Assert.IsTrue(troel.AfterDealCheck(players.ToArray()));//troel
            Assert.AreEqual(players[0], teams[0].Players[0]);//troelplayers, player with 4 aces
            Assert.AreEqual(players[1], teams[0].Players[1]);//troelplayers, player with highest heart after first player(this case, king of hearts)
            Assert.AreEqual(players[2], teams[1].Players[0]);//otherplayers
            Assert.AreEqual(players[3], teams[1].Players[1]);//otherplayers
        }

        [TestMethod]
        public void Test_Troel_When_4_Aces_And_King_Of_Hearts()
        {
            createPlayers();//de attributen van de init en cleanup methodes werken blijkbaar niet...
            IList<Card> aces = new List<Card>() { new Card(1, 14), new Card(2, 14), new Card(3, 14), new Card(4, 14) };
            players[0].hand.AddCard(aces[0]);
            players[0].hand.AddCard(aces[1]);
            players[0].hand.AddCard(aces[2]);
            players[0].hand.AddCard(aces[3]);
            players[0].hand.AddCard(new Card(1, 13));
            for (int i = 2; i < 10; i++)
            {
                players[0].hand.AddCard(new Card(1, i));
            }
            for (int i = 10; i < 13; i++)
            {
                players[1].hand.AddCard(new Card(1, i));
            }
            for (int i = 2; i < 12; i++)
            {
                players[1].hand.AddCard(new Card(2, i));
            }
            players[2].hand.AddCard(new Card(2, 12));
            players[2].hand.AddCard(new Card(2, 13));
            for (int i = 2; i < 13; i++)
            {
                players[2].hand.AddCard(new Card(3, i));
            }
            players[3].hand.AddCard(new Card(3, 13));
            for (int i = 2; i < 14; i++)
            {
                players[3].hand.AddCard(new Card(4, i));
            }
            foreach (Player player in players)
            {
                Console.Write("Player " + player.name + " cards: ");
                foreach (Card card in player.hand.Cards)
                {
                    Console.Write(card.ToString() + ", ");
                }
                Console.WriteLine();
            }
            SpecialGameCase troel = new Troel();
            Team[] teams = troel.Teams(players.ToArray());
            Assert.IsTrue(troel.AfterDealCheck(players.ToArray()));//troel
            Assert.AreEqual(players[0], teams[0].Players[0]);//troelplayers, player with 4 aces
            Assert.AreEqual(players[1], teams[0].Players[1]);//troelplayers, player with highest heart after first player(this case, queen of hearts)
            Assert.AreEqual(players[2], teams[1].Players[0]);//otherplayers
            Assert.AreEqual(players[3], teams[1].Players[1]);//otherplayers
        }

        [TestMethod]
        public void Test_Troel_When_4_Aces_And_9_Other_Highest_Hearts()
        {
            createPlayers();//de attributen van de init en cleanup methodes werken blijkbaar niet...
            IList<Card> aces = new List<Card>() { new Card(1, 14), new Card(2, 14), new Card(3, 14), new Card(4, 14) };
            players[0].hand.AddCard(aces[0]);
            players[0].hand.AddCard(aces[1]);
            players[0].hand.AddCard(aces[2]);
            players[0].hand.AddCard(aces[3]);
            for (int i = 5; i < 14; i++)
            {
                players[0].hand.AddCard(new Card(1, i));
            }
            for (int i = 2; i < 5; i++)
            {
                players[1].hand.AddCard(new Card(1, i));
            }
            for (int i = 2; i < 12; i++)
            {
                players[1].hand.AddCard(new Card(2, i));
            }
            players[2].hand.AddCard(new Card(2, 12));
            players[2].hand.AddCard(new Card(2, 13));
            for (int i = 2; i < 13; i++)
            {
                players[2].hand.AddCard(new Card(3, i));
            }
            players[3].hand.AddCard(new Card(3, 13));
            for (int i = 2; i < 14; i++)
            {
                players[3].hand.AddCard(new Card(4, i));
            }
            foreach (Player player in players)
            {
                Console.Write("Player " + player.name + " cards: ");
                foreach (Card card in player.hand.Cards)
                {
                    Console.Write(card.ToString() + ", ");
                }
                Console.WriteLine();
            }
            SpecialGameCase troel = new Troel();
            Team[] teams = troel.Teams(players.ToArray());
            Assert.IsTrue(troel.AfterDealCheck(players.ToArray()));//troel
            Assert.AreEqual(players[0], teams[0].Players[0]);//troelplayers, player with 4 aces
            Assert.AreEqual(players[1], teams[0].Players[1]);//troelplayers, player with highest heart after first player(this case, 4 of hearts)
            Assert.AreEqual(players[2], teams[1].Players[0]);//otherplayers
            Assert.AreEqual(players[3], teams[1].Players[1]);//otherplayers
        }

        [TestMethod]
        public void Test_No_Troel_When_Nobody_With_3_Or_4_Aces()
        {
            createPlayers();//de attributen van de init en cleanup methodes werken blijkbaar niet...
            for (int i = 2; i < 15; i++)
            {
                players[0].hand.AddCard(new Card(1, i));
            }
            for (int i = 2; i < 15; i++)
            {
                players[1].hand.AddCard(new Card(2, i));
            }
            for (int i = 2; i < 15; i++)
            {
                players[2].hand.AddCard(new Card(3, i));
            }
            for (int i = 2; i < 15; i++)
            {
                players[3].hand.AddCard(new Card(4, i));
            }
            foreach (Player player in players)
            {
                Console.Write("Player " + player.name + " cards: ");
                foreach (Card card in player.hand.Cards)
                {
                    Console.Write(card.ToString() + ", ");
                }
                Console.WriteLine();
            }
            SpecialGameCase troel = new Troel();
            Team[] teams = troel.Teams(players.ToArray());
            Assert.IsFalse(troel.AfterDealCheck(players.ToArray()));//no troel
        }
    }
}
