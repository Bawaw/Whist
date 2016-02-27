using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public interface IGameAI
    {
        Card GetMove();

        void ResetMemory();
        void ProcessOtherPlayerAction(Player otherPlayer, Action action);
        void ProcessOtherPlayerCard(Player otherPlayer, Card card);
    }
}
