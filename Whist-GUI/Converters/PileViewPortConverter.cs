using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Whist.GameLogic;
using Whist.GameLogic.ControlEntities;
using Whist_GUI.ViewLogic;

namespace Whist_GUI
{
    public class ColumnConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {    
            var gvm = values[0] as BaseGameViewModel;
            var itemsControl = values[1] as ItemsControl;

            if (gvm == null || itemsControl == null) return null;

            if (gvm.CurrentPlayer.Number == 0) return null;
            switch (gvm.CurrentPlayer.Number)
            {
                case 0:
                    return 1;
                case 1:
                    return 0;
                case 2:
                    return 1;
                case 3:
                    return 2;
                default:
                    break;
            }
            
            return null;   
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RowConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var gvm = values[0] as BaseGameViewModel;
            var itemsControl = values[1] as ItemsControl;

            if (gvm == null || itemsControl == null) return null;

            switch (gvm.CurrentPlayer.Number)
            {
                case 0:
                    return 2;
                case 1:
                    return 1;
                case 2:
                    return 0;
                case 3:
                    return 1;
                default:
                    break;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class AngleConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var gvm = values[0] as BaseGameViewModel;
            var viewport = values[1] as Viewport3D;
            if (gvm == null || viewport == null) return null;

            //Only make visible after rotation
            viewport.Opacity = 1;

            switch (gvm.CurrentPlayer.Number)
            {
                case 0:
                    return new RotateTransform(0,0.5,0.5);
                case 1:
                    return new RotateTransform(90, 0.5, 0.5);
                case 2:
                    return new RotateTransform(0,0.5, 0.5);
                case 3:
                    return new RotateTransform(90, 0.5, 0.5);
                default:
                    break;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
