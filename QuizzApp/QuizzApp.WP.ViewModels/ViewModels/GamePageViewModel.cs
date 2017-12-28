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
using System.Linq;
using System.Windows.Threading;
using System.Collections.Generic;
using QuizzApp.Core.Helpers;
using System.Threading.Tasks;
using QuizzApp.Tombstone;
using QuizzApp.FakeData;
using QuizzApp.Core.Entities;
using QuizzApp.Core.Controllers;
using QuizzApp.WP.ViewModels.Helpers;
using Microsoft.Phone.Info;


namespace QuizzApp.WP.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class GamePageViewModel : QAViewModelBase<GamePageViewModel>
    {

        public static int CORRECT_ANSWER = 1;
        public static int BAD_ANSWER = 0;
        public static int CLOSED_ANSWER = -1;


        #region Initialization and Cleanup

        // Default ctor
        public GamePageViewModel(GameProvider gameProvider)
            : base(gameProvider)
        {

            if (this.IsInDesignMode)
            {
                DesignDataProvider data = new DesignDataProvider();
                this.Pack = data.APackage;
                //this.APoster = this.APackage.Movies[0].Poster;
                //this.APoster2 = this.APackage.Movies[0].Poster;
                //this.APoster3 = this.APackage.Movies[0].Poster;

                Random rand = new Random();
                if (rand.Next(0, 2) == 0)
                    this.BannerPicture = "/Resources/Images/bannerIphone@2x.png";
                else
                    this.BannerPicture = "/Resources/Images/banner@2x.png";

                return;
            }

            //this.LoadPack(PackagesManager.Instance.GetPackagesOfType(0)[0]);
            //this.BannerPicture = new Uri("Resources/Images/banner@2x.png");
            //this.APoster = this.APackage.Movies[0].Poster;
            //this.APoster2 = this.APackage.Movies[1].Poster;
            //this.APoster3 = this.APackage.Movies[2].Poster;


            //Random rand = new Random();
            //if (rand.Next(0, 2) == 0)
            //this.BannerPicture = "Resources/Images/banner@2x.png";
            //else
            //    this.BannerPicture = "/Resources/Images/bannerIphone@2x.png";
        }




        private int? currentPackId;
        [Persist(PersistMode.TombstonedOnly)]
        public int? CurrentPackId
        {
            get { return currentPackId; }
            set
            {
                currentPackId = value;
                NotifyPropertyChanged(m => m.CurrentPackId);
            }
        }


        #endregion

        #region Notifications

        // Add events to notify the view or obtain data from the view        
        public event EventHandler<NotificationEventArgs> BadAnswer;
        public event EventHandler<NotificationEventArgs> GoodAnswer;
        public event EventHandler<NotificationEventArgs> ClosedAnswer;
        public event EventHandler<NotificationEventArgs> PackTerminated;

        #endregion

        #region Properties



        private Pack pack;
        public Pack Pack
        {
            get { return pack; }
            set
            {
                if (value != pack)
                {
                    pack = value;
                    NotifyPropertyChanged(m => m.Pack);

                    Media selectedM;
                    if (pack != null)
                    {
                        if (this.SelectedMediaId != null) // case rehydratation
                        {
                            selectedM = this.Pack.Medias.Where(m => m.Id == this.SelectedMediaId).FirstOrDefault();
                        }
                        else
                        {
                            selectedM = this.Pack.Medias.Where(m => m.IsCompleted == false).FirstOrDefault();

                        }

                        if (selectedM == null && this.Pack.Medias.Count > 0)
                            selectedM = this.Pack.Medias[0];

                        this.CurrentPackId = Pack.Id;
                    }
                    else
                    {
                        selectedM = null;
                        this.CurrentPackId = null;
                    }
                    
                    this.SelectedMedia = selectedM;
                }
            }
        }

        [Persist(PersistMode.TombstonedOnly)]
        public int? SelectedMediaId { get; set; }


        private Media selectedMedia;
        public Media SelectedMedia
        {
            get { return selectedMedia; }
            set
            {
                if (selectedMedia != null)
                {
                    // Stop timers
                    this.StopAndCleanHelpTimer();
                }

                selectedMedia = value;
                NotifyPropertyChanged(m => m.SelectedMedia);

                if (selectedMedia != null)
                {
                    Debug.WriteLine("SelectedMedia : " + selectedMedia.Title);

                    // Lauch Timers
                    this.LaunchHelpTimer();
                    this.SelectedMediaId = selectedMedia.Id;
                    this.SelectedMediaIndex = this.Pack.Medias.IndexOf(this.SelectedMedia) + 1 ;
                }
                else
                {
                    Debug.WriteLine("SelectedMedia : " + null);
                    this.SelectedMediaId = null;
                    this.SelectedMediaIndex = 0;
                }
            }
        }

        private int selectedMediaIndex;
        public int SelectedMediaIndex
        {
            get { return selectedMediaIndex; }
            set
            {
                selectedMediaIndex = value;
                NotifyPropertyChanged(m => m.SelectedMediaIndex);
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



        public bool IsNetworkAvailable
        {
            get
            {
                return Microsoft.Phone.Net.NetworkInformation.DeviceNetworkInformation.IsNetworkAvailable;
            }
        }


        public bool IsSoundActivated
        {
            get
            {
                return AppSettings.Instance.SoundOnOffSetting;
            }
        }

        #endregion


        #region Methods


        public override void OnActivated(bool preserved)
        {
            Debug.WriteLine("OnActivated");
            this.Initialize();
        }

        public override void OnClosing()
        {
            Debug.WriteLine("OnClosing");
            this.CleanViewModel();
        }

        public override void OnDeactivated()
        {
            Debug.WriteLine("OnDeactivated");
        }

        public override void OnLaunched()
        {
            Debug.WriteLine("OnLaunched");
            this.Initialize();
        }



        private async void Initialize()
        {
            this.NotifyLongLoadingStart();
            this.Pack = await this.LoadPackByCurrentPackId();
            this.NotifyLongLoadingStop();

            Random rand = new Random();
            if (rand.Next(0, 2) == 0)
                this.BannerPicture = "/Resources/Images/bannerIphone@2x.png";
            else
                this.BannerPicture = "/Resources/Images/banner@2x.png";
        }

        private async Task<Pack> LoadPackByCurrentPackId()
        {
            Pack returnedPack = null;
            await Task.Factory.StartNew(() =>
            {
                if (this.currentPackId != null)
                    returnedPack = this.GameProvider.GetPackById(CurrentPackId.Value);                
            });
            return returnedPack;
        }


        // Help Timer
        DispatcherTimer helpTimer;
        private void LaunchHelpTimer()
        {
            if (this.SelectedMedia == null || IsInDesignMode)
                return;

            // Calculate Time until help        
            TimeSpan timeSpan = TimeSpan.FromSeconds(this.SelectedMedia.GetTotalSecondsBeforeHelp()) - this.SelectedMedia.Time;

            if (timeSpan.TotalMilliseconds > 0)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    helpTimer = new DispatcherTimer();
                    helpTimer.Interval = timeSpan;
                    helpTimer.Tick += HelpTimerCallBack;
                    helpTimer.Start();
                });
            }
        }

        private void StopAndCleanHelpTimer()
        {
            if (this.helpTimer == null || IsInDesignMode)
                return;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.helpTimer.Stop();
                    this.helpTimer.Tick -= HelpTimerCallBack;
                });
        }

        private void HelpTimerCallBack(object sender, EventArgs e)
        {
            StopAndCleanHelpTimer();
            if (this.SelectedMedia == null)
                return;

            this.StopAndCleanHelpTimer();

        }



        public void CurrentMediaTitleFound()
        {
            if (this.SelectedMedia == null || this.Pack == null)
                return;

            this.SelectedMedia.IsCompleted = true;
            this.GameProvider.SetIsMediaCompleted(this.SelectedMedia, pack, true);

            if (this.Pack.IsTerminated)
            {
                this.GameProvider.PackTerminated(pack);
                GoToNextMedia(this);
            }
            else
            {
                TaskHelpers.RunDelayed(300, this.NotifyGoodAnswer);
            }
        }

        public void ProposerTitre(String titre)
        {
            Pack pack = this.Pack;

#if DEBUG

            // WE COMPLETE ALL THE PACK EXCEPT THE CURRENT MOVIE QUICKLY, HACK
            if (titre.Equals("BEN2"))
            {
                this.CompleteAllPack();
            }
#endif

            // SHEAT CODES - WARNING, the "0" is a zero
            if (titre.Equals("BEN0IT"))
            {
                titre = this.SelectedMedia.Title;
            }
            if (titre.Equals("BEN0IT2"))
            {
                this.CompleteAllPack();
                titre = this.SelectedMedia.Title;
            }

            int photoIndex = pack.Medias.IndexOf(this.SelectedMedia);

            double justesse = pack.TestTitle(photoIndex, titre);
            if (justesse > 0.909) // not 0.92 because "rien a declarer" cause problems and mobius
            {
                this.StopAndCleanHelpTimer();
                this.SelectedMedia.IsCompleted = true;
                this.GameProvider.SetIsMediaCompleted(this.SelectedMedia, pack, true);

                Debug.WriteLine("Good Answer");

                if (pack.IsTerminated)
                {
                    this.GameProvider.PackTerminated(pack);
                    GoToNextMedia(this);
                }
                else
                {
                    TaskHelpers.RunDelayed(300, this.NotifyGoodAnswer);
                    //TaskEx.Delay(500, new CancellationToken()).ContinueWith(_ => this.NotifyGoodAnswer());
                }
            }
            else if (justesse > 0.80)
            {
                //this.NotifyClosedAnswer();
                TaskHelpers.RunDelayed(300, this.NotifyClosedAnswer);
                //new System.Threading.Timer(obj => { this.NotifyClosedAnswer(); }, null, 500, System.Threading.Timeout.Infinite);
            }
            else
            {
                if ((titre.Length >= 5) && (pack.TestIncompleteTitle(photoIndex, titre) > 0.92))
                {
                    // On a donné le début ou le fin du titre, on dit alors "presque"
                    Debug.WriteLine("Closed Answer");
                    TaskHelpers.RunDelayed(300, this.NotifyClosedAnswer);
                    //TaskEx.Delay(500, new CancellationToken()).ContinueWith(_ => this.NotifyClosedAnswer());

                }
                else
                {
                    // Mauvaise réponse      
                    Debug.WriteLine("Bad Answer");
                    //this.NotifyBadAnswer();
                    TaskHelpers.RunDelayed(300, this.NotifyBadAnswer);
                    //new System.Threading.Timer(obj => { this.NotifyBadAnswer(); }, null, 500, System.Threading.Timeout.Infinite);
                }
            }
        }

        private void CompleteAllPack()
        {
            if (this.Pack == null)
                return;

            Media lastMedia;

            foreach (var media in this.Pack.Medias)
            {
                if (media == this.SelectedMedia)
                    lastMedia = media;
                else
                {
                    media.IsCompleted = true;
                    this.GameProvider.SetIsMediaCompleted(media, this.Pack, true);
                    media.Time += TimeSpan.FromSeconds(20);
                }
            }
        }

        private DelegateCommand<object> goToNextMediaCommand;
        public DelegateCommand<object> GoToNextMediaCommand
        {
            get { return goToNextMediaCommand ?? (goToNextMediaCommand = new DelegateCommand<object>(GoToNextMedia)); }
            private set { goToNextMediaCommand = value; }
        }

        private void GoToNextMedia(object o)
        {
            if (this.Pack == null)
                return;

            // if package is terminated, we go to package finish page
            if (this.Pack.IsTerminated)
            {
                this.NotifyPackTerminated();
            }
            else
            {

                // We iterate from current movie to next not completed
                // if all are completed we go back to the first

                int currentIndex = this.Pack.Medias.IndexOf(this.SelectedMedia);
                int? nextIndex = null;
                for (int i = currentIndex + 1; (i < this.Pack.Medias.Count) && nextIndex == null; i++)
                {
                    if (this.Pack.Medias[i].IsCompleted == false)
                        nextIndex = i;
                }

                if (nextIndex == null)
                {
                    for (int i = 0; (i < currentIndex) && nextIndex == null; i++)
                    {
                        if (this.Pack.Medias[i].IsCompleted == false)
                            nextIndex = i;
                    }
                }

                if (nextIndex != null)
                {
                    this.SelectedMedia = this.Pack.Medias[nextIndex.Value];
                }
                else
                {
                    this.SelectedMedia = this.Pack.Medias[0];
                }
            }

        }

        public override void CleanViewModel()
        {
            this.StopAndCleanHelpTimer();

            //Debug.WriteLine("ApplicationCurrentMemoryUsage : {0}", DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage"));
            
            // Clean all ressources, we force unload bitmap
            if (this.Pack != null && this.Pack.Medias != null)
            {
                foreach (var item in this.Pack.Medias)
                    if (item.Poster != null)
                        item.Poster.UnloadBitmap();
            }

            //Debug.WriteLine("ApplicationCurrentMemoryUsage : {0}", DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage"));
        }

        


        public void NavigateToPacksListPage(bool removePreviousPageFromHistory)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add(PacksListPage.PAGE_PARAM_LEVEL_ID_INT, (int)this.Pack.LevelId);
            parameters.Add(QANavigationHelper.PAGE_PARAM_REMOVE_PREVIOUS_PAGE_FROM_HISTORY, (bool)removePreviousPageFromHistory);
            QANavigationHelper.Navigate(PacksListPage.PAGE_NAME, parameters);
        }

        #endregion

        #region Completion Callbacks

        #endregion

        #region Helpers

        private void NotifyBadAnswer()
        {
            // Notify view of an error
            Notify(BadAnswer, new NotificationEventArgs());
        }

        private void NotifyClosedAnswer()
        {
            // Notify view of an error
            Notify(ClosedAnswer, new NotificationEventArgs());
        }

        private void NotifyGoodAnswer()
        {
            // Notify view of an error
            Notify(GoodAnswer, new NotificationEventArgs());
        }

        private void NotifyPackTerminated()
        {
            // Notify view of an error
            Notify(PackTerminated, new NotificationEventArgs());
        }

        #endregion
    }
}