using Microsoft.VisualStudio.TestTools.UnitTesting;
using Whist.GameLogic.ControlEntities;
using System.Collections.Generic;

namespace WhistTest
{
    [TestClass]
    public class BidTest
    {
        private List<Player> players = new List<Player>();

        private void createPlayers()
        {
            for (int i = 1; i < 5; i++)
            {
                players.Add(new Player("P" + i));
            }
        }

        [TestMethod]
        public void Ask_And_Join()
        {
            createPlayers();
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.ASK);
            bool containsInvalidAction = false;
            bool containsValidAction = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.JOIN)
                {
                    containsValidAction = true;
                    break;
                }
            }
            Assert.IsTrue(containsValidAction);
        }
    }
}
