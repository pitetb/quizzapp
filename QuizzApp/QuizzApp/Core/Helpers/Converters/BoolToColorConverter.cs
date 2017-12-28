using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace QuizzApp.Core.Helpers.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null)
            {
                return ((bool)value == true) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }
            else if (parameter.ToString() == "Inverse")
            {
                return ((bool)value == true) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
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
