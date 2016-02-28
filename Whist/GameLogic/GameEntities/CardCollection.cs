using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic
{
    [Serializable]
    public abstract class CardCollection{
        public static Random rng;
        protected ObservableCollection<Card> deck;
        public int Count { get { return deck.Count; } }

        public CardCollection() {
            rng = new Random();
            deck = new ObservableCollection<Card>(); 
        }

        protected Card Remove(Card card) {
            if (deck.Count <= 0 || !deck.Contains(card))
                return null;
            deck.Remove(card);
            return card;
        }

        public void AddCard(Card card)
        {
            if (!deck.Contains(card))
                deck.Add(card);
        }

        public override string ToString()
        {
            string cardCollection = "";
            deck.ForEach(x => cardCollection += x);
            return cardCollection;
        }
    }

    public class DeckCollection : CardCollection
    {

        //peep at first card without removing it
        public Card peep() {
            return deck[0];
        }

        //Draw a card, card gets removed from deck
        public Card Draw()
        {
            return Remove(deck[0]);
        }

        //Draw #n cards, card gets removed from deck
        public IList<Card> Draw(int n)
        {
            if (deck.Count < n) return null;
            IList<Card> cards = deck.GetRange(0, n-1);
            foreach (Card c in cards)
                deck.Remove(c);
            //deck.RemoveRange(0, n);
            return cards;
        }

        //new deck 
        public void initialise()
        {
            deck.Clear();
            for (int i = 1; i <= Enum.GetNames(typeof(Suits)).Length; i++)
                for (int j = 2; j <= Enum.GetNames(typeof(Numbers)).Length +1; j++)
                    deck.Add(new Card(i,j));
        }

        public void shuffle() {
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }
    }
        
    [Serializable]
    public class HandCollection : CardCollection
    {
        public ObservableCollection<Card> Cards { get { return deck; } }

        public void sort() {
            deck.Sort();
        }

        public Card Play(Card card)
        {
            return Remove(card);
        }

        public void AddCards(IList<Card> cards) {
            cards.ForEach(x => AddCard(x));
        }
    }
}
