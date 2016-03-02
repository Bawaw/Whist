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
            arena.stats.PrintResultsToConsole();

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

            gameManager = new GameManager(players, 1001, new AIBidType[]
            {
                AIBidType.BASIC,
                AIBidType.BASIC,
                AIBidType.BASIC,
                AIBidType.BASIC
            }, new AIGameType[]
            {
                AIGameType.PERFECTMEMORY,
                AIGameType.PERFECTMEMORY,
                AIGameType.PERFECTMEMORY,
                AIGameType.PERFECTMEMORY
            });
        }



        public void PlayGame()
        {
            while (gameManager.IsGameInProgress)
            {
                DoBidPhase();
                DoGamePhase();
                stats.ProcessRound(Round);
                StartNewRound();
                if (gameManager.RoundNumber % (gameManager.RoundsToPlay/10) == 0)
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
        public Dictionary<Case, int> gamecaseoccurences = new Dictionary<Case, int>();
        public int[] abondancesPlayed = new int[4];
        public int[] abondancesWon = new int[4];
        public int[] alonePlayed = new int[4];
        public int[] aloneWon = new int[4];
        public int[] teamPlayed = new int[4];
        public int[] teamWon = new int[4];
        public int[] troelPlayed = new int[4];
        public int[] troelWon = new int[4];

        public Dictionary<Case, int[]> gamecaseplaysperplayer = new Dictionary<Case, int[]>();
        public Dictionary<Case, int[]> gamecasewinsperplayer = new Dictionary<Case, int[]>();

        public void ProcessRound(Round round)
        {
            foreach (Player p in round.Players)
            {
                tricksWon[p.Number] += p.Tricks;
            }
            foreach (Team t in round.Teams)
            {
                if (t.Tricks >= t.objective)
                {
                    foreach (Player p in t.Players)
                    {
                        roundsWon[p.Number]++;
                    }
                }
            }
            if (!gamecaseoccurences.ContainsKey(round.GameCase))
                gamecaseoccurences.Add(round.GameCase, 0);
            gamecaseoccurences[round.GameCase]++;

            if (!gamecaseplaysperplayer.ContainsKey(round.GameCase))
            {
                gamecaseplaysperplayer.Add(round.GameCase, new int[] { 0, 0, 0, 0 });
                gamecasewinsperplayer.Add(round.GameCase, new int[] { 0, 0, 0, 0 });
            }

            var team = round.Teams.Where(t => t.objective == round.Teams.Max(tm => tm.objective)).Single();
            foreach (var pl in team.Players)
            {
                gamecaseplaysperplayer[round.GameCase][pl.Number]++;
                if (team.Tricks >= team.objective)
                    gamecasewinsperplayer[round.GameCase][pl.Number]++;
            }
        }

        public void PrintResultsToConsole()
        {
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Player " + (i+1) + ": ");
                Console.WriteLine("-Tricks won " + tricksWon[i]);
                Console.WriteLine("-Rounds won " + roundsWon[i]);
                foreach(var kv in gamecaseplaysperplayer)
                {
                    int percentage = 0;
                    if (kv.Value[i] != 0)
                        percentage = (100 * gamecasewinsperplayer[kv.Key][i]) / kv.Value[i];
                    Console.WriteLine("-" + kv.Key + ": " + gamecasewinsperplayer[kv.Key][i] + "/" + kv.Value[i] + " (" + percentage + "%)");
                }
            }
            Console.WriteLine();
            Console.WriteLine("GameCases:");
            foreach(var c in gamecaseoccurences)
            {
                Console.WriteLine("-" + c.Key + ": " + c.Value);
            }
        }
    }
}
