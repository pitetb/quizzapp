using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using QuizzApp.Core.Entities;
using QuizzApp.WP.ViewModels;

namespace MovieQuizz2
{
    public partial class ThreePosterControl : UserControl
    {
        public ThreePosterControl()
        {
            InitializeComponent();
        }


        public ThreePosterViewModel CurrentViewModel
        {
            get
            {
                if (this.LayoutRoot.DataContext is ThreePosterViewModel)
                    return (ThreePosterViewModel)this.LayoutRoot.DataContext;
                else
                    return null;
            }
        }


        
        


        public MediaImage Poster1
        {
            get { return (MediaImage)GetValue(Poster1Property); }
            set { SetValue(Poster1Property, value); }
        }

        // Using a DependencyProperty as the backing store for Poster1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Poster1Property =
            DependencyProperty.Register("Poster1", typeof(MediaImage), typeof(ThreePosterControl), new PropertyMetadata(null, OnPoster1PropertyChanged));

        private static void OnPoster1PropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ThreePosterControl myUserControl = dependencyObject as ThreePosterControl;
            myUserControl.OnPoster1PropertyChanged(e);
        }

        private void OnPoster1PropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CurrentViewModel != null)
                this.CurrentViewModel.Poster1 = ((MediaImage)e.NewValue);
        }


        public MediaImage Poster2
        {
            get { return (MediaImage)GetValue(Poster2Property); }
            set { SetValue(Poster2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Poster1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Poster2Property =
            DependencyProperty.Register("Poster2", typeof(MediaImage), typeof(ThreePosterControl), new PropertyMetadata(null, OnPoster2PropertyChanged));

        private static void OnPoster2PropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ThreePosterControl myUserControl = dependencyObject as ThreePosterControl;
            myUserControl.OnPoster2PropertyChanged(e);
        }

        private void OnPoster2PropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CurrentViewModel != null)
                this.CurrentViewModel.Poster2 = ((MediaImage)e.NewValue);
        }


        public MediaImage Poster3
        {
            get { return (MediaImage)GetValue(Poster2Property); }
            set { SetValue(Poster2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Poster1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Poster3Property =
            DependencyProperty.Register("Poster3", typeof(MediaImage), typeof(ThreePosterControl), new PropertyMetadata(null, OnPoster3PropertyChanged));

        private static void OnPoster3PropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ThreePosterControl myUserControl = dependencyObject as ThreePosterControl;
            myUserControl.OnPoster3PropertyChanged(e);
        }

        private void OnPoster3PropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CurrentViewModel != null)
                this.CurrentViewModel.Poster3 = ((MediaImage)e.NewValue);
        }

        
    }

}
