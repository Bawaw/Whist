using Microsoft.VisualStudio.TestTools.UnitTesting;
using Whist.GameLogic.ControlEntities;
using System.Collections.Generic;
using Whist.GameLogic.GameCases;
using Whist.GameLogic;

namespace WhistTest
{
    [TestClass]
    public class BidTest
    {
        private List<Player> players = new List<Player>();

        //TODO: actions testen voor third en fourth player
        //TODO: alone(after someone asked and all the rest passed), and no choose trump when solo slim(this is done, only pass shouldn't be possible...)

        private void createPlayers()
        {
            for (int i = 1; i < 5; i++)
            {
                players.Add(new Player("P" + i));
            }
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_TROEL()
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
            for (int i = 12; i < 14; i++)
            {
                players[1].hand.AddCard(new Card(1, i));
            }
            for (int i = 2; i < 13; i++)
            {
                players[1].hand.AddCard(new Card(2, i));
            }
            players[2].hand.AddCard(aces[3]);
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
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            bool containsActionSOLO = false;
            bool containsActionSOLOSLIM = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK || action == Action.JOIN || action == Action.ABONDANCE || action == Action.MISERIE)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.PASS)
                {
                    containsActionPASS = true;
                }
                else if (action == Action.SOLO)
                {
                    containsActionSOLO = true;
                }
                else if (action == Action.SOLOSLIM)
                {
                    containsActionSOLOSLIM = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
            Assert.IsTrue(containsActionSOLO);
            Assert.IsTrue(containsActionSOLOSLIM);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_PASS_And_No_TROEL()
        {
            SpecialGameCase troel = new Troel();
            do
            {
                createPlayers();    //Need to be sure that it isn't troel, otherwise, if it's troel, tests will fail
            } while (troel.AfterDealCheck(players.ToArray()));
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.PASS);
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            bool containsActionASK = false;
            bool containsActionABONDANCE = false;
            bool containsActionMISERIE = false;
            bool containsActionSOLO = false;
            bool containsActionSOLOSLIM = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.JOIN)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.PASS)
                {
                    containsActionPASS = true;
                }
                else if (action == Action.ASK)
                {
                    containsActionASK = true;
                }
                else if (action == Action.ABONDANCE)
                {
                    containsActionABONDANCE = true;
                }
                else if (action == Action.MISERIE)
                {
                    containsActionMISERIE = true;
                }
                else if (action == Action.SOLO)
                {
                    containsActionSOLO = true;
                }
                else if (action == Action.SOLOSLIM)
                {
                    containsActionSOLOSLIM = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
            Assert.IsTrue(containsActionASK);
            Assert.IsTrue(containsActionABONDANCE);
            Assert.IsTrue(containsActionMISERIE);
            Assert.IsTrue(containsActionSOLO);
            Assert.IsTrue(containsActionSOLOSLIM);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_ASK_And_No_TROEL()
        {
            SpecialGameCase troel = new Troel();
            do
            {
                createPlayers();    //Need to be sure that it isn't troel, otherwise, if it's troel, tests will fail
            } while (troel.AfterDealCheck(players.ToArray()));
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.ASK);
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            bool containsActionJOIN = false;
            bool containsActionABONDANCE = false;
            bool containsActionMISERIE = false;
            bool containsActionSOLO = false;
            bool containsActionSOLOSLIM = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if(action == Action.PASS)
                {
                    containsActionPASS = true;
                }
                else if (action == Action.JOIN)
                {
                    containsActionJOIN = true;
                }
                else if (action == Action.ABONDANCE)
                {
                    containsActionABONDANCE = true;
                }
                else if(action == Action.MISERIE)
                {
                    containsActionMISERIE = true;
                }
                else if (action == Action.SOLO)
                {
                    containsActionSOLO = true;
                }
                else if (action == Action.SOLOSLIM)
                {
                    containsActionSOLOSLIM = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
            Assert.IsTrue(containsActionJOIN);
            Assert.IsTrue(containsActionABONDANCE);
            Assert.IsTrue(containsActionMISERIE);
            Assert.IsTrue(containsActionSOLO);
            Assert.IsTrue(containsActionSOLOSLIM);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_ASK_And_JOIN_And_No_TROEL()
        {
            SpecialGameCase troel = new Troel();
            do
            {
                createPlayers();    //Need to be sure that it isn't troel, otherwise, if it's troel, tests will fail
            } while (troel.AfterDealCheck(players.ToArray()));
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.ASK);
            dealAndBid.DoAction(Action.JOIN);
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            bool containsActionABONDANCE = false;
            bool containsActionMISERIE = false;
            bool containsActionSOLO = false;
            bool containsActionSOLOSLIM = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK || action == Action.JOIN)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.PASS)
                {
                    containsActionPASS = true;
                }
                else if (action == Action.ABONDANCE)
                {
                    containsActionABONDANCE = true;
                }
                else if (action == Action.MISERIE)
                {
                    containsActionMISERIE = true;
                }
                else if (action == Action.SOLO)
                {
                    containsActionSOLO = true;
                }
                else if (action == Action.SOLOSLIM)
                {
                    containsActionSOLOSLIM = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
            Assert.IsTrue(containsActionABONDANCE);
            Assert.IsTrue(containsActionMISERIE);
            Assert.IsTrue(containsActionSOLO);
            Assert.IsTrue(containsActionSOLOSLIM);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_ABONDANCE_And_No_TROEL()
        {
            SpecialGameCase troel = new Troel();
            do
            {
                createPlayers();    //Need to be sure that it isn't troel, otherwise, if it's troel, tests will fail
            } while (troel.AfterDealCheck(players.ToArray()));
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.ABONDANCE);
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            bool containsActionMISERIE = false;
            bool containsActionSOLO = false;
            bool containsActionSOLOSLIM = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK || action == Action.JOIN || action == Action.ABONDANCE)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if(action == Action.PASS)
                {
                    containsActionPASS = true;
                }
                else if (action == Action.MISERIE)
                {
                    containsActionMISERIE = true;
                }
                else if (action == Action.SOLO)
                {
                    containsActionSOLO = true;
                }
                else if (action == Action.SOLOSLIM)
                {
                    containsActionSOLOSLIM = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
            Assert.IsTrue(containsActionMISERIE);
            Assert.IsTrue(containsActionSOLO);
            Assert.IsTrue(containsActionSOLOSLIM);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_ABONDANCE_And_PASS_And_PASS_And_PASS_And_No_TROEL()
        {
            SpecialGameCase troel = new Troel();
            do
            {
                createPlayers();    //Need to be sure that it isn't troel, otherwise, if it's troel, tests will fail
            } while (troel.AfterDealCheck(players.ToArray()));
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.ABONDANCE);
            dealAndBid.DoAction(Action.PASS);
            dealAndBid.DoAction(Action.PASS);
            dealAndBid.DoAction(Action.PASS);
            bool containsInvalidAction = false;
            bool containsActionHEARTS = false;
            bool containsActionCLUBS = false;
            bool containsActionDIAMONDS = false;
            bool containsActionSPADES = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.PASS || action == Action.ASK || action == Action.JOIN || action == Action.ABONDANCE || action == Action.MISERIE || action == Action.SOLO || action == Action.SOLOSLIM)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.HEARTS)
                {
                    containsActionHEARTS = true;
                }
                else if (action == Action.CLUBS)
                {
                    containsActionCLUBS = true;
                }
                else if (action == Action.DIAMONDS)
                {
                    containsActionDIAMONDS = true;
                }
                else if (action == Action.SPADES)
                {
                    containsActionSPADES = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionHEARTS);
            Assert.IsTrue(containsActionCLUBS);
            Assert.IsTrue(containsActionDIAMONDS);
            Assert.IsTrue(containsActionSPADES);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_MISERIE_And_No_TROEL()
        {
            SpecialGameCase troel = new Troel();
            do
            {
                createPlayers();    //Need to be sure that it isn't troel, otherwise, if it's troel, tests will fail
            } while (troel.AfterDealCheck(players.ToArray()));
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.MISERIE);
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            bool containsActionMISERIE = false;
            bool containsActionSOLO = false;
            bool containsActionSOLOSLIM = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK || action == Action.JOIN || action == Action.ABONDANCE)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if(action == Action.PASS)
                {
                    containsActionPASS = true;
                }
                else if (action == Action.MISERIE)
                {
                    containsActionMISERIE = true;
                }
                else if (action == Action.SOLO)
                {
                    containsActionSOLO = true;
                }
                else if (action == Action.SOLOSLIM)
                {
                    containsActionSOLOSLIM = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
            Assert.IsTrue(containsActionMISERIE);
            Assert.IsTrue(containsActionSOLO);
            Assert.IsTrue(containsActionSOLOSLIM);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_MISERIE_And_MISERIE_And_No_TROEL()
        {
            SpecialGameCase troel = new Troel();
            do
            {
                createPlayers();    //Need to be sure that it isn't troel, otherwise, if it's troel, tests will fail
            } while (troel.AfterDealCheck(players.ToArray()));
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.MISERIE);
            dealAndBid.DoAction(Action.MISERIE);
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            bool containsActionMISERIE = false;
            bool containsActionSOLO = false;
            bool containsActionSOLOSLIM = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK || action == Action.JOIN || action == Action.ABONDANCE)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.PASS)
                {
                    containsActionPASS = true;
                }
                else if (action == Action.MISERIE)
                {
                    containsActionMISERIE = true;
                }
                else if (action == Action.SOLO)
                {
                    containsActionSOLO = true;
                }
                else if (action == Action.SOLOSLIM)
                {
                    containsActionSOLOSLIM = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
            Assert.IsTrue(containsActionMISERIE);
            Assert.IsTrue(containsActionSOLO);
            Assert.IsTrue(containsActionSOLOSLIM);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_MISERIE_And_MISERIE_And_MISERIE_And_No_TROEL()
        {
            SpecialGameCase troel = new Troel();
            do
            {
                createPlayers();    //Need to be sure that it isn't troel, otherwise, if it's troel, tests will fail
            } while (troel.AfterDealCheck(players.ToArray()));
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.MISERIE);
            dealAndBid.DoAction(Action.MISERIE);
            dealAndBid.DoAction(Action.MISERIE);
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            bool containsActionSOLO = false;
            bool containsActionSOLOSLIM = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK || action == Action.JOIN || action == Action.ABONDANCE || action == Action.MISERIE)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.PASS)
                {
                    containsActionPASS = true;
                }
                else if (action == Action.SOLO)
                {
                    containsActionSOLO = true;
                }
                else if (action == Action.SOLOSLIM)
                {
                    containsActionSOLOSLIM = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
            Assert.IsTrue(containsActionSOLO);
            Assert.IsTrue(containsActionSOLOSLIM);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_SOLO()
        {
            createPlayers();    //Independent of whether it's troel or not
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.SOLO);
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            bool containsActionSOLOSLIM = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK || action == Action.JOIN || action == Action.ABONDANCE || action == Action.MISERIE || action == Action.SOLO)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.PASS)
                {
                    containsActionPASS = true;
                }
                else if (action == Action.SOLOSLIM)
                {
                    containsActionSOLOSLIM = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
            Assert.IsTrue(containsActionSOLOSLIM);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_SOLO_And_PASS_And_PASS_And_PASS()
        {
            createPlayers();    //Independent of whether it's troel or not
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.SOLO);
            dealAndBid.DoAction(Action.PASS);
            dealAndBid.DoAction(Action.PASS);
            dealAndBid.DoAction(Action.PASS);
            bool containsInvalidAction = false;
            bool containsActionHEARTS = false;
            bool containsActionCLUBS = false;
            bool containsActionDIAMONDS = false;
            bool containsActionSPADES = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.PASS || action == Action.ASK || action == Action.JOIN || action == Action.ABONDANCE || action == Action.MISERIE || action == Action.SOLO || action == Action.SOLOSLIM)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.HEARTS)
                {
                    containsActionHEARTS = true;
                }
                else if (action == Action.CLUBS)
                {
                    containsActionCLUBS = true;
                }
                else if (action == Action.DIAMONDS)
                {
                    containsActionDIAMONDS = true;
                }
                else if (action == Action.SPADES)
                {
                    containsActionSPADES = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionHEARTS);
            Assert.IsTrue(containsActionCLUBS);
            Assert.IsTrue(containsActionDIAMONDS);
            Assert.IsTrue(containsActionSPADES);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_SOLOSLIM()
        {
            createPlayers();    //Independent of whether it's troel or not
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.SOLOSLIM);
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK || action == Action.JOIN || action == Action.ABONDANCE || action == Action.MISERIE || action == Action.SOLO || action == Action.SOLOSLIM)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.PASS)
                {
                    containsActionPASS = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
        }

        [TestMethod]
        public void Test_Valid_And_Invalid_Actions_When_SOLOSLIM_And_PASS_And_PASS_And_PASS()
        {
            createPlayers();    //Independent of whether it's troel or not
            DealAndBidNormal dealAndBid = new DealAndBidNormal(players.ToArray());
            dealAndBid.DoAction(Action.SOLOSLIM);
            dealAndBid.DoAction(Action.PASS);
            dealAndBid.DoAction(Action.PASS);
            dealAndBid.DoAction(Action.PASS);
            bool containsInvalidAction = false;
            bool containsActionPASS = false;
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.ASK || action == Action.JOIN || action == Action.ABONDANCE || action == Action.MISERIE || action == Action.SOLO || action == Action.SOLOSLIM || action == Action.HEARTS || action == Action.CLUBS || action == Action.DIAMONDS || action == Action.SPADES)
                {
                    containsInvalidAction = true;
                    break;
                }
            }
            foreach (Action action in dealAndBid.GetPossibleActions())
            {
                if (action == Action.PASS) //actually this should be impossible(even if it doesn't really pass)
                {
                    containsActionPASS = true;
                }
            }
            Assert.IsFalse(containsInvalidAction);
            Assert.IsTrue(containsActionPASS);
        }
    }
}
