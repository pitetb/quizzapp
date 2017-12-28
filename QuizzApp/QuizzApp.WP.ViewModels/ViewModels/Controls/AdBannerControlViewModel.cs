using System;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;

// Toolkit namespace
using SimpleMvvmToolkit;

// Toolkit extension methods
using SimpleMvvmToolkit.ModelExtensions;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using QuizzApp.FakeData;
using QuizzApp.Core.Helpers;


namespace QuizzApp.WP.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class AdBannerControlViewModel : ViewModelBaseWithDesign<AdBannerControlViewModel>
    {
        #region Initialization and Cleanup


        // Default ctor
        public AdBannerControlViewModel()
        {

            if (this.IsInDesignMode)
            {
                DesignDataProvider design = new DesignDataProvider();
                this.DesignString = design.DesignString;
            }

            //Random rand = new Random();
            //if (rand.Next(0, 2) == 0)
            //    this.BannerPicture = "/Resources/Images/bannerIphone@2x.png";
            //else
            //    

            this.BannerPicture = "/Resources/fr/Images/banner@2x.png";
            this.UpdateShowAd();
        }

        #endregion

        
        #region Properties

        private string aDesignString;
        public string DesignString
        {
            get { return aDesignString; }
            set
            {
                aDesignString = value;
                NotifyPropertyChanged(m => m.DesignString);
            }
        }

        private string bannerPicture;
        public string BannerPicture
        {
            get { return bannerPicture; }
            set
            {
                bannerPicture = value;
                NotifyPropertyChanged(m => m.BannerPicture);
            }
        }

        private bool? adLoadError;
        public bool? AdLoadError
        {
            get { return adLoadError; }
            set
            {
                adLoadError = value;
                NotifyPropertyChanged(m => m.AdLoadError);
                this.UpdateShowAd();
            }
        }

        private bool showAd;
        public bool ShowAd
        {
            get { return showAd; }
            set
            {
                showAd = value;
                NotifyPropertyChanged(m => m.ShowAd);
            }
        }

        public bool IsNetworkAvailable
        {
            get
            {
                return Microsoft.Phone.Net.NetworkInformation.DeviceNetworkInformation.IsNetworkAvailable || this.IsInDesignMode;
            }
        }


                
        #endregion

        #region Methods

        private void UpdateShowAd()
        {
            if (IsNetworkAvailable && ((AdLoadError == null || AdLoadError == false)))
            {
                this.ShowAd = true;
            }
            else
            {
                this.ShowAd = false;
            }
        }

        #endregion

        #region Completion Callbacks

        #endregion

        #region Helpers

        
        #endregion
    }
}