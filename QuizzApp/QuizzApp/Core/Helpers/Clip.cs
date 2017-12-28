using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace QuizzApp.Core.Helpers
{
    public class Clip
    {
        public static bool GetToBounds(DependencyObject depObj)
        {
            return (bool)depObj.GetValue(ToBoundsProperty);
        }

        public static void SetToBounds(DependencyObject depObj, bool clipToBounds)
        {
            depObj.SetValue(ToBoundsProperty, clipToBounds);
        }

        /// <summary>
        /// Identifies the ToBounds Dependency Property.
        /// <summary>
        public static readonly DependencyProperty ToBoundsProperty =
            DependencyProperty.RegisterAttached("ToBounds", typeof(bool),
            typeof(Clip), new PropertyMetadata(false, OnToBoundsPropertyChanged));


        private static void OnToBoundsPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe != null)
            {
                ClipToBounds(fe);

                // whenever the element which this property is attached to is loaded
                // or re-sizes, we need to update its clipping geometry
                fe.Loaded += new RoutedEventHandler(fe_Loaded);
                fe.SizeChanged += new SizeChangedEventHandler(fe_SizeChanged);

            }
        }

        public static double GetRadiusX(DependencyObject depObj)
        {
            return (double)depObj.GetValue(RadiusXProperty);
        }

        public static void SetRadiusX(DependencyObject depObj, double val)
        {
            depObj.SetValue(RadiusXProperty, val);
        }

        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.RegisterAttached("RadiusX", typeof(double),
            typeof(Clip), new PropertyMetadata(0.0d, OnRadiusXPropertyChanged));


        private static void OnRadiusXPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe != null)
            {
                ClipToBounds(fe);                
            }
        }


        public static double GetRadiusY(DependencyObject depObj)
        {
            return (double)depObj.GetValue(RadiusYProperty);
        }

        public static void SetRadiusY(DependencyObject depObj, double val)
        {
            depObj.SetValue(RadiusYProperty, val);
        }

        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.RegisterAttached("RadiusY", typeof(double),
            typeof(Clip), new PropertyMetadata(0.0d, OnRadiusYPropertyChanged));


        private static void OnRadiusYPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe != null)
            {
                ClipToBounds(fe);
            }
        }


        /// <summary>
        /// Creates a rectangular clipping geometry which matches the geometry of the
        /// passed element
        /// </summary>
        private static void ClipToBounds(FrameworkElement Element)
        {            
            if (GetToBounds(Element))
            {
                Element.Clip = new RectangleGeometry()
                {
                    Rect = new Rect(0, 0, Element.ActualWidth, Element.ActualHeight)
                };

                var rect = Element.Clip as RectangleGeometry;
                rect.RadiusX = GetRadiusX(Element);
                rect.RadiusY = GetRadiusY(Element);
            }
            else
            {
                Element.Clip = null;
            }
        }

        static void fe_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClipToBounds(sender as FrameworkElement);
        }

        static void fe_Loaded(object sender, RoutedEventArgs e)
        {
            ClipToBounds(sender as FrameworkElement);
        }
        
    }
}
