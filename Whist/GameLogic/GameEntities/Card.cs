using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic
{
    public enum Numbers
    {
        TWO=2, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING, ACE
    }
    public enum Suits
    {
        HEARTS=1, CLUBS, DIAMONDS, SPADES
    }

    public class Card : IComparer<Card>, IComparable
    {
        private Numbers number;
        public Numbers Number { get { return number; }}
        private Suits suit;
        public Suits Suit { get { return suit; } }

        public Card(Suits suit, Numbers number) {
            this.number = number;
            this.suit = suit;               
        }

        public Card(int suit, int number): this((Suits)suit, (Numbers)number){}

        public int Compare(Card x, Card y)
        {
            if (x.suit < y.suit) return -1;
            if (y.suit < x.suit) return 1;
            if (x.number < y.number) return -1;
            if (y.number < x.number) return 1;
            return 0;
        }

        public override String ToString() {
            return number.ToString() + " of " + suit.ToString();
        }

        public int CompareTo(object obj)
        {
            if(obj is Card)
            { 
                Card y = (Card)obj;
                if (this.suit < y.suit) return -1;
                if (y.suit < this.suit) return 1;
                if (this.number < y.number) return -1;
                if (y.number < this.number) return 1;
            }
            return 0;
        }
    }
}
