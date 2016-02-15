using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.GameCases;

namespace Whist.GameLogic.ControlEntities
{
    class DealAndBidNormal : IDealingAndBidding
    {
        private Player[] players;
        public Suits Trump
        {
            get;
            private set;
        }

        Player playerA; //PlayerA is, when no special: asking/alone player, or when special miserie: (possibly) one of miserie players, or when special troel: player with most aces 
        Player playerB; //PlayerB is, when no special: joining/alone player, or when special miserie: (possibly) one of miserie players, or when special troel: other teammember
        Player HighestSpecialPlayer;
        Action currentSpecial = 0;
        Dictionary<Player, bool> passedPlayers;
        public const int lowestSpecial = 4;
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


        public DealAndBidNormal(Player[] players)
        {
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
            CheckForTroel();
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
            /*
            List<Player> troelPlayers = new List<Player>();
            for (int i = 0; i < players.Count(); i++)
            {
                int aces = 0;
                foreach (Card card in players[i].hand.Cards)
                {
                    if (card.Number == Numbers.ACE)
                    {
                        aces++;
                    }
                }
                if (aces == 3 || aces == 4)
                {
                    if (aces == 3)
                    {
                        troelPlayers.Add(players[i]);
                        bool teamPlayerFound = false;
                        for (int j = 0; j < players.Count(); j++)
                        {
                            //if it is another player than current player
                            if (j != i)
                            {
                                foreach (Card card in players[j].hand.Cards)
                                {
                                    if (card.Number == Numbers.ACE)
                                    {
                                        teamPlayerFound = true;
                                        troelPlayers.Add(players[j]);
                                        //set trump
                                        Trump = card.Suit;
                                        //card found, break out of loop of cards
                                        break;
                                    }
                                }
                                if (teamPlayerFound)
                                {
                                    //player found, break out of loop of players
                                    break;
                                }
                            }
                        }
                    }
                    else if (aces == 4)
                    {
                        troelPlayers.Add(players[i]);
                        //search for player with king of hearts, or if player with 4 aces also contains king of hearts, search for player with queen of hearts and so on
                        Card highestHeart = new Card(Suits.HEARTS, Numbers.KING);
                        while (players[i].hand.Cards.Contains(highestHeart))
                        {
                            //highestHeart--;
                            int number = (int)highestHeart.Number;
                            highestHeart = new Card(1, number--);
                        }
                        //teamplayer is player with highestHeart
                        Player teamPlayer = players.Where(p => p.hand.Cards.Contains(highestHeart)).First();
                        troelPlayers.Add(teamPlayer);
                        //set trump
                        Trump = Suits.HEARTS;
                    }
                    troel = true;
                }
            }
            if (!troel)//No Troel
                return false;
            else//Troel
            {
                playerA = troelPlayers.First();//Player with most aces
                playerB = troelPlayers.Last();//Team member;
                //set currentPlayer
                CurrentPlayer = playerB;
                GameCase = Case.TROEL;
                return true;
            }*/
        }


        //Bidding
        //Let each player in turn make a decision


        public IEnumerable<Action> GetPossibleActions()
        {
            var possibleActions = new HashSet<Action>();
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
                    if (currentSpecial == Action.MISERIE && playerB == null)
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
                        actionsDone--;
                        break;
                    }
                case Action.ALONE:
                    {
                        playerB = CurrentPlayer; //(intended) result: PlayerB == PlayerA || perhaps unnecessary with changes. 
                        GameCase = Case.ALONE;
                        break;
                    }
                case Action.ABONDANCE:
                    {
                        currentSpecial = Action.ABONDANCE;
                        GameCase = Case.ABONDANCE;
                        HighestSpecialPlayer = CurrentPlayer;
                        break;
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
                            GameCase = Case.MISERIE;
                        }
                        HighestSpecialPlayer = CurrentPlayer;
                        break;
                    }
                case Action.SOLO:
                    {
                        currentSpecial = Action.SOLO;
                        GameCase = Case.SOLO;
                        HighestSpecialPlayer = CurrentPlayer;
                        break;
                    }
                case Action.SOLOSLIM:
                    {
                        currentSpecial = Action.SOLOSLIM;
                        GameCase = Case.SOLOSLIM;
                        HighestSpecialPlayer = CurrentPlayer;
                        break;
                    }
                default:
                    return false;
            }
            actionsDone++;
            SetNextPlayer();
            return true;
        }

        //Set the Current Player to the next player.
        //Skip passed players.
        private void SetNextPlayer()
        {

            if (actionsDone >= 4)//All players made a decision.
            {
                switch (passedPlayers.Where(p => p.Value == true).Count())//Amount of players that passed.
                {
                    case 4:
                        {
                            CurrentPlayer = null;
                            return;
                        }
                    case 3:
                        {
                            if (GameCase == Case.FFA && playerA != null)//No one did anything special, one player Asked, no one responded.
                                CurrentPlayer = playerA;//Ask player if he wants to go alone or FFA.
                            else
                                CurrentPlayer = null;
                            return;
                        }
                    case 2:
                        {
                            if (GameCase == Case.TEAM)
                            {
                                CurrentPlayer = null;
                                return;
                            }
                            if (GameCase == Case.MISERIE && playerA != null)
                            {
                                CurrentPlayer = null;
                                return;
                            }
                            break;
                        }

                    case 1:
                        {
                            if (GameCase == Case.MISERIE && playerA != null && playerB != null)
                            {
                                CurrentPlayer = null;
                                return;
                            }
                            break;
                        }
                }
            }

            if (CurrentPlayer == null)
                return;

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
                        /*Team teamA = new Team(new Player[] { playerA, playerB }, 9);
                        Player[] others = players.Except(teamA.Players).ToArray();
                        Team teamB = new Team(others, 5);
                        teams = new Team[] { teamA, teamB };*/
                        break;
                    }
                case Case.ABONDANCE:
                    {
                        firstPlayer = HighestSpecialPlayer;
                        Team teamA = new Team(new Player[] { HighestSpecialPlayer }, 9);
                        Player[] others = players.Except(teamA.Players).ToArray();
                        Team teamB = new Team(others, 5);
                        teams = new Team[] { teamA, teamB };
                        break;
                    }
                case Case.MISERIE:
                    {
                        var teammmsss = new List<Team>();
                        var miserieplayers = new List<Player>();
                        miserieplayers.Add(HighestSpecialPlayer);
                        if (playerA != null)
                            miserieplayers.Add(playerA);
                        if (playerB != null)
                            miserieplayers.Add(playerB);

                        foreach (var mplayer in miserieplayers)
                            teammmsss.Add(new Team(new Player[] { mplayer }, 0));
                        teammmsss.Add(new Team(players.Except(miserieplayers).ToArray(), 1));
                        teams = teammmsss.ToArray();
                        break;
                    }
                case Case.SOLO:
                    {
                        firstPlayer = HighestSpecialPlayer;
                        Team teamA = new Team(new Player[] { HighestSpecialPlayer }, 13);
                        Player[] others = players.Except(teamA.Players).ToArray();
                        Team teamB = new Team(others, 1);
                        teams = new Team[] { teamA, teamB };
                        break;
                    }
                case Case.SOLOSLIM:
                    {
                        Team teamA = new Team(new Player[] { HighestSpecialPlayer }, 13);
                        Player[] others = players.Except(teamA.Players).ToArray();
                        Team teamB = new Team(others, 1);
                        teams = new Team[] { teamA, teamB };
                        break;
                    }
                default: return null;
            }


            return new ResultData(teams, GameCase, firstPlayer, Trump);
        }
    }

    internal class ResultData
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
