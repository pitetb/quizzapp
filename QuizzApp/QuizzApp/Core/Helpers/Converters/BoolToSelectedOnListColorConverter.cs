using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace QuizzApp.Core.Helpers.Converters
{
    public class BoolToSelectedOnListColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush brush = Application.Current.Resources["MQItemSelectedOnListBrush"] as Brush;

            if (parameter == null)
            {
                return ((bool)value == true) ? brush : new SolidColorBrush(Colors.Transparent);
            }
            else if (parameter.ToString() == "Inverse")
            {
                return ((bool)value == true) ? new SolidColorBrush(Colors.Transparent) : brush;
            }
            return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    
}
