using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace QuizzApp.Core.Helpers.Converters
{
    public class TimespanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null || ((value is TimeSpan) == false))
                return string.Empty;

            TimeSpan val = (TimeSpan) value;
                        
            string fmt = @"hh\h\ mm\m\ ss\s";
            if (parameter != null && parameter.ToString().Equals("Milliseconds", StringComparison.InvariantCultureIgnoreCase))
                fmt = @"hh\h\ mm\m\ ss\.ff\s";

            return val.ToString(fmt);            
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
