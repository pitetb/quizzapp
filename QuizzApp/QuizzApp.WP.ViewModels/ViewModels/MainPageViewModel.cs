using System;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;

// Toolkit namespace
using SimpleMvvmToolkit;

// Toolkit extension methods
using SimpleMvvmToolkit.ModelExtensions;
using System.Diagnostics;
using System.IO;
using QuizzApp.Core.Helpers;
using QuizzApp.Core.Controllers;
using QuizzApp.WP.ViewModels.Helpers;

namespace QuizzApp.WP.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class MainPageViewModel : QAViewModelBase<MainPageViewModel>
    {
        #region Initialization and Cleanup


        // Default ctor
        public MainPageViewModel(GameProvider gameProvider)
            : base(gameProvider)
        {

            if (this.IsInDesignMode)
            {
                this.AppTitle = "Simple MVVM Toolkit for WP7 DSM";
            }
        }

        #endregion

        #region Notifications

        
        #endregion

        #region Properties

        private string appTitle = "Simple MVVM Toolkit for WP7";
        public string AppTitle
        {
            get
            { 
                return appTitle;
            }
            set
            {
                appTitle = value;
                NotifyPropertyChanged(m => m.AppTitle);
            }
        }

        // Add Header property using the mvvmprop code snippet
        private string pageTitle = "hello mvvm";
        public string PageTitle
        {
            get
            {
                if (IsInDesignMode) return "page title";
                return pageTitle;
            }
            set
            {
                pageTitle = value;
                NotifyPropertyChanged(m => m.PageTitle);
            }
        }

        
        #endregion

        #region Methods


        // STEP: Add a Navigate method accepting a page name
        public void NavigateToLevelsPage()
        {
            QANavigationHelper.Navigate(LevelsPage.PAGE_NAME);
            //NavigationHelper.Navigate("LevelsPage3");
        }

        public void NavigateToSettingsPage()
        {
            QANavigationHelper.Navigate(SettingsPage.PAGE_NAME);
        }


        public int GetTotalPoints()
        {           
            int nbPoints = 0;
            var levels = this.GameProvider.GetBaseLevels();
            foreach (var item in levels)
            {
                var aLevel = this.GameProvider.GetLevel(item.Id);
                foreach (var aPack in aLevel.Packs)
                {
                    if (aPack.IsTerminated)
                        nbPoints += aPack.PossiblePoints;
                }
            }

            return nbPoints;
        }

        #endregion

        #region Completion Callbacks

        #endregion
        
    }
}