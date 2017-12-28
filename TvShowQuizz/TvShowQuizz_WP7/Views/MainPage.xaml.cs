using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using TvShowQuizz.Views;
using System.Threading;
using System.IO.IsolatedStorage;
using TvShowQuizz.Helpers;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using TvShowQuizz.Resources;
using Microsoft.Phone.Tasks;
using System.Windows.Navigation;
using QuizzApp.Tombstone;
using Coding4Fun.Toolkit.Controls;
using GoogleAnalytics;
using System.Threading.Tasks;
using QuizzApp.Core.Helpers;
using System.Globalization;
using QuizzApp.WP.ViewModels;
using System.Resources;
using System.Reflection;



namespace TvShowQuizz
{
    public partial class MainPage : PhoneApplicationPage
    {
        
        // Constructeur
        public MainPage()
        {
            InitializeComponent();
            //ShowPopup();
        }

        public MainPageViewModel CurrentViewModel {
            get
            {
                if (this.DataContext is MainPageViewModel)
                    return (MainPageViewModel)this.DataContext;
                else
                    return null;
            }        
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (PersistenceManager.Instance.ApplicationMode != ApplicationMode.Activated && ! popupShowed)
                ShowPopup();
            else
                EasyTracker.GetTracker().SendView("Home Screen");

            // Line to call event on view model
            PersistenceManager.Instance.PageNavigated(this.CurrentViewModel);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);            
        }

        private bool popupShowed = false;
        private void ShowPopup()
        {
            ResourceManager rm = new ResourceManager("TvShowQuizz.Resources.AppResources", Assembly.GetExecutingAssembly());

            this.LayoutRoot.Visibility = System.Windows.Visibility.Collapsed; 
            this.popuGrid.Visibility = System.Windows.Visibility.Visible;
            ((PopupSplash)this.popuGrid.Children[0]).PopupSplashViewModel.LoadFromAppRessourcesTerminated += PopupSplashViewModel_LoadFromAppRessourcesTerminated;
            ((PopupSplash)this.popuGrid.Children[0]).PopupSplashViewModel.NoNetworkAlert += PopupSplashViewModel_NoNetworkAlert;
            ((PopupSplash)this.popuGrid.Children[0]).PopupSplashViewModel.StartupInitializingMoviesStep1 += PopupSplashViewModel_StartupInitializingMoviesStep1;
            ((PopupSplash)this.popuGrid.Children[0]).PopupSplashViewModel.StartupInitializingMoviesStep2 += PopupSplashViewModel_StartupInitializingMoviesStep2;
            ((PopupSplash)this.popuGrid.Children[0]).PopupSplashViewModel.LoadPackagesFromAppResources(rm);
        }
        
        
        private void PopupSplashViewModel_StartupInitializingMoviesStep2(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                PopupSplashViewModel vm = (PopupSplashViewModel)sender;
                vm.ProgressIndication = AppResources.StartupInitializingMoviesStep1;
            }
            );
        }

        private void PopupSplashViewModel_StartupInitializingMoviesStep1(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                PopupSplashViewModel vm = (PopupSplashViewModel)sender;
                vm.ProgressIndication = AppResources.StartupInitializingMoviesStep2;
            }
            );
        }

        

        private void PopupSplashViewModel_NoNetworkAlert(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            MessagePrompt helpPrompt = MessagePromptHelper.GetNewMessagePromptWithNoTitle();
            helpPrompt.IsAppBarVisible = true;
            helpPrompt.IsCancelVisible = false;
            helpPrompt.Message = AppResources.InitNoNetworkMessage;

            Button button = new Button();
            button.Content = "Ok";
            helpPrompt.Show();
        }


        private void PopupSplashViewModel_LoadFromAppRessourcesTerminated(object sender, SimpleMvvmToolkit.NotificationEventArgs<Exception> e)
        {   
            this.Dispatcher.BeginInvoke(() =>
                {                
                    this.LayoutRoot.Visibility = System.Windows.Visibility.Visible;
                    ((PopupSplash)this.popuGrid.Children[0]).UnloadProgressBar(); 
                    this.popuGrid.Visibility = System.Windows.Visibility.Collapsed;
                    SystemTray.IsVisible = false;
                    popupShowed = true;
                    EasyTracker.GetTracker().SendView("Home Screen");
                }
            );
        }

        

        private void buttonPlay_ImageClick(object sender, RoutedEventArgs e)
        {
            if (this.CurrentViewModel == null)
                return;

            this.CurrentViewModel.NavigateToLevelsPage();
        }

        

       
        private void ButtonRateApp_ImageClick(object sender, RoutedEventArgs e)
        {
            EasyTracker.GetTracker().SendEvent("ui_action", "evaluate", "home", 0);
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        
        private void ButtonScores_Click(object sender, RoutedEventArgs e)
        {
            EasyTracker.GetTracker().SendEvent("ui_action", "get_scores", "home", 0);

            if (this.CurrentViewModel == null)
                return;

            int totalPoints = this.CurrentViewModel.GetTotalPoints();

            var text = AppResources.ScoresPopupText.Replace("#totalPoints", (totalPoints).ToString());
            MessageBox.Show(text , AppResources.ScoresPopupTitle, MessageBoxButton.OK);
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            EasyTracker.GetTracker().SendEvent("ui_action", "settings", "home", 0);

            if (this.CurrentViewModel == null)
                return;

            this.CurrentViewModel.NavigateToSettingsPage();
        }

        
    }


    
}