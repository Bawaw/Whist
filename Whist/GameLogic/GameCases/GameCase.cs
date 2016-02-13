using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.ControlEntities;

namespace Whist.GameLogic.GameCases
{
    public abstract class SpecialGameCase
    {
        public virtual int ID { get; }

        public virtual bool IsSelectable { get { return true; } }
        public virtual bool AllowsPlayerToSelectTrump { get { return false; } }


        public virtual bool HasAfterDealCheck { get { return false; } }
        public virtual bool AfterDealCheck(Player[] players) { return false; }

        public virtual int MaxAmountSelectedPlayers { get { return 1; } }
        public List<Player> selectedPlayers = new List<Player>();
        public void selectPlayer(Player player)
        {
            if (selectedPlayers.Count + 1 > MaxAmountSelectedPlayers) throw new Exception("Over MaxAmountSelectedPlayers");
            selectedPlayers.Add(player);
        }

        public abstract Team[] Teams(Player[] players);
        public abstract void ApplyScores(Team[] teams);
    }

    public class Troel : SpecialGameCase
    {
        public override bool IsSelectable { get { return false; } }
        public override bool HasAfterDealCheck { get { return true; } }
        public override bool AfterDealCheck(Player[] players)
        {
            //What about Trump? Need some way to give Trump, or decide troel doesn't change Trump.
            foreach (Player player in players)
            {
                int aces = 0;
                foreach (Card card in player.hand.Cards)
                {
                    if (card.Number == Numbers.ACE)
                    {
                        aces++;
                    }
                }
                if (aces >= 3)
                    return true;
            }
            return false;
        }

        public override Team[] Teams(Player[] players)
        {
            foreach (Player player in players)
            {
                int aces = 0;
                foreach (Card card in player.hand.Cards)
                {
                    if (card.Number == Numbers.ACE)
                    {
                        aces++;
                    }
                }
                if (aces >= 3)
                {
                    selectedPlayers.Add(player);
                    if (aces == 3)
                    {
                        foreach (Player playerTwo in players)
                        {
                            if (selectedPlayers[0] != playerTwo)
                            {
                                foreach (Card card in player.hand.Cards)
                                    if (card.Number == Numbers.ACE)
                                    {
                                        selectedPlayers.Add(playerTwo);
                                        break;
                                    }
                            }
                        }
                    }
                    else if (aces == 4)
                    {
                        //search for player with king of hearts, or if player with 4 aces also contains king of hearts, search for player with queen of hearts and so on
                        Card highestHeart = new Card(Suits.HEARTS, Numbers.KING);
                        while (selectedPlayers[0].hand.Cards.Contains(highestHeart))
                        {
                            //highestHeart--;
                            int number = (int)highestHeart.Number;
                            highestHeart = new Card(1, number--);
                        }
                        //teamplayer is player with highestHeart
                        selectedPlayers.Add(players.Where(p => p.hand.Cards.Contains(highestHeart)).Single());
                    }
                }
            }

            return new Team[]
            {
                new Team(selectedPlayers.ToArray(), 9),
                new Team(players.Except(selectedPlayers).ToArray(), 5)
            };
        }

        public override void ApplyScores(Team[] teams)
        {
            var teamA = teams.Where(t => t.objective == 9).Single();
            var teamB = teams.Where(t => t.objective == 5).Single();

            if (teamA.Tricks >= 9)
            {
                if (teamA.Tricks < 13)
                {
                    teamA.applyScore(8);
                    teamB.applyScore(-8);
                }
                else
                {
                    teamA.applyScore(15);
                    teamB.applyScore(-15);
                }
            }
            else
            {
                teamA.applyScore(-8);
                teamB.applyScore(8);
            }
        }

    }

    public class Abondance : SpecialGameCase
    {
        public override int ID { get { return 10; } }

        public override bool AllowsPlayerToSelectTrump { get { return true; } }

        public override Team[] Teams(Player[] players)
        {
            Team teamA = new Team(new Player[] { selectedPlayers[0] }, 9);
            Player[] others = (Player[])players.Except(selectedPlayers);
            Team teamB = new Team(others, 5);
            return new Team[] { teamA, teamB };
        }

        public override void ApplyScores(Team[] teams)
        {
            var teamA = teams.Where(t => t.objective == 9).Single();
            var teamB = teams.Where(t => t.objective == 5).Single();

            if (teamA.Tricks >= 9)
            {
                teamA.applyScore(24);
                teamB.applyScore(-8);
            }
            else
            {
                teamA.applyScore(-24);
                teamB.applyScore(8);
            }
        }
    }

    public class Miserie : SpecialGameCase
    {
        public override int ID { get { return 20; } }

        public override int MaxAmountSelectedPlayers { get { return 3; } }

        public override Team[] Teams(Player[] players)
        {
            var teammmsss = new List<Team>();
            foreach (var mplayer in selectedPlayers)
                teammmsss.Add(new Team(new Player[] { mplayer }, 0));
            teammmsss.Add(new Team((Player[])players.Except(selectedPlayers), 1));
            return teammmsss.ToArray();
        }


        public override void ApplyScores(Team[] teams)
        {
            var miserieTeams = teams.Where(t => t.objective == 0);
            var antimiserieTeam = teams.Where(t => t.objective == 1).Single();

            foreach (Team misTeam in miserieTeams)
                if (misTeam.Tricks == 0)
                {
                    misTeam.applyScore(33);
                    foreach (Team losingTeam in teams.Except(new Team[] { misTeam }))
                        losingTeam.applyScore(-11);
                }
                else
                {
                    misTeam.applyScore(-33);
                    foreach (Team winingTeam in teams.Except(new Team[] { misTeam }))
                        winingTeam.applyScore(11);
                }
        }
    }

    public class Solo : SoloSlim
    {
        public override int ID { get { return 30; } }

        public override bool AllowsPlayerToSelectTrump { get { return true; } }


        public override void ApplyScores(Team[] teams)
        {
            var teamA = teams.Where(t => t.objective == 13).Single();
            var teamB = teams.Where(t => t.objective == 1).Single();

            if (teamA.Tricks == 13)
            {
                teamA.applyScore(150);
                teamB.applyScore(-50);
            }
            else
            {
                teamA.applyScore(-150);
                teamB.applyScore(50);
            }
        }
    }

    public class SoloSlim : SpecialGameCase
    {
        public override int ID { get { return 40; } }

        public override Team[] Teams(Player[] players)
        {
            Team teamA = new Team(new Player[] { selectedPlayers[0] }, 13);
            Player[] others = (Player[])players.Except(selectedPlayers);
            Team teamB = new Team(others, 5);
            return new Team[] { teamA, teamB };
        }


        public override void ApplyScores(Team[] teams)
        {
            var teamA = teams.Where(t => t.objective == 13).Single();
            var teamB = teams.Where(t => t.objective == 1).Single();

            if (teamA.Tricks == 13)
            {
                teamA.applyScore(300);
                teamB.applyScore(-100);
            }
            else
            {
                teamA.applyScore(-300);
                teamB.applyScore(100);
            }
        }
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
