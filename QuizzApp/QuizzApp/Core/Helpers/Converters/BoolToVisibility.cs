using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace QuizzApp.Core.Helpers.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null)
            {
                return ((bool)value == true) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (parameter.ToString() == "Inverse")
            {
                return ((bool)value == true) ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    
}
