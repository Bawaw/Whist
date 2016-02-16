using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.GameCases;

namespace Whist.GameLogic.ControlEntities
{
    public class DealAndBidNormal : IDealingAndBidding
    {
        private Player[] players;
        public Suits Trump
        {
            get;
            private set;
        }

        Player playerA; //PlayerA is, when no special: asking/alone player, or when special miserie: (possibly) one of miserie players, or when special troel: player with most aces 
        Player playerB; //PlayerB is, when no special: joining/alone player, or when special miserie: (possibly) one of miserie players, or when special troel: other teammember
        Action currentSpecial = 0;
        Dictionary<Player, bool> passedPlayers;
        private bool askForTrump;
        public const int lowestSpecial = 8;
        public Case GameCase
        {
            get; private set;
        }

        public Player CurrentPlayer
        {
            get;
            private set;
        }
        public bool InBiddingPhase
        {
            get { return CurrentPlayer != null; }
        }

        Dictionary<Case, SpecialGameCase> specialGameCases;

        public DealAndBidNormal(Player[] players)
        {
            specialGameCases = SpecialGameCaseFactory.GetDictionary();
            this.players = players;
            CurrentPlayer = players[0];
            passedPlayers = new Dictionary<Player, bool>();
            foreach (var player in players)
                passedPlayers.Add(player, false);

            DealCards();
        }

        //Deal Cards and set initial Trump, also check for Troel
        private void DealCards()
        {
            (new DealCardsSimple()).DealCards(players);
            Trump = players[3].hand.Cards.Last().Suit;
            foreach (Player player in players)
                player.hand.sort();
        }

        /*
        Check if any of the player has three aces.
        If so, troel.
        */
        private bool CheckForTroel()
        {
            Troel troel = new Troel();
            if (troel.AfterDealCheck(players))
            {
                GameCase = Case.TROEL;
                return true;
            }
            else
                return false;
        }


        //Bidding
        //Let each player in turn make a decision


        public IEnumerable<Action> GetPossibleActions()
        {
            var possibleActions = new HashSet<Action>();
            if (askForTrump)
            {
                possibleActions.Add(Action.HEARTS);
                possibleActions.Add(Action.CLUBS);
                possibleActions.Add(Action.DIAMONDS);
                possibleActions.Add(Action.SPADES);
                return possibleActions;
            }
            possibleActions.Add(Action.PASS);
            if (currentSpecial == 0 && GameCase != Case.TROEL)
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
                if (GameCase == Case.TROEL)
                {
                    possibleActions.Add(Action.SOLO);
                    possibleActions.Add(Action.SOLOSLIM);
                }
                else
                {
                    for (int i = (int)currentSpecial + 1; i < Enum.GetValues(typeof(Action)).Length; i++)
                    {
                        possibleActions.Add((Action)i);
                    }
                    if (currentSpecial == Action.MISERIE && specialGameCases[Case.MISERIE].selectedPlayers.Count < specialGameCases[Case.MISERIE].MaxAmountSelectedPlayers)
                        possibleActions.Add(Action.MISERIE);
                }
            }

            return possibleActions;
        }

        int actionsDone = 0;

        public bool DoAction(Action action)
        {
            if (!GetPossibleActions().Contains(action))
                return false;

            if (askForTrump)
            {
                switch (action)
                {
                    case Action.HEARTS:
                        {
                            Trump = Suits.HEARTS;
                            break;
                        }
                    case Action.CLUBS:
                        {
                            Trump = Suits.CLUBS;
                            break;
                        }
                    case Action.SPADES:
                        {
                            Trump = Suits.SPADES;
                            break;
                        }
                    case Action.DIAMONDS:
                        {
                            Trump = Suits.DIAMONDS;
                            break;
                        }
                }
            }
            else
            {

                switch (action)
                {
                    case Action.PASS:
                        {
                            passedPlayers[CurrentPlayer] = true;
                            break;
                        }
                    case Action.ASK:
                        {
                            playerA = CurrentPlayer;
                            break;
                        }
                    case Action.JOIN:
                        {
                            playerB = CurrentPlayer;
                            GameCase = Case.TEAM;
                            break;
                        }
                    case Action.ALONE:
                        {
                            GameCase = Case.ALONE;
                            break;
                        }
                    case Action.ABONDANCE:
                        {
                            currentSpecial = Action.ABONDANCE;
                            GameCase = Case.ABONDANCE;
                            specialGameCases[Case.ABONDANCE].selectPlayer(CurrentPlayer);
                            break;
                        }
                    case Action.MISERIE:
                        {
                            if (currentSpecial != Action.MISERIE)
                            {
                                playerA = null;
                                playerB = null;
                                currentSpecial = Action.MISERIE;
                                GameCase = Case.MISERIE;
                            }
                            specialGameCases[Case.MISERIE].selectPlayer(CurrentPlayer);
                            break;
                        }
                    case Action.SOLO:
                        {
                            currentSpecial = Action.SOLO;
                            GameCase = Case.SOLO;
                            specialGameCases[Case.SOLO].selectPlayer(CurrentPlayer);
                            break;
                        }
                    case Action.SOLOSLIM:
                        {
                            currentSpecial = Action.SOLOSLIM;
                            GameCase = Case.SOLOSLIM;
                            specialGameCases[Case.SOLOSLIM].selectPlayer(CurrentPlayer);
                            break;
                        }
                    default:
                        return false;
                }
            }
            actionsDone++;
            SetNextPlayer();
            return true;
        }

