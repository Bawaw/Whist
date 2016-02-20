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
    public class ViewPortConverter : IMultiValueConverter
    {
        public const float height = 145.2f;
        public const float width = 100;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {    
            var gvm = values[0] as BaseGameViewModel;
            var itemsControl = values[1] as ItemsControl;

            if (gvm == null || itemsControl == null) return null;

            switch (gvm.CurrentPlayer.Number)
            {
                case 1:
                    return 1;
                case 2:
                    return 0;
                case 3:
                    return 1;
                case 4:
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

    public class ViewPortConverter2 : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var gvm = values[0] as BaseGameViewModel;
            var itemsControl = values[1] as ItemsControl;

            if (gvm == null || itemsControl == null) return null;

            switch (gvm.CurrentPlayer.Number)
            {
                case 1:
                    return 2;
                case 2:
                    return 1;
                case 3:
                    return 0;
                case 4:
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


    public class ViewPortConverter3 : IMultiValueConverter
    {
        public const float HEIGHT = 145.2f;
        public const float width = 100;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var gvm = values[0] as BaseGameViewModel;
            var viewport = values[1] as Viewport3D;
            if (gvm == null || viewport == null) return null;

            switch (gvm.CurrentPlayer.Number)
            {
                case 1:
                    return new RotateTransform(0,0.5,0.5);
                case 2:
                    return new RotateTransform(90, 0.5, 0.5);
                case 3:
                    return new RotateTransform(0,0.5, 0.5);
                case 4:
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
