using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.GameCases;

namespace Whist.GameLogic.ControlEntities
{
    public class SimpleScoreMechanisme : IScoreCalculation
    {
        public void CalculateScores(Team[] teams, Case gameCase)
        {
            int[] scores = new int[teams.Length];

            for (int i = 0; i < teams.Length; i++)
            {
                scores[i] = teams[i].Tricks;
                foreach (var player in teams[i].Players)
                    player.score += scores[i];
            }
        }
    }

    public class BasicScoreMechanism : IScoreCalculation
    {
        public void CalculateScores(Team[] teams, Case gameCase)
        {
            switch (gameCase)
            {
                case Case.TEAM:
                    {
                        Team teamA = teams.Where(t => t.objective == 8).Single();
                        Team teamB = teams.Where(t => t.objective == 6).Single();
                        if (teamA.Tricks >= teamA.objective)
                        {
                            int overslagen = teamA.Tricks - teamA.objective;
                            int scorePP = (2 + overslagen);
                            if (teamA.Tricks == 13)
                                scorePP *= 2;
                            teamA.applyScore(scorePP);
                            teamB.applyScore(-scorePP);
                        }
                        else
                        {
                            int onderslagen = teamA.objective - teamA.Tricks;
                            int scorePP = 2 + onderslagen;
                            teamA.applyScore(-scorePP);
                            teamB.applyScore(scorePP);
                        }
                        break;
                    }
                case Case.ALONE:
                    {
                        Team aloneTeam = teams.Where(t => t.objective == 5).Single();
                        Team otherTeam = teams.Where(t => t.objective == 9).Single();
                        if (aloneTeam.Tricks >= aloneTeam.objective)
                        {
                            int overslagen = aloneTeam.Tricks - aloneTeam.objective;
                            int scorePP = (2 + overslagen);
                            if (aloneTeam.Tricks == 13)
                                scorePP *= 2;
                            aloneTeam.applyScore(scorePP * 3);
                            otherTeam.applyScore(-scorePP);
                        }
                        else
                        {
                            int onderslagen = aloneTeam.objective - aloneTeam.Tricks;
                            int scorePP = 3 + onderslagen;
                            aloneTeam.applyScore(-scorePP * 3);
                            otherTeam.applyScore(scorePP);
                        }
                        break;
                    }
                default:
                    {
                        SpecialGameCaseFactory.GetDictionary()[gameCase].ApplyScores(teams);
                        break;
                    }
                /*case Case.ABONDANCE:
                    {
                        var abondance = new Abondance();
                        abondance.ApplyScores(teams);
                        break;
                    }
                case Case.TROEL:
                    {
                        var troel = new Troel();
                        troel.ApplyScores(teams);
                        break;
                    }
                case Case.MISERIE:
                    {
                        var miserie = new Miserie();
                        miserie.ApplyScores(teams);
                        break;
                    }
                case Case.SOLO:
                    {
                        var solo = new Solo();
                        solo.ApplyScores(teams);
                        break;
                    }
                case Case.SOLOSLIM:
                    {
                        var soloslim = new Soloslim();
                        soloslim.ApplyScores(teams);
                        break;
                    }*/
            }
        }
    }
}