        //Set the Current Player to the next player.
        //Skip passed players.
        private void SetNextPlayer()
        {
            if (askForTrump)
            {
                CurrentPlayer = null;
                return;
            }

            if (actionsDone >= 4 )//All players made a decision.
            {
                switch (passedPlayers.Where(p => p.Value == true).Count())//Amount of players that passed.
                {
                    case 4:
                        {
                            CurrentPlayer = null;
                            break;
                        }
                    case 3:
                        {
                            if (GameCase == Case.FFA && playerA != null)//No one did anything special, one player Asked, no one responded.
                                CurrentPlayer = playerA;//Ask player if he wants to go alone or FFA.
                            else
                                CurrentPlayer = null;
                            break;
                        }
                    case 2:
                        {
                            if (GameCase == Case.TEAM)
                            {
                                CurrentPlayer = null;
                                break;
                            }
                            if (GameCase == Case.MISERIE && specialGameCases[Case.MISERIE].selectedPlayers.Count() == 2)
                            {
                                CurrentPlayer = null;
                                break;
                            }
                            break;
                        }

                    case 1:
                        {
                            if (GameCase == Case.MISERIE && specialGameCases[Case.MISERIE].selectedPlayers.Count() == 3)
                            {
                                CurrentPlayer = null;
                                break;
                            }
                            break;
                        }
                }
            }

            if (CurrentPlayer == null)
            {
                if (specialGameCases.ContainsKey(GameCase) && specialGameCases[GameCase].AllowsPlayerToSelectTrump)
                {
                    CurrentPlayer = specialGameCases[GameCase].selectedPlayers[0];
                    askForTrump = true;
                }
                return;
            }

            do
            {
                int nIndex = getCurrentIndex() + 1;
                if (nIndex == players.Length)
                    nIndex = 0;
                CurrentPlayer = players[nIndex];
            } while (passedPlayers[CurrentPlayer]);

        }

        private int getCurrentIndex()
        {
            for (int i = 0; i < players.Length; i++)
                if (players[i] == CurrentPlayer)
                    return i;
            return -1;
        }

        //Set Game Case and teams
        public ResultData FinalizeBidding()
        {
            if (GameCase == 0)//GameCase 0 is FFA, perhaps unnecessary.
            {
                if (!passedPlayers.ContainsValue(false))//Everyone passed => FFA
                {
                    GameCase = Case.FFA;
                }
            }

            Team[] teams;
            Player firstPlayer = players[0];
            switch (GameCase)
            {
                case Case.TEAM:
                    {
                        Team teamA = new Team(new Player[] { playerA, playerB }, 8);
                        Player[] others = players.Except(teamA.Players).ToArray();
                        Team teamB = new Team(others, 6);
                        teams = new Team[] { teamA, teamB };
                        break;
                    }
                case Case.ALONE:
                    {
                        firstPlayer = playerA;
                        Team teamA = new Team(new Player[] { playerA }, 5);
                        Player[] others = players.Except(teamA.Players).ToArray();
                        Team teamB = new Team(others, 9);
                        teams = new Team[] { teamA, teamB };
                        break;
                    }
                case Case.FFA:
                    {
                        Team teamA = new Team(new Player[] { players[0] }, 4);
                        Team teamB = new Team(new Player[] { players[1] }, 4);
                        Team teamC = new Team(new Player[] { players[2] }, 4);
                        Team teamD = new Team(new Player[] { players[3] }, 4);
                        teams = new Team[] { teamA, teamB, teamC, teamD };
                        break;
                    }
                case Case.TROEL:
                    {
                        Troel troel = new Troel();
                        teams = troel.Teams(players);
                        break;
                    }
                case Case.ABONDANCE:
                    {
                        teams = specialGameCases[Case.ABONDANCE].Teams(players);
                        break;
                    }
                case Case.MISERIE:
                    {
                        teams = specialGameCases[Case.MISERIE].Teams(players);
                        break;
                    }
                case Case.SOLO:
                    {
                        teams = specialGameCases[Case.SOLO].Teams(players);
                        break;
                    }
                case Case.SOLOSLIM:
                    {
                        teams = specialGameCases[Case.SOLOSLIM].Teams(players);
                        break;
                    }
                default: return null;
            }

            return new ResultData(teams, GameCase, firstPlayer, Trump);
        }
    }

    public class ResultData
    {
        public Team[] teams;
        public Case gameCase;
        public Player firstPlayer;
        public Suits trump;

        public ResultData(Team[] teams, Case gameCase, Player firstPlayer, Suits trump)
        {
            this.teams = teams;
            this.gameCase = gameCase;
            this.firstPlayer = firstPlayer;
            this.trump = trump;
        }
    }

    public enum Action
    {
        HEARTS,
        CLUBS,
        DIAMONDS,
        SPADES,
        PASS,
        ASK,
        JOIN,
        ALONE,
        ABONDANCE = DealAndBidNormal.lowestSpecial,
        MISERIE,
        SOLO,
        SOLOSLIM
    }


    public enum Case
    {
        FFA,
        TEAM,
        ALONE,
        TROEL,
        ABONDANCE,
        MISERIE,
        SOLO,
        SOLOSLIM
    }
}
