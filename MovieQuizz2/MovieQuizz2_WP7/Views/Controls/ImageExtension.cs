using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MovieQuizz2
{
    public static class ImageExtension
    {

        #region MultiResolutionUri
        public static readonly DependencyProperty MultiResolutionUriProperty =
         DependencyProperty.RegisterAttached("MultiResolutionUri",
         typeof(Uri),
         typeof(ImageExtension),
         new PropertyMetadata(MultiResolutionUriChanged));

        // Called when Property is retrieved
        public static Uri GetMultiResolutionUri(DependencyObject obj)
        {
            return obj.GetValue(MultiResolutionUriProperty) as Uri;
        }

        // Called when Property is set
        public static void SetMultiResolutionUri(
          DependencyObject obj,
          Uri value)
        {
            obj.SetValue(MultiResolutionUriProperty, value);
        }

        // Called when property is changed
        private static void MultiResolutionUriChanged(
         object sender,
         DependencyPropertyChangedEventArgs args)
        {
            Image attachedObject = sender as Image;
            if (attachedObject == null) return;

            Uri uri =  attachedObject.GetValue(MultiResolutionUriProperty) as Uri;
            if (uri != null)
            {
                string[] originalStringSplitted = uri.OriginalString.Split('.');
                if (originalStringSplitted.Length < 2)
                    return;


                switch (ResolutionHelper.CurrentResolution)
                {
                    case Resolutions.HD:
                        originalStringSplitted[originalStringSplitted.Length - 2] = originalStringSplitted[originalStringSplitted.Length - 2] + ".Screen-720";
                        break;
                    case Resolutions.WXGA:
                        originalStringSplitted[originalStringSplitted.Length - 2] = originalStringSplitted[originalStringSplitted.Length - 2] + ".Screen-wxga";
                        break;
                    case Resolutions.WVGA:
                        originalStringSplitted[originalStringSplitted.Length - 2] = originalStringSplitted[originalStringSplitted.Length - 2] + ".Screen-wvga";
                        break;
                    default:
                        throw new InvalidOperationException("Unknown resolution type");
                }
                
                string newUri = String.Join(".", originalStringSplitted);
                attachedObject.Source = new BitmapImage(new Uri(newUri, UriKind.RelativeOrAbsolute));
            }
        }
        #endregion

    }

    public enum Resolutions { WVGA, WXGA, HD };

    public static class ResolutionHelper
    {
        private static bool IsWvga
        {
            get
            {
#if WINDOWS_PHONE7
                return true;
#else
                return App.Current.Host.Content.ScaleFactor == 100;
#endif
            }
        }

        private static bool IsWxga
        {
            get
            {
#if WINDOWS_PHONE7
                return false;
#else
                return App.Current.Host.Content.ScaleFactor == 160;
#endif
            }
        }

        private static bool IsHD
        {
            get
            {
#if WINDOWS_PHONE7
                return false;
#else
                return App.Current.Host.Content.ScaleFactor == 150;
#endif
            }
        }

        public static Resolutions CurrentResolution
        {
            get
            {
                if (IsWvga) return Resolutions.WVGA;
                else if (IsWxga) return Resolutions.WXGA;
                else if (IsHD) return Resolutions.HD;
                else throw new InvalidOperationException("Unknown resolution");
            }
        }
    }

}
