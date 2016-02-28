using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whist.AIs;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;

namespace Whist
{
    class Program
    {
        static void Main(string[] args) {
            var testCard1 = new Card(Suits.HEARTS, Numbers.JACK);
            var testCard2 = new Card(Suits.SPADES, Numbers.KING);

            PlaySearchAI search = new PlaySearchAI(new StandardReferee());

            HandCollection coll1 = new HandCollection();
            coll1.AddCards(new List<Card>(new Card[] { new Card(Suits.DIAMONDS, Numbers.KING), new Card(Suits.CLUBS, Numbers.FOUR) }));

            HandCollection coll2 = new HandCollection();
            coll2.AddCards(new List<Card>(new Card[] { new Card(Suits.DIAMONDS, Numbers.QUEEN), new Card(Suits.HEARTS, Numbers.THREE) }));

            HandCollection coll3 = new HandCollection();
            coll3.AddCards(new List<Card>(new Card[] { testCard1, testCard2 }));

            HandCollection coll4 = new HandCollection();
            coll4.AddCards(new List<Card>(new Card[] { new Card(Suits.SPADES, Numbers.ACE), new Card(Suits.CLUBS, Numbers.KING) }));

            search.TestMove(testCard1, new HandCollection[] { coll1, coll2, coll3, coll4 },2);

        }
        /*
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
        }*/
    }
}
