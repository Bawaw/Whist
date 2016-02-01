using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic.ControlEntities
{
    public interface IScoreMechanisme {
        //calculates score and cleans tricks
        Team distributeScore(Team[] teams, GameMode gameMode);
    }

    public class SimpleScoreMechanisme : IScoreMechanisme
    {
        public Team distributeScore(Team[] teams,GameMode gameMode)
        {
            int[] scores = new int[teams.Length];

            for (int i = 0; i < teams.Length; i++)
                scores[i] = teams[i].Tricks; 

            //assumes 2 teams can not have same score
            return teams[scores.ToList().IndexOf(scores.Max())];
        }
    }
}
