using System;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;

// Toolkit namespace
using SimpleMvvmToolkit;

// Toolkit extension methods
using SimpleMvvmToolkit.ModelExtensions;
using System.Diagnostics;
using QuizzApp.FakeData;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuizzApp.Tombstone;
using QuizzApp.Core.Helpers.Collections;
using QuizzApp.Core.Helpers;
using QuizzApp.Core.Entities;
using System.Reflection;
using Microsoft.Phone.Tasks;
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
    public class SettingsViewModel : QAViewModelBase<SettingsViewModel>
    {
        
        #region Initialization and Cleanup

        // Default ctor
        public SettingsViewModel(GameProvider gameProvider)
            : base(gameProvider)
        {
            if (this.IsInDesignMode)
            {
                DesignDataProvider data = new DesignDataProvider(); 
                return;
            }

            this.VersionAppli = GetVersionNumber();
        }


        private static string GetVersionNumber()
        {
            var asm = Assembly.GetExecutingAssembly();
            var parts = asm.FullName.Split(',');
            return parts[1].Split('=')[1];
        }

        #endregion

        #region Notifications


        #endregion

        #region Properties

        private string versionAppli;
        public string VersionAppli
        {
            get { return versionAppli; }
            set
            {
                versionAppli = value;
                NotifyPropertyChanged(m => m.VersionAppli);
            }
        }

        #endregion


        #region Methods

        public void NavigateToProposePack()
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(Constants.PROPOSE_PACK_URL);
            wbt.Show();
        }

        public void NavigateToQuizzAppWebSite()
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(Constants.QUIZZ_APP_WEB_SITE);
            wbt.Show();
        }

        public void NavigateToHelpPage()
        {
            Dictionary<string, object> pageParams = new Dictionary<string, object>();
            pageParams.Add(HelpPage.PAGE_PARAM_NEXT_PAGE_NAME, SettingsPage.PAGE_NAME);

            Dictionary<string, object> pageParams2 = new Dictionary<string, object>();
            pageParams2.Add("essai", "toto");
            pageParams.Add(HelpPage.PAGE_PARAM_NEXT_PAGE_PARAMS, pageParams2);

            QANavigationHelper.Navigate(HelpPage.PAGE_NAME, pageParams);
        }

        public override void OnActivated(bool preserved)
        {
            base.OnActivated(preserved);
            if (!preserved)
                this.Initialize();
        }

        public override void OnLaunched()
        {
            base.OnLaunched();           
        }

        private void Initialize()
        {
            
        }

        
        #endregion

        #region Completion Callbacks

        #endregion

        #region Helpers

        
        #endregion
    }
}