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

            Console.ReadKey();
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
            
            gameManager = new GameManager(players, 10001, new AIBidType[]
            {
                AIBidType.BASIC,
                AIBidType.BASIC,
                AIBidType.BASIC,
                AIBidType.BASIC
            }, new AIGameType[]
            {
                AIGameType.PERFECTMEMORY,
                AIGameType.MEMORY,
                AIGameType.MEMORY,
                AIGameType.MEMORY,
            });
        }
        


        public void PlayGame()
        {
            while (gameManager.IsGameInProgress)
            {
                DoBidPhase();
                DoGamePhase();
                StartNewRound();
                if (gameManager.RoundNumber % 1000 == 0)
                    Console.WriteLine("Round " + gameManager.RoundNumber + " of " + gameManager.RoundsToPlay);
            }
        }

        public void StartNewRound()
        {
            gameManager.StartNewRound();
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
            foreach (Player p in gameManager.NonHumanPlayers)
                gameManager.GetAI(p).ProcessOtherPlayerCard(Round.CurrentPlayer, card);
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
