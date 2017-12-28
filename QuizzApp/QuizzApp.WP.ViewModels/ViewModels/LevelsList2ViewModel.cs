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
using QuizzApp.Core.Controllers;
using QuizzApp.WP.ViewModels.Helpers.MigrationManager;
using QuizzApp.WP.ViewModels.Helpers;

namespace QuizzApp.WP.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class LevelsList2ViewModel : QAViewModelBase<LevelsList2ViewModel>
    {
        #region Initialization and Cleanup

        // Default ctor
        public LevelsList2ViewModel(GameProvider gameProvider) 
            : base(gameProvider)
        {
            if (this.IsInDesignMode)
            {
                DesignDataProvider data = new DesignDataProvider();
                          
                return;
            }
        }
        

       

        #endregion

        #region Notifications


        #endregion

        #region Properties

        private ObservableCollection<DifficultyPresenter> difficulties;
        public ObservableCollection<DifficultyPresenter> Difficulties
        {
            get { return difficulties; }
            set
            {
                difficulties = value;
                NotifyPropertyChanged(m => m.Difficulties);
            }
        }

        private ObservableCollection<Pack> allPacks;
        public ObservableCollection<Pack> AllPacks
        {
            get { return allPacks; }
            set
            {
                allPacks = value;
                NotifyPropertyChanged(m => m.AllPacks);
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
            //if (this.Difficulties == null) // test if data is still in memory
                this.Initialize();
        }

        private void Initialize()
        {
            this.LoadAllDifficultiesAndLevels();
        }

        public void LoadDifficultiesAndLevels()
        {
            Task.Factory.StartNew(() => this.LoadAllDifficultiesAndLevels()); 
        }

        public async void LoadAllDifficultiesAndLevels(bool forceServerRefresh = false)
        {
            this.NotifyLongLoadingStart();

            var difficulties = this.GameProvider.GetDifficulties();
            var levels = this.GameProvider.GetBaseLevels();
            var serverLevels = await this.GameProvider.GetDlLevelListAsync(forceServerRefresh);



            var terminatedOnV1 = new MigrationManager().GetMQ1PacksCompleted(); ;

            this.Difficulties = new ObservableCollection<DifficultyPresenter>();
            this.AllPacks = new ObservableCollection<Pack>();


            //List<DifficultyPresenter> diffs = new List<DifficultyPresenter>();
            //List<Pack> packs = new List<Pack>();

            foreach (var item in difficulties)
            {
                DifficultyPresenter difPresenter = new DifficultyPresenter();
                difPresenter.Id = item.Id;
                difPresenter.Name = item.Name;

                List<LevelPresenter> presentedLevels = new List<LevelPresenter>();
                var difLevels = levels.Where(m => m.DifficultyId == item.Id);
               
                foreach (var aBaseLevel in difLevels)
                {
                    Level aLevel = this.GameProvider.GetLevel(aBaseLevel.Id);
                    LevelPresenter levPresent = new LevelPresenter();
                    levPresent.Id = aLevel.Id;
                    levPresent.Number = aLevel.Val;
                    levPresent.IsLocal = true;
                    levPresent.PercentProgress = aLevel.Progression;
                    presentedLevels.Add(levPresent);

                    foreach (var p in aLevel.Packs)
                        this.AllPacks.Add(p);
                }

                foreach (var serverLevel in serverLevels.Where(m => m.DifficultyId == item.Id)) // Adding sever levels
                {                    
                    if (!difLevels.Select(m => m.Id).Contains(serverLevel.Id))
                    {
                        LevelPresenter levPresent = new LevelPresenter();
                        levPresent.Id = serverLevel.Id;
                        levPresent.Number = serverLevel.Value;
                        levPresent.IsLocal = false;
                        presentedLevels.Add(levPresent);
                                                
                        // We update progress percent depending on MovieQuizz1 packs terminated (migration stuff)
                        int nbPacksTerminatedOnV1 = 0;
                        foreach (var aServerPack in serverLevel.Packs)
                        {
                            
                            if (aServerPack.Fextra1.HasValue && terminatedOnV1.Contains(((int)aServerPack.Fextra1.Value)))
                            {
                                nbPacksTerminatedOnV1++;
                                levPresent.PacksIdTerminatedOnV1.Add(aServerPack.Id);
                            }


                            Pack pck = new Pack();
                            pck.Title = aServerPack.Title;
                            pck.IsRemotePack = true;
                            pck.LevelId = aServerPack.LevelId;
                            pck.Difficulty = serverLevel.DifficultyId;
                            for (int i = 0; i < 10; i++)
                                pck.Medias.Add(new Media() { IsCompleted = false });

                            this.AllPacks.Add(pck);
                        }

                        levPresent.PercentProgress = (double) (((double)nbPacksTerminatedOnV1) / serverLevel.Packs.Count);

                        
                    }
                }

                // reorder by value;
                difPresenter.Levels = new ObservableCollection<LevelPresenter>(presentedLevels.OrderBy(m => m.Number));

                this.Difficulties.Add(difPresenter);

                //this.AllPacks = new ObservableCollection<Pack>(packs);
            }


            //this.Difficulties = new ObservableCollection<DifficultyPresenter>(diffs);

            this.NotifyLongLoadingStop();
        }


        public void DownloadLevel(int levelId)
        {
            LevelPresenter level = null;
            foreach (var item in this.Difficulties)
            {
                level = item.Levels.Where(m => m.Id == levelId).FirstOrDefault();
                if (level != null)
                    break;
            }

            if (level != null)
                this.DownloadLevel(level);
        }

        public async void DownloadLevel(LevelPresenter level)
        {
            this.NotifyInstallLevelStart();
            bool res = await this.GameProvider.DownloadAndInstallLevel(level.Id, DonwloadProgressCallback, InstallFilesProgressCallback, new CancellationToken());
            if (res)
            {
                // We set has completed all level terminated on V1
                foreach (var packId in level.PacksIdTerminatedOnV1)
                {
                    var pack = this.GameProvider.GetPackById(packId);
                    if (pack != null)
                        this.GameProvider.SetFullPackCompleted(pack, true);
                    pack.IsRemotePack = false;
                }

                level.IsLocal = true;
            }           
            this.NotifyInstallTerminated(res);
            this.Initialize();
        }

        private void DonwloadProgressCallback(int value)
        {
            this.NotifyDownloadProgress(value);
        }

        private void InstallFilesProgressCallback(int value)
        {
            this.NotifyInstallFilesProgress(value);
        }

        public event EventHandler<NotificationEventArgs<int>> DownloadProgressEvent;
        public event EventHandler<NotificationEventArgs> InstallLevelStartEvent;
        public event EventHandler<NotificationEventArgs<int>> InstallFilesProgress;
        public event EventHandler<NotificationEventArgs<bool>> InstallLevelTerminatedEvent;
        private void NotifyInstallLevelStart()
        {
            Notify(InstallLevelStartEvent, new NotificationEventArgs());
        }

        private void NotifyInstallTerminated(bool result)
        {
            Notify(InstallLevelTerminatedEvent, new NotificationEventArgs<bool>(string.Empty, result));
        }

        private void NotifyDownloadProgress(int result)
        {
            Notify(DownloadProgressEvent, new NotificationEventArgs<int>(string.Empty, result));
        }

        private void NotifyInstallFilesProgress(int result)
        {
            Notify(InstallFilesProgress, new NotificationEventArgs<int>(string.Empty, result));
        }

        public override void CleanViewModel()
        {
            
        }


        // STEP: Add a Navigate method accepting a page name        
        public void NavigateToPacksListPage(int levelId)
        {
            Dictionary<string, object> par = new Dictionary<string, object>();
            par.Add(PacksListPage.PAGE_PARAM_LEVEL_ID_INT, levelId);

            QANavigationHelper.Navigate(PacksListPage.PAGE_NAME, par);
        }


        // STEP: Add a Navigate method accepting a page name        
        public void NavigateToPackagePage(Pack pack)
        {            
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
        }

        
        public void NavigateToLevelsPage()
        {
            //NavigationHelper.Navigate(LevelsPage.PAGE_NAME);
            QANavigationHelper.Navigate(LevelsPage.PAGE_NAME);
        }
        
        #endregion

        #region Completion Callbacks

        #endregion

        #region Helpers

        
        #endregion
    }
}