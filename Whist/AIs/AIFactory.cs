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
        public static AI CreateAI(Player player, GameManager game, AIType type)
        {
            switch (type)
            {
                case AIType.Basic:
                    return new BaseAI(player, game);
            }
            throw new ApplicationException();
        }
    }

    public enum AIType
    {
        Basic
    }
}
