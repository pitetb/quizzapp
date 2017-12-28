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
using QuizzApp.Core.Entities;
using QuizzApp.FakeData;


namespace QuizzApp.WP.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class ThreePosterViewModel : ViewModelBaseWithDesign<ThreePosterViewModel>
    {
        #region Initialization and Cleanup


        // Default ctor
        public ThreePosterViewModel()
        {

            if (this.IsInDesignMode)
            {
                DesignDataProvider design = new DesignDataProvider();
                //this.Poster1 = design.Poster1;
                //this.Poster2 = design.Poster2;
                //this.Poster3 = design.Poster3;

                this.DesignString = design.DesignString;
                this.Poster1 = design.Poster1;
                this.Poster2 = design.Poster2;
                this.Poster3 = design.Poster3;
                //this.DesignString = "" + this.Poster1.Image.PixelWidth;
            }
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


        private MediaImage poster1;
        public MediaImage Poster1
        {
            get { return poster1; }
            set
            {
                poster1 = value;
                NotifyPropertyChanged(m => m.Poster1);
            }
        }

        private MediaImage poster2;
        public MediaImage Poster2
        {
            get { return poster2; }
            set
            {
                poster2 = value;
                NotifyPropertyChanged(m => m.Poster2);
            }
        }


        private MediaImage poster3;
        public MediaImage Poster3
        {
            get { return poster3; }
            set
            {
                poster3 = value;
                NotifyPropertyChanged(m => m.Poster3);
            }
        }

                
        #endregion

        #region Methods



        #endregion

        #region Completion Callbacks

        #endregion

        #region Helpers

        
        #endregion
    }
}