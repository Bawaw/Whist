using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    class DealAndBidNormal : IDealingAndBidding
    {
        private Player[] players;
        private Suits trump;

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
            DeckCollection cardCollection = new DeckCollection();
            cardCollection.initialise();
            cardCollection.shuffle();
            Card firstCard = cardCollection.peep();
            int nCards = cardCollection.Count / players.Length;
            foreach (var player in players)
                player.hand.AddCards(cardCollection.Draw(nCards));
            trump = firstCard.Suit;

            if (CheckForTroel())
                CurrentPlayer = null;
        }

        /*
        Check if any of the player has three aces.
        If so, troel.
        */
        private bool CheckForTroel()
        {
            bool troel = false;
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
                                        trump = card.Suit;
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
                        trump = Suits.HEARTS;
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
            }
        }

        public bool InBiddingPhase
        {
            get { return CurrentPlayer != null; }
        }

        //Bidding
        //Let each player in turn make a decision

        Player playerA; //PlayerA is, when no special: asking/alone player, or when special miserie: (possibly) one of miserie players
        Player playerB; //PlayerB is, when no special: joining/alone player, or when special miserie: (possibly) one of miserie players
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
                        GameCase = Case.TEAM;
                        return true;
                    }
                case Action.ALONE:
                    {
                        playerB = CurrentPlayer; //(intended) result: askplayer == joinplayer 
                        GameCase = Case.ALONE;
                        return true;
                    }
                case Action.ABONDANCE:
                    {
                        currentSpecial = Action.ABONDANCE;
                        GameCase = Case.ABONDANCE;
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
                            GameCase = Case.MISERIE;
                        }
                        HighestSpecialPlayer = CurrentPlayer;
                        return true;
                    }
                case Action.SOLO:
                    {
                        currentSpecial = Action.SOLO;
                        GameCase = Case.SOLO;
                        HighestSpecialPlayer = CurrentPlayer;
                        return true;
                    }
                case Action.SOLOSLIM:
                    {
                        currentSpecial = Action.SOLOSLIM;
                        GameCase = Case.SOLOSLIM;
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
            if (!passedPlayers.ContainsValue(false))//everyone passed
            {
                CurrentPlayer = null;
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
        public CaseAndTeam FinalizeBidding()
        {
            //TODO
            //Determine which of the 8 cases it is.
            if (GameCase == 0)
            {
                if (!passedPlayers.ContainsValue(false))//Everyone passed => FFA
                {
                    GameCase = Case.FFA;
                }
            }
            Team[] teams;
            switch (GameCase)
            {
                case Case.TEAM:
                    {
                        Team teamA = new Team(new Player[] { playerA, playerB }, 8);
                        Player[] others = (Player[]) players.Except(teamA.Players);
                        Team teamB = new Team(others, 6);
                        teams = new Team[] { teamA, teamB };
                        break;
                    }
                case Case.ALONE:
                    {
                        Team teamA = new Team(new Player[] { playerA }, 5);
                        Player[] others = (Player[])players.Except(teamA.Players);
                        Team teamB = new Team(others, 9);
                        teams = new Team[] { teamA, teamB };
                        break;
                    }
                case Case.FFA:
                    {
                        Team teamA = new Team(new Player[] { playerA }, 4);
                        Team teamB = new Team(new Player[] { playerB }, 4);
                        Player[] others = (Player[]) players.Except(teamA.Players);
                        Team teamC = new Team(new Player[] { others.First() }, 4);
                        Team teamD = new Team(new Player[] { others.Last() }, 4);
                        teams = new Team[] { teamA, teamB, teamC, teamD };
                        break;
                    }
                case Case.TROEL:
                    {
                        Team teamA = new Team(new Player[] { playerA, playerB }, 9);
                        Player[] others = (Player[])players.Except(teamA.Players);
                        Team teamB = new Team(others, 5);
                        teams = new Team[] { teamA, teamB };
                        break;
                    }
                case Case.ABONDANCE:
                    {
                        Team teamA = new Team(new Player[] { HighestSpecialPlayer }, 9);
                        playerB[] others = (playerB[])players.Except(teamA.Players);
                        Team teamB = new Team(others, 5);
                        teams = new Team[] { team1, teamB };
                        break;
                    }
                case Case.MISERIE:
                    {

                    }
                case Case.SOLO:
                    {

                    }
                case Case.SOLOSLIM:
                    {

                    }
                default: return null;
            }


            return new CaseAndTeam(teams, GameCase);
        }
    }

    internal class CaseAndTeam
    {
        public Team[] teams;
        public Case gameCase;

        public CaseAndTeam(Team[] teams, Case gameCase)
        {
            this.teams = teams;
            this.gameCase = gameCase;
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
