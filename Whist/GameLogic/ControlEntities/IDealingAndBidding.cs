using System.Collections.Generic;

namespace Whist.GameLogic.ControlEntities
{
    interface IDealingAndBidding
    {
        Player CurrentPlayer { get; }
        Case GameCase { get; }
        bool InBiddingPhase { get; }

        bool DoAction(Action action);
        IEnumerable<Action> GetPossibleActions();
    }
}