using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Whist.GameLogic;
using Whist_GUI.ViewLogic;

namespace Whist_GUI
{
    public class CardImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Whist.GameLogic.Card card = ((CardViewModel) value).Card;
            Card card = value as Card;
            if (card == null) return new Uri(@"Textures\red_back.png", UriKind.Relative);
            //check is face, else number as #
            string number = ((int)card.Number < 11) ? ((int)card.Number).ToString() : card.Number.ToString().ToLower();
            string uglyHack = "";
            if (card.Number >= Numbers.JACK && card.Number < Numbers.ACE)
                uglyHack = "2";
            var uri = string.Format( @"Textures\{0}_of_{1}.png", number, card.Suit.ToString().ToLower() + uglyHack);
            return new Uri(@uri, UriKind.Relative); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("this function is not implemented");
        }
    }
}
