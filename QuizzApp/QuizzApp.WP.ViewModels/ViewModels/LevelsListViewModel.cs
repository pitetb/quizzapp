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
    public class LevelsListViewModel : QAViewModelBase<LevelsListViewModel>
    {
        #region Initialization and Cleanup

        // Default ctor
        public LevelsListViewModel(GameProvider gameProvider)
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
            this.LoadDifficultiesAndLevels();
        }

        public void LoadDifficultiesAndLevels()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    this.LoadAllDifficultiesAndLevelsAsync();
                }
                catch (Exception)
                {

                }
            });
        }


        private bool isLoadingDifficultiesAndLevel = false;
        public async void LoadAllDifficultiesAndLevelsAsync(bool forceServerRefresh = false)
        {
            Debug.WriteLine("LoadAllDifficultiesAndLevelsAsync Started");

            if (isLoadingDifficultiesAndLevel)
                return;
            
            this.NotifyLongLoadingStart();
            this.isLoadingDifficultiesAndLevel = true;

            try
            {


                List<Difficulty> difficulties = this.GameProvider.GetDifficulties();
                List<BaseLevel> levels = this.GameProvider.GetBaseLevels();

                var serverLevels = await this.GameProvider.GetDlLevelListAsync(forceServerRefresh);



                var terminatedOnV1 = new MigrationManager().GetMQ1PacksCompleted(); ;

                List<DifficultyPresenter> diffs = new List<DifficultyPresenter>();

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


                        for (int i = 0; i < aLevel.Packs.Count && i < 3; i++)
                        {
                            if (i == 0)
                            {
                                levPresent.Title1 = aLevel.Packs[i].Title;
                                levPresent.DifficultyTitle1 = aLevel.Packs[i].Difficulty;
                            }
                            else if (i == 1)
                            {
                                levPresent.Title2 = aLevel.Packs[i].Title;
                                levPresent.DifficultyTitle2 = aLevel.Packs[i].Difficulty;
                            }
                            else if (i == 2)
                            {
                                levPresent.Title3 = aLevel.Packs[i].Title;
                                levPresent.DifficultyTitle3 = aLevel.Packs[i].Difficulty;
                            }
                        }
                        presentedLevels.Add(levPresent);
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
                            }

                            levPresent.PercentProgress = (double)(((double)nbPacksTerminatedOnV1) / serverLevel.Packs.Count);

                            for (int i = 0; i < serverLevel.Packs.Count || i < 3; i++)
                            {
                                if (i == 0)
                                {
                                    levPresent.Title1 = serverLevel.Packs[i].Title;
                                    levPresent.DifficultyTitle1 = serverLevel.Packs[i].Difficulty.HasValue ? (double)serverLevel.Packs[i].Difficulty : 1;
                                }
                                else if (i == 1)
                                {
                                    levPresent.Title2 = serverLevel.Packs[i].Title;
                                    levPresent.DifficultyTitle2 = serverLevel.Packs[i].Difficulty.HasValue ? (double)serverLevel.Packs[i].Difficulty : 1;
                                }
                                else if (i == 2)
                                {
                                    levPresent.Title3 = serverLevel.Packs[i].Title;
                                    levPresent.DifficultyTitle3 = serverLevel.Packs[i].Difficulty.HasValue ? (double)serverLevel.Packs[i].Difficulty : 1;
                                }
                            }

                        }
                    }

                    // reorder by value;
                    difPresenter.Levels = new ObservableCollection<LevelPresenter>(presentedLevels.OrderBy(m => m.Number));

                    diffs.Add(difPresenter);
                }


                this.Difficulties = new ObservableCollection<DifficultyPresenter>(diffs);

            }
            catch (Exception e )
            {
                Debug.WriteLine("LoadAllDifficultiesAndLevelsAsync exception : " + e);
            }

            this.isLoadingDifficultiesAndLevel = false;
            this.NotifyLongLoadingStop();
        }

        public void CancelCurrentDonwloadIfAny()
        {
            if (this.cancellationTokenSource != null)
                this.cancellationTokenSource.Cancel();
        }

        private CancellationTokenSource cancellationTokenSource;

        public async void DownloadLevel(LevelPresenter level)
        {
            this.NotifyInstallLevelStart();
            this.cancellationTokenSource = new CancellationTokenSource();
            bool res = await this.GameProvider.DownloadAndInstallLevel(level.Id, DonwloadProgressCallback, InstallFilesProgressCallback, cancellationTokenSource.Token);
            if (res && this.cancellationTokenSource != null && this.cancellationTokenSource.IsCancellationRequested == false)
            {
                // We set has completed all level terminated on V1
                foreach (var packId in level.PacksIdTerminatedOnV1)
                {
                    var pack = this.GameProvider.GetPackById(packId);
                    if (pack != null)
                        this.GameProvider.SetFullPackCompleted(pack, true);
                }

                level.IsLocal = true;
            }
            this.cancellationTokenSource = null;
            this.NotifyInstallTerminated(res);
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
            if (InstallLevelStartEvent != null)
                Notify(InstallLevelStartEvent, new NotificationEventArgs());
        }

        private void NotifyInstallTerminated(bool result)
        {
            if (InstallLevelTerminatedEvent != null)
                Notify(InstallLevelTerminatedEvent, new NotificationEventArgs<bool>(string.Empty, result));
        }

        private void NotifyDownloadProgress(int result)
        {
            if (DownloadProgressEvent != null)
                Notify(DownloadProgressEvent, new NotificationEventArgs<int>(string.Empty, result));
        }

        private void NotifyInstallFilesProgress(int result)
        {
            if (InstallFilesProgress != null)
                Notify(InstallFilesProgress, new NotificationEventArgs<int>(string.Empty, result));
        }

        public override void CleanViewModel()
        {
            this.CancelCurrentDonwloadIfAny();
        }


        // STEP: Add a Navigate method accepting a page name        
        public void NavigateToPacksListPage(int levelId)
        {
            Dictionary<string, object> par = new Dictionary<string, object>();
            par.Add(PacksListPage.PAGE_PARAM_LEVEL_ID_INT, levelId);

            QANavigationHelper.Navigate(PacksListPage.PAGE_NAME, par);
        }

        public void NavigateToAllPacksListPage()
        {
            //NavigationHelper.Navigate(LevelsPage.PAGE_NAME);
            QANavigationHelper.Navigate("LevelsPage3");
        }

        #endregion

        #region Completion Callbacks

        #endregion

        #region Helpers


        #endregion
    }
}