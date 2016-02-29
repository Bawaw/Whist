using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
    public enum AIBidType
    {
        BASIC,
        CAUTIOUS,
        SIMGAME,
        OMNISCIENT
    }

    public enum AIGameType
    {
        BASIC,
        MEMORY,
        PERFECTMEMORY,
        BRUTEFORCE,
        OMNISCIENT
    }

    public class AIFactory
    {

        public static AI CreateAI(Player player, GameManager game, AIBidType bidType, AIGameType gameType)
        {
            return new AI(player, CreateBidAI(player, game, bidType), CreateGameAI(player, game, gameType));
        }

        public static IBidAI CreateBidAI(Player player, GameManager game, AIBidType type)
        {
            switch (type)
            {
                case AIBidType.BASIC:
                    return new BaseBidAI(player, game);
                case AIBidType.CAUTIOUS:
                    return new CautiousBidAI(player, game);
                case AIBidType.SIMGAME:
                    return new SimulateGameBidAI(player, game);
                case AIBidType.OMNISCIENT:
                    return new BidSearchAI(game);
            }
            throw new ApplicationException();
        }

        public static IGameAI CreateGameAI(Player player, GameManager game, AIGameType type)
        {
            switch (type)
            {
                case AIGameType.BASIC:
                    return new BaseGameAI(player, game);
                case AIGameType.MEMORY:
                    return new MemoryAI(player, game);
                case AIGameType.PERFECTMEMORY:
                    return new PerfectMemoryAI(player, game);
                case AIGameType.BRUTEFORCE:
                    return new BruteForceAI(player, game);
                case AIGameType.OMNISCIENT:
                    return new OmniscentSearchAI(game, new StandardReferee());
            }
            throw new ApplicationException();
        }
    }
}