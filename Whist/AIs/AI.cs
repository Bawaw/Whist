using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public abstract class AI
    {
        private Dictionary<Player, AIMemory> memory;
        protected Player player;
        private GameManager game;

        protected Round Round { get { return game.Round; } }

        public AI(Player player, GameManager game)
        {
            this.player = player;
            this.game = game;
        }

        public virtual void ProcessOtherPlayerAction(Player otherPlayer, Action action)
        {

        }

        public virtual void ProcessOtherPlayerCard(Player otherPlayer, Card card)
        {

        }

        protected Team CurrentTeam()
        {
            return Round.Teams.Where(t => t.Players.Any(p => p == Round.CurrentPlayer)).Single();
        }

        protected int GetCurrentPlayerIndex()
        {
            for (int i = 0; i < Round.Players.Count(); i++)
                if (Round.Players[i] == Round.CurrentPlayer)
                    return i;
            return -1;
        }

        
    }

    public class AIMemory
    {
        private Dictionary<Suits, bool> hasCardsOfSuitLeft;

        public AIMemory()
        {
            hasCardsOfSuitLeft = new Dictionary<Suits, bool>();
            foreach (Suits suit in System.Enum.GetValues(typeof(Suits)))
                hasCardsOfSuitLeft.Add(suit, true);
        }

        public bool HasCardsOfSuitLeft(Suits suit)
        {
            return hasCardsOfSuitLeft[suit];
        }

        public void NoCardsOfSuitLeft(Suits suit)
        {
            hasCardsOfSuitLeft[suit] = false;
        }
    }
}
