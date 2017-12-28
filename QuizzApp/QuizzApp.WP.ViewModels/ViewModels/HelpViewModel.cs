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
using System.ComponentModel;
using System.Windows.Media;
using QuizzApp.FakeData;
using System.Windows.Navigation;
using QuizzApp.Core.Helpers;
using System.Collections.Generic;
using QuizzApp.Tombstone;
using Newtonsoft.Json;
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
    public class HelpViewModel : QAViewModelBase<HelpViewModel>
    {
        #region Initialization and Cleanup

        // Default ctor
        public HelpViewModel(GameProvider gameProvider)
            : base(gameProvider)
        {
            if (this.IsInDesignMode)
            {
                DesignDataProvider data = new DesignDataProvider();
                return;
            }

        }


        public override void OnActivated(bool preserved)
        {
            base.OnActivated(preserved);
            this.UpdateDictionaryFromPersistentItems();
        }

        #endregion

        #region Notifications

       
        #endregion

        #region Properties


        private string nextPageName;
        [Persist(PersistMode.TombstonedOnly)]
        public string NextPageName
        {
            get { return nextPageName; }
            set
            {
                nextPageName = value;
                NotifyPropertyChanged(m => m.NextPageName);
            }
        }

        private IDictionary<string, object> nextPageParams = new Dictionary<string, object>();
        public IDictionary<string, object> NextPageParams
        {
            get { return nextPageParams; }
            set
            {
                nextPageParams = value;
                NotifyPropertyChanged(m => m.NextPageParams);
                this.UpdateNextPageParamsPersistentItems();
            }
        }

        [Persist(PersistMode.TombstonedOnly)]
        public string NextPageParamsAsString { get; set; }


        private void UpdateNextPageParamsPersistentItems()
        {
            this.NextPageParamsAsString = string.Empty;

            if (this.NextPageParams != null)
            {
                this.NextPageParamsAsString = JsonConvert.SerializeObject(this.NextPageParams, Formatting.None);
            }
        }

        private void UpdateDictionaryFromPersistentItems()
        {
            if (this.NextPageParamsAsString == null)
                return;
            
            var items = JsonConvert.DeserializeObject<IDictionary<string, object>>(this.NextPageParamsAsString);
            this.NextPageParams = items;            
        }




        

       
        private int selectedPageIndex = 0;
        [Persist(PersistMode.TombstonedOnly)]
        public int SelectedPageIndex
        {
            get { return selectedPageIndex; }
            set
            {
                selectedPageIndex = value;
                NotifyPropertyChanged(m => m.SelectedPageIndex);
            }
        }



        private string debug = "Tirlintiton";
        public string Debug
        {
            get { return debug; }
            set
            {
                debug = value;
                NotifyPropertyChanged(m => m.Debug);
            }
        }

        #endregion


        #region Methods

        


        public void GoToPreviousPage()
        {
            if (this.NextPageName != null && !string.IsNullOrEmpty(NextPageName))
            {
                nextPageParams.Add(QANavigationHelper.PAGE_PARAM_REMOVE_PREVIOUS_PAGE_FROM_HISTORY, true);
                QANavigationHelper.Navigate(nextPageName, nextPageParams);
            }
            else
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add(QANavigationHelper.PAGE_PARAM_REMOVE_PREVIOUS_PAGE_FROM_HISTORY, true);
                QANavigationHelper.Navigate(MainPage.PAGE_NAME, parameters);
            }
        }

        #endregion

        #region Completion Callbacks

        #endregion

       
    }
}