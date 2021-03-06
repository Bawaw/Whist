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
            this.players = players;
            init();
        }

        private void init()
        {
            playerA = null;
            playerB = null;
            askForTrump = false;
            GameCase = Case.UNDECIDED;
            currentSpecial = 0;


            specialGameCases = SpecialGameCaseFactory.GetDictionary();
            CurrentPlayer = players[0];
            actionsDone = 0;
            passedPlayers = new Dictionary<Player, bool>();
            foreach (var player in players)
            {
                passedPlayers.Add(player, false);
                player.hand.Cards.Clear();
            }
 
            DealCards();
        }

        //Deal Cards and set initial Trump, also check for Troel
        private void DealCards()
        {
            (new DealCardsSimple()).DealCards(players);
            Trump = players[3].hand.Cards.Last().Suit;
            foreach (Player player in players)
                player.hand.sort();
            CheckForTroel();
        }
        
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
                    {
                        if (playerA != CurrentPlayer)
                            possibleActions.Add(Action.JOIN);
                        else
                            possibleActions.Add(Action.ALONE);
                    }
                }

                if (playerA != CurrentPlayer)
                    for (int i = lowestSpecial; i < Enum.GetValues(typeof(Action)).Length; i++)
                    {
                        possibleActions.Add((Action)i);
                    }

            }
            else
            {
                if (GameCase == Case.TROEL)
                {
                    for (int i = (int)Action.SOLO; i < Enum.GetValues(typeof(Action)).Length; i++)
                    {
                        possibleActions.Add((Action)i);
                    }
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

        int actionsDone;

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
                            if (GameCase == Case.UNDECIDED)
                            {
                                init();
                                return;
                            }
                            else
                            {
                                CurrentPlayer = null;
                                break;//Possible in case of Troel.
                            }
                        }
                    case 3:
                        {
                            if (GameCase == Case.UNDECIDED && playerA != null)//No one did anything special, one player Asked, no one responded.
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
                    GameCase = Case.UNDECIDED;
                    throw new ApplicationException();
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
                        Team teamA = new Team(new Player[] { playerA }, 5);
                        Player[] others = players.Except(teamA.Players).ToArray();
                        Team teamB = new Team(others, 9);
                        teams = new Team[] { teamA, teamB };
                        break;
                    }
                case Case.TROEL:
                    {
                        Troel troel = new Troel();
                        teams = troel.Teams(players);
                        Trump = troel.GetTrump(players);
                        firstPlayer = teams.Where(t => t.objective == 8).Single().Players.Where(p => p.hand.Cards.Where(c => c.Number == Numbers.ACE).Count() <= 1).Single();
                        break;
                    }
                case Case.ABONDANCE:
                    {
                        firstPlayer = specialGameCases[Case.ABONDANCE].selectedPlayers[0];
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
                        firstPlayer = specialGameCases[Case.SOLO].selectedPlayers[0];
                        teams = specialGameCases[Case.SOLO].Teams(players);
                        break;
                    }
                case Case.SOLOSLIM:
                    {
                        firstPlayer = specialGameCases[Case.SOLOSLIM].selectedPlayers[0];
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
        UNDECIDED,
        TEAM,
        ALONE,
        TROEL,
        ABONDANCE,
        MISERIE,
        SOLO,
        SOLOSLIM
    }
}
