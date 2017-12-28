using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using MovieQuizz2.Helpers;
using QuizzApp.WP.ViewModels;

namespace MovieQuizz2
{
    public partial class PopupSplash : UserControl
    {
        public PopupSplash()
        {
            InitializeComponent();

            
        }

        // Required for performance
        // Set IsIndeterminate = false after collapsing the progressbar. It will run in the background & use battery & UI thread cycles even though its collapsed!
        public void UnloadProgressBar()
        {
            this.progressBar1.IsIndeterminate = false;
        }

        //public PopupSplash(PopupSplashViewModel popupSplashViewModel) 
        //    : this()
        //{
        //    this.LayoutRoot.DataContext = popupSplashViewModel;
        //}

        public PopupSplashViewModel PopupSplashViewModel
        {
            get
            {
                if (this.LayoutRoot.DataContext is PopupSplashViewModel)
                    return (PopupSplashViewModel)this.LayoutRoot.DataContext;
                else
                    return null;
            }
        }
                
    }
}
