using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    class DealAndBidNormal
    {
        public Round round;

        public DealAndBidNormal(Round round)
        {
            this.round = round;
            CurrentPlayer = round.players[0];
            passedPlayers = new Dictionary<Player, bool>();
            foreach (var player in round.players)
                passedPlayers.Add(player, false);
        }

        //Deal Cards and set initial Trump, also check for Troel
        private void DealCards()
        {
            DeckCollection cardCollection = new DeckCollection();
            cardCollection.initialise();
            cardCollection.shuffle();
            Card firstCard = cardCollection.peep();
            int nCards = cardCollection.Count / round.players.Length;
            foreach (var player in round.players)
                player.hand.AddCards(cardCollection.Draw(nCards));
            round.Trump = firstCard.Suit;

            if (CheckForTroel())
                CurrentPlayer = null;
        }

        /*
        Check if any of the player has three aces.
        If so, troel.
        */
        private bool CheckForTroel()
        {
            //TODO

            if (false)//No Troel
                return false;
            else//Troel
            {
                playerA = null;//Player with most aces
                playerB = null;//Team member;
                return true;
            }
        }

        public bool InBiddingPhase
        {
            get { return CurrentPlayer != null; }
        }

        //Bidding
        //Let each player in turn make a decision

        Player playerA; //PlayerA is, when no special: asking/alone player, or when special miserie: (possibly) one of miserie players
        Player playerB; //PlayerA is, when no special: joining/alone player, or when special miserie: (possibly) one of miserie players
        Player HighestSpecialPlayer;
        Action currentSpecial = 0;
        Dictionary<Player, bool> passedPlayers;
        public const int lowestSpecial = 4;

        public Player CurrentPlayer
        {
            get;
            private set;
        }

        public IEnumerable<Action> GetPossibleActions()
        {
            var possibleActions = new HashSet<Action>();
            possibleActions.Add(Action.PASS);
            if (currentSpecial == 0)
            {
                if (playerA == null)
                {
                    possibleActions.Add(Action.ASK);
                }
                else
                {
                    if (playerB == null)
                        possibleActions.Add(Action.JOIN);
                }

                if (playerA == CurrentPlayer && playerB == null)
                {
                    possibleActions.Add(Action.ALONE);
                }
                else
                {
                    for (int i = lowestSpecial; i < Enum.GetValues(typeof(Action)).Length; i++)
                    {
                        possibleActions.Add((Action)i);
                    }
                }

            }
            else
            {
                for (int i = (int)currentSpecial + 1; i < Enum.GetValues(typeof(Action)).Length; i++)
                {
                    possibleActions.Add((Action)i);
                }
                if (currentSpecial == Action.MISERIE && playerB == null)
                    possibleActions.Add(Action.MISERIE);
            }

            return possibleActions;
        }


        public bool DoAction(Action action)
        {
            if (!GetPossibleActions().Contains(action))
                return false;

            switch (action)
            {
                case Action.PASS:
                    {
                        passedPlayers.Add(CurrentPlayer, true);
                        return true;
                    }
                case Action.ASK:
                    {
                        playerA = CurrentPlayer;
                        return true;
                    }
                case Action.JOIN:
                    {
                        playerB = CurrentPlayer;
                        return true;
                    }
                case Action.ALONE:
                    {
                        playerB = CurrentPlayer; //(intended) result: askplayer == joinplayer 
                        return true;
                    }
                case Action.ABONDANCE:
                    {
                        currentSpecial = Action.ABONDANCE;
                        HighestSpecialPlayer = CurrentPlayer;
                        return true;
                    }
                case Action.MISERIE:
                    {
                        if (currentSpecial == Action.MISERIE)
                        {
                            if (playerA == null)
                                playerA = HighestSpecialPlayer;
                            else
                                playerB = HighestSpecialPlayer;
                        }
                        else
                        {
                            playerA = null;
                            playerB = null;
                            currentSpecial = Action.MISERIE;
                        }
                        HighestSpecialPlayer = CurrentPlayer;
                        return true;
                    }
                case Action.SOLO:
                    {
                        currentSpecial = Action.SOLO;
                        HighestSpecialPlayer = CurrentPlayer;
                        return true;
                    }
                case Action.SOLOSLIM:
                    {
                        currentSpecial = Action.SOLOSLIM;
                        HighestSpecialPlayer = CurrentPlayer;
                        return true;
                    }
                default:
                    return false;
            }
        }

        //Set the Current Player to the next player.
        //Skip passed players.
        private void SetNextPlayer()
        {
            if (!passedPlayers.ContainsValue(true))//everyone passed
            {
                CurrentPlayer = null;
            }

            do
            {
                int nIndex = getCurrentIndex() + 1;
                if (nIndex == round.players.Length)
                    nIndex = 0;
                CurrentPlayer = round.players[nIndex];
            } while (passedPlayers[CurrentPlayer]);

        }

        private int getCurrentIndex()
        {
            for (int i = 0; i < round.players.Length; i++)
                if (round.players[i] == CurrentPlayer)
                    return i;
            return -1;
        }

        //Set Game Case and teams
        private void FinalizeBidding()
        {
            //TODO
            //Determine which of the 8 cases it is.
            if (currentSpecial == 0)
            {
                if (passedPlayers.ContainsValue(true) && passedPlayers.ContainsValue(false))
                {
                    if (playerA != null)
                    {
                        if (playerB != playerA)
                        {
                            round.gameCase = Case.TEAM;//Someone passed and player A & B are different => team
                             //Team = PlayerA + PlayerB
                        }
                        else
                        {
                            round.gameCase = Case.ALONE;//Someone passed and player A & B are the same => alone
                            //Team = PlayerA
                        }
                    }
                }
                else if (!passedPlayers.ContainsValue(true))//Everyone passed => FFA
                {
                    round.gameCase = Case.FFA;
                    //Team
                }
                else if (!passedPlayers.ContainsValue(false))//No one passed and no special => troel.
                {
                    round.gameCase = Case.TROEL;
                    //Team = PlayerA + PlayerB
                }
            }
            else
            {
                switch (currentSpecial)
                {
                    case Action.ABONDANCE:
                        {
                            round.gameCase = Case.ABONDANCE;
                            //team = HighestSpecialPlayer
                            break;
                        }
                    case Action.MISERIE:
                        {
                            round.gameCase = Case.MISERIE;
                            //team = not miseries vs (seperate team) miseries
                            break;
                        }
                    case Action.SOLO:
                        {
                            round.gameCase = Case.SOLO;
                            //team = HighestSpecialPlayer
                            break;
                        }
                    case Action.SOLOSLIM:
                        {
                            round.gameCase = Case.SOLOSLIM;
                            //team = HighestSpecialPlayer
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }

    public enum Action
    {
        PASS,
        ASK,
        JOIN,
        ALONE,
        ABONDANCE = DealAndBidNormal.lowestSpecial,
        MISERIE,
        SOLO,
        SOLOSLIM
    }
}
