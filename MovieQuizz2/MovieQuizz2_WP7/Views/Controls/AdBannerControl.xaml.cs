using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using QuizzApp.WP.ViewModels;
using MovieQuizz2.Resources;

namespace MovieQuizz2.Views.Controls
{
    public partial class AdBannerControl : UserControl
    {
        public AdBannerControl()
        {
            InitializeComponent();

            this.CurrentViewModel.BannerPicture = AppResources.AdBannerReplacement;
        }

        public AdBannerControlViewModel CurrentViewModel
        {
            get
            {
                if (this.LayoutRoot.DataContext is AdBannerControlViewModel)
                    return (AdBannerControlViewModel)this.LayoutRoot.DataContext;
                else
                    return null;
            }
        }

        #region control properties
        public string AdUnitID
        {
            get { return (string)GetValue(AdUnitIDProperty); }
            set { SetValue(AdUnitIDProperty, value); }
        }

        public static readonly DependencyProperty AdUnitIDProperty =
            DependencyProperty.Register("AdUnitID", typeof(string), typeof(AdBannerControl), new PropertyMetadata(string.Empty, OnAdUnitIDPropertyChanged));

        private static void OnAdUnitIDPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            AdBannerControl myUserControl = dependencyObject as AdBannerControl;
            myUserControl.OnAdUnitIDPropertyChanged(e);
        }

        private void OnAdUnitIDPropertyChanged(DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion
    }
}
