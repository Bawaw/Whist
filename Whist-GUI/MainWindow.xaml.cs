using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Whist.GameLogic;

namespace Whist_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private class AnonCard {
            public String uri { get {return "Textures/cardBack_red4.png"; } }
        }

        private class TempEnemy {
            private IList<AnonCard> cards;
            private String name;

            public IList<AnonCard> Cards { get { return cards; } }
            public String Name { get { return name; } }
            public int Score { get { return 100; } }

            public TempEnemy(String name, int cards) {
                this.cards = new List<AnonCard>(Enumerable.Range(0, cards).Select(x => new AnonCard()));
                this.name = name;
            }
        }
        HandCollection handCollection;
        public MainWindow()
        {
            handCollection = new HandCollection();
            handCollection.AddCard(new Card(1, 2));
            handCollection.AddCard(new Card(1, 3));
            handCollection.AddCard(new Card(1, 4));
            handCollection.AddCard(new Card(1, 5));
            handCollection.AddCard(new Card(1, 6));
            handCollection.AddCard(new Card(1, 7));
            handCollection.AddCard(new Card(1, 8));
            handCollection.AddCard(new Card(1, 9));
            handCollection.AddCard(new Card(1, 10));
            handCollection.AddCard(new Card(1, 11));
            handCollection.AddCard(new Card(1, 12));
            handCollection.AddCard(new Card(1, 13));
            handCollection.AddCard(new Card(1, 14));

            handCollection.CardPlayed += HandCollection_CardPlayed;

            InitializeComponent();
            Hand.DataContext = handCollection;

            Enemy_1.DataContext = new TempEnemy("player 1",13);
        }

        private void HandCollection_CardPlayed(object sender, EventArgs e)
        {
            Card card = (Card)sender;
            Hand.DataContext = null;
            Hand.DataContext = handCollection;
            Image CardsImage = new Image();
            CardsImage.Source = new BitmapImage(new Uri(card.uri, UriKind.Relative));
            Canvas.Children.Add(CardsImage);

        }
    }
}
