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
using System.IO.IsolatedStorage;
using System.Windows.Threading;
using System.Threading.Tasks;
using QuizzApp.Core.Helpers;
using QuizzApp.Core.Controllers;
using System.Resources;
using System.Reflection;
using System.Globalization;
using QuizzApp.WP.ViewModels.Helpers.MigrationManager;

namespace QuizzApp.WP.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class PopupSplashViewModel : QAViewModelBase<PopupSplashViewModel>
    {
        #region Initialization and Cleanup


        // Default ctor
        public PopupSplashViewModel(GameProvider gameProvider)
            : base(gameProvider)
        {
            if (this.IsInDesignMode)
            {
               
            }
            
            

            this.ProgressIndication = "Initializing Data...";
        }

        #endregion

        #region Notifications

        // Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> LoadFromAppRessourcesTerminated;
        public event EventHandler<NotificationEventArgs> NoNetworkAlert;

        public event EventHandler<NotificationEventArgs> StartupInitializingMoviesStep1;
        public event EventHandler<NotificationEventArgs> StartupInitializingMoviesStep2;

        #endregion

        #region Properties

        private string progressIndication;
        public string ProgressIndication
        {
            get { return progressIndication; }
            set
            {
                progressIndication = value;
                NotifyPropertyChanged(m => m.ProgressIndication);
            }
        }

        #endregion

        #region Methods

        public void LoadPackagesFromAppResources(ResourceManager rm)
        {

            Task.Factory.StartNew(async () =>
            {
                Debug.WriteLine("SplachScreen start");

                
                
                // This deletes all existing files in IsolatedStorage - Useful in testing
                //this.ProgressIndication = Resources.AppResources.StartupInitializingMoviesStep1;
                // In live should not do this, but only load files once - this speeds subsequent loading of the app
                //using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                //{
                //    isoStore.Remove();
                //}

                //ResourceManager rm = new ResourceManager("MovieQuizz2.Resources.AppResources", Assembly.GetExecutingAssembly());
                if (AppSettings.Instance.IsQuizzAppFirstAppLaunch)
                {
#if DEBUG
                    // ADD TEST DATA
                    //this.ProgressIndication = Resources.AppResources.StartupInitializingMoviesStep1;
                    //IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForApplication();
                    //storageFile.CreateDirectory("FakeData\\Images");
                    //IsolatedStorageHelpers.CopyBinaryFile(storageFile, "FakeData\\Images\\avatar.jpg", true);
                    //IsolatedStorageHelpers.CopyBinaryFile(storageFile, "FakeData\\Images\\fightclub.jpg", true);
                    //IsolatedStorageHelpers.CopyBinaryFile(storageFile, "FakeData\\Images\\themask.jpg", true);

                    //storageFile.CreateDirectory("FakeData\\Package\\pack_1");
                    //IsolatedStorageHelpers.CopyBinaryFile(storageFile, "FakeData\\Package\\pack_1\\pack_1.sqlite", true);
#endif
                    
                    // Load packs in app
                    //this.ProgressIndication = Resources.AppResources.StartupInitializingMoviesStep1;
                    this.NotifyStartupInitializingMoviesStep1();

                    this.GameProvider.InstallMainDb(rm);                    
                }
                                

                // Make "init"
                //this.ProgressIndication = Resources.AppResources.StartupInitializingMoviesStep2;
                this.NotifyStartupInitializingMoviesStep2();
                bool initOk = await this.GameProvider.GetInitInfosAsync(false);

                if (!initOk)
                    this.NotifyNoNetworkAlert();
                

                // Install 'in App' levels 
                if (AppSettings.Instance.IsQuizzAppFirstAppLaunch)
                {
                   this.GameProvider.DeployPackagesInApp(rm);
                   AppSettings.Instance.IsQuizzAppFirstAppLaunch = false;
                }
                
                // Import Movie Quizz 1 packs
                new MigrationManager().MigrateMovieQuizz1Packs();


                Thread.Sleep(150);
                NotifyLoadTerminated(string.Empty, null);
            });   
        }
                

        

        #endregion

        #region Completion Callbacks

        #endregion

        #region Helpers
                
        private void NotifyLoadTerminated(string message, Exception error)
        {
            // Notify view of an error
            Notify(LoadFromAppRessourcesTerminated, new NotificationEventArgs<Exception>(message, error));
        }


        private void NotifyNoNetworkAlert()
        {
            // Notify view of an error
            Notify(NoNetworkAlert, new NotificationEventArgs());
        }

        private void NotifyStartupInitializingMoviesStep1()
        {
            // Notify view of an error
            Notify(StartupInitializingMoviesStep1, new NotificationEventArgs());
        }

        private void NotifyStartupInitializingMoviesStep2()
        {
            // Notify view of an error
            Notify(StartupInitializingMoviesStep2, new NotificationEventArgs());
        }

        #endregion
        
    }
}