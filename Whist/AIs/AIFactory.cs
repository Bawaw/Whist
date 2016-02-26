﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic.ControlEntities;

namespace Whist.AIs
{
        public enum AIBidType
        {
        BASIC
        }

        public enum AIGameType
        {
            BASIC, 
        MEMORY,
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
                case AIGameType.OMNISCIENT:
                    return new OmniscentSearchAI(player, game, new StandardReferee());
            }
            throw new ApplicationException();
        }
    }
}