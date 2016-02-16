using System.Collections.Generic;

namespace Whist.GameLogic.ControlEntities
{
    public interface IDealingAndBidding
    {
        Player CurrentPlayer { get; }
        Case GameCase { get; }
        bool InBiddingPhase { get; }
        Suits Trump { get; }

        bool DoAction(Action action);
        IEnumerable<Action> GetPossibleActions();
        ResultData FinalizeBidding();
    }
}