using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace QuizzApp.Core.Helpers.Converters
{
    public class ByteToStringConverter : IValueConverter
    {       

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long byteCount = 0;
            if (!long.TryParse(value.ToString(), out byteCount))
                return BytesToString(0);

            return BytesToString(byteCount);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        //private static String BytesToString(long byteCount)
        //{
        //    string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        //    if (byteCount == 0)
        //        return "0" + suf[0];
        //    long bytes = Math.Abs(byteCount);
        //    int place = (int) (Math.Floor(Math.Log(bytes, 1024)));
        //    double num = Math.Round(bytes / Math.Pow(1024, place), 0);
        //    return (Math.Sign(byteCount) * num).ToString() + suf[place];
        //}

        public static String BytesToString(long size)
        {
            String[] suffixes = new String[] { "Oct", "Ko", "Mo", "Go", "To" };

            double tmpSize = size;
            int i = 0;

            while (tmpSize >= 1024)
            {
                tmpSize /= 1024.0;
                i++;
            }

            // arrondi à 10^-2
            tmpSize *= 100;
            tmpSize = (int)(tmpSize + 0.5);
            tmpSize /= 100;

            return ((int)tmpSize) + " " + suffixes[i];
        }
    }

    
}
