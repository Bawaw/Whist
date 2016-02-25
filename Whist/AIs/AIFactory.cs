using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public class AIFactory
    {
        public enum AIBidType
        {
            BASIC,
            MEMORY
        }

        public enum AIGameType
        {
            BASIC, 
            OMNISCIENT
        }

        public static IBidAI CreateBidAI(Player player, GameManager game, AIBidType type)
        {
            switch (type)
            {
                case AIBidType.BASIC:
                    return new BidAI(player, game);
                    //case AIType.Memory:
                    //  return new MemoryAI(player, game);
            }
            throw new ApplicationException();
        }

        public static IGameAI CreateGameAI(Player player, GameManager game, AIGameType type)
        {
            switch (type)
            {
                case AIGameType.BASIC:
                    return new GameAI(player, game);
                case AIGameType.OMNISCIENT:
                    return new OmniscentSearchAI(player, game);
                    //case AIType.Memory:
                    //  return new MemoryAI(player, game);
            }
            throw new ApplicationException();
        }
    }
}