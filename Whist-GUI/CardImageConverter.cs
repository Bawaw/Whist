using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Whist.GameLogic;

namespace Whist_GUI
{
    public class CardImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Card card = value as Card;
            if (card == null) return new Uri(@"Textures\red_back.png", UriKind.Relative);
            //check is face, else number as #
            String number = ((int)card.Number < 11) ? ((int)card.Number).ToString() : card.Number.ToString().ToLower();
            String uri = "Textures\\" + number + "_of_" + card.Suit.ToString().ToLower() + ".png";
            return new Uri(@uri, UriKind.Relative); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("this function is not implemented");
        }
    }
}
