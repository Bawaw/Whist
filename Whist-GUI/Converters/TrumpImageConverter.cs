﻿using System;
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
    public class TrumpImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //check is face, else number as #
            //string uri = "Textures\\" + number + "_of_" + card.Suit.ToString().ToLower() + ".png";
            return new Uri(@"Textures\" + value + ".png",UriKind.Relative);//new Uri(@uri, UriKind.Relative); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("this function is not implemented");
        }
    }
}
