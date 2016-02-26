using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.AIs;
using Whist.GameLogic.ControlEntities;

namespace Whist_AI_Arena
{
    class Program
    {
        static void Main(string[] args)
        {
            AIArena arena = new AIArena();
            arena.PlayGame();

            foreach (Player p in arena.players)
                Console.WriteLine(p.name + " scored " + p.score);

        }
        

        
    }

    class AIArena
    {
        public Settings settings;
        public Stats stats;
        public Player[] players;
        public GameManager gameManager { get; private set; }
        public Round Round { get { return gameManager.Round; } }

        public AIArena()
        {
            stats = new Stats();
            players = new Player[]
            {
                new Player("C1", 0),
                new Player("C2", 1),
                new Player("C3", 2),
                new Player("C4", 3)
            };

            var bidAIPlayers = new Dictionary<Player, IBidAI>();
            var gameAIPlayers = new Dictionary<Player, IGameAI>();
            gameManager = new GameManager(players, new AIBidType[]
            {
                AIBidType.BASIC,
                AIBidType.BASIC,
                AIBidType.BASIC,
                AIBidType.BASIC
            }, new AIGameType[]
            {
                AIGameType.OMNISCIENT,
                AIGameType.BASIC,
                AIGameType.BASIC,
                AIGameType.BASIC,
            });
        }
        


        public void PlayGame()
        {
            while (gameManager.IsGameInProgress)
            {
                DoBidPhase();
                DoGamePhase();
                StartNewRound();
            }
        }

        public void StartNewRound()
        {
            gameManager.StartNewRound();
        }

        private void CyclePlayers()
        {
            var temp = players[0];
            for (int i = 1; i < players.Length; i++)
            {
                players[i - 1] = players[i];
            }
            players[players.Length - 1] = temp;
        }

        public void DoBidPhase()
        {
            while (Round.InBiddingPhase)
            {
                DoBidAction();
            }
            Round.EndBiddingRound();
        }

        public void DoBidAction()
        {
            var action = GetCurrentBidAI().GetAction();
            Round.BiddingDoAction(action);
        }

        public AI GetCurrentBidAI()
        {
            return gameManager.GetAI(Round.CurrentPlayer);
        }

        public void DoGamePhase()
        {
            while (Round.InTrickPhase)
            {
                while (Round.TrickInProgress)
                {
                    PlayCard();
                }
                Round.EndTrick();
            }
            Round.EndTricksRound();
        }

        public void PlayCard()
        {
            var card = GetCurrentGameAI().GetMove();
            Round.PlayCard(card);
        }


        public AI GetCurrentGameAI()
        {
            return gameManager.GetAI(Round.CurrentPlayer);
        }

        int ChoosePlayerIndex()
        {
            Console.WriteLine("Select player index. (1-4)");
            int playerIndex = 0;
            while (!int.TryParse(Console.ReadLine(), out playerIndex) || !(playerIndex > 0 && playerIndex <= 4)) ;
            return playerIndex;
        }
    }

    class Settings
    {

    }

    class Stats
    {
        public int[] tricksWon = new int[4];
        public int[] roundsWon = new int[4];
    }
}
