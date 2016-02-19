using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.ControlEntities;

namespace Whist.GameLogic.GameCases
{
    public static class SpecialGameCaseFactory
    {
        public static List<SpecialGameCase> GetList()
        {
            return new List<SpecialGameCase>()
            {
                new Abondance(),
                new Troel(),
                new Miserie(),
                new Solo(),
                new Soloslim()
            };
        }

        public static Dictionary<Case, SpecialGameCase> GetDictionary()
        {
            var dict =  new Dictionary<Case, SpecialGameCase>();
            dict.Add(Case.ABONDANCE, new Abondance());
            dict.Add(Case.MISERIE, new Miserie());
            dict.Add(Case.SOLO, new Solo());
            dict.Add(Case.SOLOSLIM, new Soloslim());
            dict.Add(Case.TROEL, new Troel());
            return dict;
        }
    }

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
                if (player.hand.Cards.Where(c => c.Number == Numbers.ACE).Count() >= 3)
                    return true;
            return false;
        }

        public override Team[] Teams(Player[] players)
        {
            foreach (Player player in players)
            {
                int aces = player.hand.Cards.Where(c => c.Number == Numbers.ACE).Count();
                if (aces >= 3)
                {
                    selectedPlayers.Add(player);
                    if (aces == 3)
                    {
                        foreach (Player playerTwo in players.Except(selectedPlayers))
                        {
                            if (playerTwo.hand.Cards.Where(c => c.Number == Numbers.ACE).Count() > 0)
                            {
                                selectedPlayers.Add(playerTwo);
                                break;
                            }
                        }
                    }
                    else if (aces == 4)
                    {
                        var playerOneHearts = player.hand.Cards.Where(c => c.Suit == Suits.HEARTS);
                        Numbers highestHeart = Numbers.KING;
                        while (playerOneHearts.Any(c => c.Number == highestHeart))
                        {
                            highestHeart--;
                        }
                        foreach (Player p in players.Except(selectedPlayers))
                        {
                            if (p.hand.Cards.Any(c => c.Suit == Suits.HEARTS && c.Number == highestHeart))
                            {
                                selectedPlayers.Add(p);
                                break;
                            }
                        }
                    }
                    break;
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
        
        public Suits GetTrump(Player[] players)
        {
            foreach (Player player in players)
            {
                var aces = player.hand.Cards.Where(c => c.Number == Numbers.ACE);
                if (aces.Count() == 1)
                {
                    return aces.Single().Suit;
                }
                if (aces.Count() == 4)
                {
                    return Suits.HEARTS;
                }
            }
            return Suits.HEARTS;
        }
    }

    public class Abondance : SpecialGameCase
    {
        public override int ID { get { return 10; } }

        public override bool AllowsPlayerToSelectTrump { get { return true; } }

        public override Team[] Teams(Player[] players)
        {
            Team teamA = new Team(new Player[] { selectedPlayers[0] }, 9);
            Player[] others = players.Except(selectedPlayers).ToArray();
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
            teammmsss.Add(new Team(players.Except(selectedPlayers).ToArray(), 1));
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

    public class Solo : Soloslim
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

    public class Soloslim : SpecialGameCase
    {
        public override int ID { get { return 40; } }

        public override Team[] Teams(Player[] players)
        {
            Team teamA = new Team(new Player[] { selectedPlayers[0] }, 13);
            Player[] others = players.Except(selectedPlayers).ToArray();
            Team teamB = new Team(others, 1);
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
    
}
