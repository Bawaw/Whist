using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
