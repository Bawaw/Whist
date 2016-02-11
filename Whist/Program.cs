﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist
{
    class Program
    {
        private static WhistController whistController;
        static void Main(string[] args)
        {
            whistController = TestControllerFactory.generate();
            Console.Out.WriteLine("Welcome,");

            while (true)
            {
                Console.Out.WriteLine("Press any key to deal cards");
                Console.In.ReadLine();
                whistController.start();
                Console.Clear();
                do
                {
                    do
                    {
                        int card = -1;
                        do
                        {
                            if (whistController.Pile.Count > 0)
                                Console.Out.WriteLine("pile: {" + PrintList<Card>(whistController.Pile, ",") + "}");
                            Console.Out.WriteLine("Trump: " + whistController.Trump);
                            Console.Out.WriteLine("turn: " + whistController.CurrentPlayer);
                            whistController.CurrentPlayer.hand.sort();
                            Console.Out.WriteLine("cards: \n" + PrintNumberedList<Card>(whistController.GetPlayerCards(), "\n"));
                            Console.Out.WriteLine("What card would you like to play? {0,1,2,3,...}");
                            card = int.Parse(Console.In.ReadLine());
                            Console.Clear();
                        } while (!whistController.PlayCard((whistController.GetPlayerCards())[card]));
                    } while (!whistController.isEndTrick());
                    Console.Out.WriteLine(whistController.EndTrick() + " won the trick. \n Press any key to continue");
                    Console.In.ReadLine();
                } while (!whistController.isEndGame());
                Console.Out.WriteLine(whistController.EndGame().TeamName + " won the game. \n Press any key to continue");
                whistController.EndGame();

            }
        }

        public static String PrintList<T>(IList<T> data, String seperator)
        {
            String line = "";
            foreach (T t in data)
                line += t.ToString() + seperator;
            return line.Remove(line.Length - 1);
        }

        public static String PrintNumberedList<T>(IList<T> data, String seperator)
        {
            String line = "";
            for (int i = 0; i < data.Count; i++)
                line += i + ") " + data[i].ToString() + seperator;

            return line.Remove(line.Length - 1);
        }
    }
}
