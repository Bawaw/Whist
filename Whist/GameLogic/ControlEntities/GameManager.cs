using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.AIs;

namespace Whist.GameLogic.ControlEntities
{
    public class GameManager
    {
        Player[] players;
        Dictionary<Player, IGameAI> gameAIPlayers;
        Dictionary<Player, IBidAI> bidAIPlayers;


        public GameManager()
        {
            players = new Player[]
            {
                new Player("Player",1),
                new Player("Comp 1",2),
                new Player("Comp 2",3),
                new Player("Comp 3",4)
            };
            HumanPlayer = players[0];

            RoundsToPlay = 13;
            RoundNumber = 1;
            IsGameInProgress = true;

            Round = new Round(players);
            gameAIPlayers = new Dictionary<Player, IGameAI>();
            bidAIPlayers = new Dictionary<Player, IBidAI>();
            foreach (Player player in NonHumanPlayers)
            {
                gameAIPlayers.Add(player, AIFactory.CreateGameAI(player, this, AIGameType.OMNISCIENT));
                bidAIPlayers.Add(player, AIFactory.CreateBidAI(player, this, AIBidType.BASIC));
            }

        }

        public GameManager(Player[] players, AIBidType[] bidAITypes, AIGameType[] gameAITypes)
        {
            this.players = players;
            bidAIPlayers = new Dictionary<Player, IBidAI>();
            gameAIPlayers = new Dictionary<Player, IGameAI>();
            for (int i = 0; i < 4; i++)
            {
                bidAIPlayers.Add(players[i], AIFactory.CreateBidAI(players[i], this, bidAITypes[i]));
                gameAIPlayers.Add(players[i], AIFactory.CreateGameAI(players[i], this, gameAITypes[i]));
            }
            RoundsToPlay = 13;
            RoundNumber = 1;
            IsGameInProgress = true;
            Round = new Round(players);
        }

        public Player HumanPlayer
        {
            get;
            private set;
        }

        public Round Round
        {
            get; private set;
        }

        public int RoundsToPlay { get; private set; }
        public int RoundNumber { get; private set; }

        public IEnumerable<Player> NonHumanPlayers
        {
            get { return players.Except(new Player[] { HumanPlayer }); }
        }

        public IGameAI GetGameAI(Player player)
        {
            return gameAIPlayers[player];
        }

        public IBidAI GetBidAI(Player player)
        {
            return bidAIPlayers[player];
        }

        public bool IsRoundInProgress
        {
            get { if (Round == null) return false; else return Round.RoundInProgress; }
        }

        public void StartNewRound()
        {
            if (!IsRoundInProgress && IsGameInProgress)
            {
                if (RoundNumber < RoundsToPlay)
                {
                    CyclePlayers();
                    RoundNumber++;
                    Round = new Round(players);
                }
                else
                {
                    IsGameInProgress = false;
                }
            }
        }

        public bool IsGameInProgress { get; private set; }

        private void CyclePlayers()
        {
            var temp = players[0];
            for (int i = 1; i < players.Length; i++)
            {
                players[i - 1] = players[i];
            }
            players[players.Length - 1] = temp;
        }
    }
}
