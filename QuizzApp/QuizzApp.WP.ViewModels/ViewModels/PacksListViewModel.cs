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
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuizzApp.Tombstone;
using QuizzApp.Core.Entities;
using QuizzApp.Core.Helpers.Collections;
using QuizzApp.FakeData;
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
    public class PacksListViewModel : QAViewModelBase<PacksListViewModel>
    {
        #region Initialization and Cleanup

        // Default ctor
        public PacksListViewModel(GameProvider gameProvider)
            : base(gameProvider)
        {
            if (this.IsInDesignMode)
            {
                DesignDataProvider data = new DesignDataProvider();
                this.CurrentLevel = data.ALevel;
                this.Packs = new AsyncObservableCollection<Pack>(data.APackageList);
                return;
            }

            
        }
        

       

        #endregion

        #region Notifications


        #endregion

        #region Properties


        private int currentLevelId;
        [Persist(PersistMode.TombstonedOnly)]
        public int CurrentLevelId
        {
            get { return currentLevelId; }
            set
            {
                currentLevelId = value;
                NotifyPropertyChanged(m => m.CurrentLevelId);
            }
        }


        private Level currentLevel;
        public Level CurrentLevel
        {
            get { return currentLevel; }
            set
            {
                currentLevel = value;
                NotifyPropertyChanged(m => m.CurrentLevel);
            }
        }
        

        

        private AsyncObservableCollection<Pack> packs;
        public AsyncObservableCollection<Pack> Packs
        {
            get { return packs; }
            set
            {
                if (value != packs)
                {
                    packs = value;
                    NotifyPropertyChanged(m => m.Packs);
                }
            }
        }
        
        #endregion


        #region Methods


        public override void OnActivated(bool preserved)
        {
            base.OnActivated(preserved);
            if (!preserved)
                this.Initialize();
        }

        public override void OnLaunched()
        {
            base.OnLaunched();
            this.Initialize();
        }

        private void Initialize()
        {
            this.LoadLevelAndPack();
        }



        private void LoadLevelAndPack()
        {
            this.Packs = new AsyncObservableCollection<Pack>();
            this.NotifyLongLoadingStart();

            

            Task.Factory.StartNew(() =>
            {
                try
                {
                    Level aLevel = this.GameProvider.GetLevel(this.currentLevelId);
                    this.CurrentLevel = aLevel;
                    this.Packs = new AsyncObservableCollection<Pack>(aLevel.Packs.OrderBy(m => m.Difficulty));
                }
                catch (Exception)
                {                   
                   
                }                
                this.NotifyLongLoadingStop();               
            });
        }

        
        public override void CleanViewModel()
        {
            this.Packs = new AsyncObservableCollection<Pack>();
        }


        // STEP: Add a Navigate method accepting a page name        
        public void NavigateToPackagePage(Pack pack)
        {

            //if (pack.IsTerminated)
            //{
            //    Dictionary<string, object> parameters = new Dictionary<string, object>();
            //    parameters.Add(PackageCompletedPage.PAGE_PARAM_PACKID, pack.Id);
            //    NavigationHelper.Navigate(typeof(PackageCompletedPage).Name, parameters);
            //}
            //else
            //{

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add(GamePage.PAGE_PARAM_PACKID, pack.Id);

                if (AppSettings.Instance.HasHelpPageAlreadyBeSeen)
                {
                    QANavigationHelper.Navigate(GamePage.PAGE_NAME, parameters);
                }
                else
                {
                    Dictionary<string, object> parametersHelp = new Dictionary<string, object>();
                    parametersHelp.Add(HelpPage.PAGE_PARAM_NEXT_PAGE_NAME, typeof(GamePage).Name);
                    parametersHelp.Add(HelpPage.PAGE_PARAM_NEXT_PAGE_PARAMS, parameters);
                    QANavigationHelper.Navigate(typeof(HelpPage).Name, parametersHelp);
                    AppSettings.Instance.HasHelpPageAlreadyBeSeen = true;
                }
            //}
        }

        public void ReinitPack(Pack pack)
        {
            if (! this.Packs.Contains(pack))
                return;

            this.GameProvider.SetFullPackCompleted(pack, false);

            this.Initialize();
        }

        public void FinishPack(Pack pack)
        {
            if (!this.Packs.Contains(pack))
                return;

            this.GameProvider.SetFullPackCompleted(pack, true);

            this.Initialize();
        }

        #endregion

        #region Completion Callbacks

        #endregion

        #region Helpers

        
        #endregion
    }
}