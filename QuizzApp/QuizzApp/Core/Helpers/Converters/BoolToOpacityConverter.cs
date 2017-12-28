using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace QuizzApp.Core.Helpers.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool?)value;
            double parameterValue = 1;
            if (parameter is string)
                parameterValue = double.Parse((string)parameter, CultureInfo.InvariantCulture);
            
            if (boolValue.HasValue)
            {
                if (boolValue.Value)
                    return 1;
                else
                    return parameterValue;
            }
            else
                return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class BoolToOpacityConverterReverse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool?)value;
            double parameterValue = 1;
            if (parameter is string)
                parameterValue = double.Parse((string)parameter, CultureInfo.InvariantCulture);

            if (boolValue.HasValue)
            {
                if (boolValue.Value == false)
                    return 1;
                else
                    return parameterValue;
            }
            else
                return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
