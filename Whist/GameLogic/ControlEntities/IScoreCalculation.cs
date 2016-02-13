namespace Whist.GameLogic.ControlEntities
{
    public interface IScoreCalculation
    {
        void CalculateScores(Team[] teams, Case gameCase);
    }
}