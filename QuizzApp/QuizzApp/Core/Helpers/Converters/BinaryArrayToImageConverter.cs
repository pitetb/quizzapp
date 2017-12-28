using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace QuizzApp.Core.Helpers.Converters
{
    public class BinaryArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is byte[])
            {
                byte[] bytes = value as byte[];
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    stream.Position = 0;
                    BitmapImage image = new BitmapImage();
                    image.SetSource(stream);
                    return image;
                }
                
            }

            return null;
        }

        

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
