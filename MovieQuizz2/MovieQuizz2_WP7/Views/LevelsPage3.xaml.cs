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
using MovieQuizz2.Views;
using System.Threading;
using System.IO.IsolatedStorage;
using MovieQuizz2.Helpers;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using MovieQuizz2.Resources;
using Microsoft.Phone.Tasks;
using System.Windows.Navigation;
using QuizzApp.Tombstone;
using Coding4Fun.Toolkit.Controls;
using GoogleAnalytics;
using System.Threading.Tasks;
using QuizzApp.Core.Helpers;
using System.Globalization;
using System.Windows.Threading;
using QuizzApp.WP.ViewModels;
using QuizzApp.Core.Entities;



namespace MovieQuizz2
{
    public partial class LevelsPage3 : PhoneApplicationPage
    {

        // Constructeur
        public LevelsPage3()
        {
            InitializeComponent();
            this.PrepareApplicationBar();
            this.CurrentViewModel.LongLoadingStartEvent += CurrentViewModel_LongLoadingStartEvent;
            this.CurrentViewModel.LongLoadingStopEvent += CurrentViewModel_LongLoadingStopEvent;

            this.CurrentViewModel.InstallLevelStartEvent += CurrentViewModel_InstallLevelStartEvent;
            this.CurrentViewModel.InstallLevelTerminatedEvent += CurrentViewModel_InstallTerminatedEvent;
            this.CurrentViewModel.InstallFilesProgress += CurrentViewModel_InstallFilesProgress;
            this.CurrentViewModel.DownloadProgressEvent += CurrentViewModel_DownloadProgressEvent;
        }






        public LevelsList2ViewModel CurrentViewModel
        {
            get
            {
                if (this.DataContext is LevelsList2ViewModel)
                    return (LevelsList2ViewModel)this.DataContext;
                else
                    return null;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // retrieve parameters
            IDictionary<string, object> pageValues;
            if (NavigationHelper.PageValues.TryGetValue(this.GetType().Name, out pageValues))
            {
                object removePreviousPageFromHist;
                if (pageValues.TryGetValue(NavigationHelper.PAGE_PARAM_REMOVE_PREVIOUS_PAGE_FROM_HISTORY, out removePreviousPageFromHist))
                {
                    bool? removePreviousPage = removePreviousPageFromHist as bool?;
                    if (removePreviousPage.HasValue)
                    {
                        if (removePreviousPage == true)
                        {
                            NavigationHelper.RemoveBackEntry();
                            pageValues.Remove(NavigationHelper.PAGE_PARAM_REMOVE_PREVIOUS_PAGE_FROM_HISTORY);
                        }
                    }
                }
            }

            //if (e.NavigationMode == NavigationMode.Back)
            //    this.CurrentViewModel.LoadAllDifficultiesAndLevels();


            // Line to call event on view model
            PersistenceManager.Instance.PageNavigated(this.CurrentViewModel);

            EasyTracker.GetTracker().SendView("Levels Screen");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }


        private bool systemTrayVisibleBck;

        private void CurrentViewModel_LongLoadingStopEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {

            SystemTray.SetProgressIndicator(this, null);
            SystemTray.SetIsVisible(this, systemTrayVisibleBck);
        }

        private void CurrentViewModel_LongLoadingStartEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            systemTrayVisibleBck = SystemTray.IsVisible;
            SystemTray.Opacity = 0.3;
            SystemTray.BackgroundColor = Colors.Transparent;
            SystemTray.SetIsVisible(this, true);

            ProgressIndicator prog = new ProgressIndicator();
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = AppResources.LoadingIndicator;
            SystemTray.SetProgressIndicator(this, prog);
        }



        private void PrepareApplicationBar()
        {
            ApplicationBar.IsVisible = true;
            for (int i = 0; i < ApplicationBar.Buttons.Count; i++)
            {
                if (i == 0)
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[i]).Text = AppResources.AppBarRefreshText;

            }
        }


        private void ApplicationBarRefreshButton_Click(object sender, EventArgs e)
        {
            if (this.CurrentViewModel == null)
                return;

            EasyTracker.GetTracker().SendEvent("ui_action", "click", "refresh_button", 0);
            this.CurrentViewModel.LoadAllDifficultiesAndLevels(true);
        }

        private void ApplicationBarLevelsViewButton_Click(object sender, EventArgs e)
        {
            if (this.CurrentViewModel == null)
                return;

            //EasyTracker.GetTracker().SendEvent("ui_action", "click", "refresh_button", 0);
            this.CurrentViewModel.NavigateToLevelsPage();
        }

        


        private void MQLevelButton_ButtonClicked(object sender, EventArgs e)
        {
            MQLevelButton button = sender as MQLevelButton;
            if (button == null || this.CurrentViewModel == null)
                return;

            LevelPresenter levelPresenter = button.DataContext as LevelPresenter;
            if (levelPresenter == null)
                return;

            if (levelPresenter.IsLocal == false)
            {
                EasyTracker.GetTracker().SendEvent("ui_action", "download_level", levelPresenter.Id.ToString(), levelPresenter.Number);
                this.CurrentViewModel.DownloadLevel(levelPresenter);
            }
            else
            {
                this.CurrentViewModel.NavigateToPacksListPage(levelPresenter.Id);
            }
        }


        private void MQPackagePresenterControl_ButtonClicked(object sender, EventArgs e)
        {
            MQPackagePresenterControl button = sender as MQPackagePresenterControl;
            if (button == null || this.CurrentViewModel == null)
                return;

            Pack pack = button.DataContext as Pack;
            if (pack == null)
                return;

            if (pack.IsRemotePack)
            {
                //EasyTracker.GetTracker().SendEvent("ui_action", "download_level", levelPresenter.Id.ToString(), levelPresenter.Number);
                this.CurrentViewModel.DownloadLevel(pack.LevelId);
            }
            else
            {
                this.CurrentViewModel.NavigateToPackagePage(pack);
            }
        }


        MessagePrompt downloadPrompt;
        private void CurrentViewModel_InstallLevelStartEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            //downloadPrompt = MessagePromptHelper.GetNewMessagePromptWithNoTitle();
            //downloadPrompt.IsAppBarVisible = false;
            //downloadPrompt.IsCancelVisible = false;
            //downloadPrompt.ActionPopUpButtons.Clear();
            //downloadPrompt.Message = AppResources.PopupLevelsDownloadMessage;
            //downloadPrompt.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            //downloadPrompt.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            //downloadPrompt.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //downloadPrompt.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            //var progressBar = new ProgressBar();
            //progressBar.IsEnabled = true;
            //progressBar.Value = 0;
            //downloadPrompt.Body = progressBar;
            //downloadPrompt.Show();

            Debug.WriteLine("CurrentViewModel_InstallLevelStartEvent");

            // We show the "about" app info
            downloadPrompt = MessagePromptHelper.GetNewMessagePromptWithNoTitleAndWhiteStyle();
            downloadPrompt.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            downloadPrompt.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            var body = new DownloadPopupContentControl();
            body.ProgressBar.Visibility = System.Windows.Visibility.Visible;
            body.PercentTextblock.Visibility = System.Windows.Visibility.Visible;
            body.SetTitleText(AppResources.PopupLevelsDownloadMessage);
            //  Prompt if sure of reinit            
            downloadPrompt.ActionPopUpButtons.Clear();
            downloadPrompt.Body = body;
            downloadPrompt.Show();

        }

        private void CurrentViewModel_DownloadProgressEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<int> e)
        {
            if (downloadPrompt == null)
                return;

            Debug.WriteLine("CurrentViewModel_DownloadProgressEvent");
            DownloadPopupContentControl control = (DownloadPopupContentControl)downloadPrompt.Body;
            control.SetPercentProgress(e.Data);
            //downloadPrompt.Message = e.Data.ToString();
        }

        private void CurrentViewModel_InstallFilesProgress(object sender, SimpleMvvmToolkit.NotificationEventArgs<int> e)
        {
            if (downloadPrompt == null)
                return;

            Debug.WriteLine("CurrentViewModel_InstallFilesProgress");
            DownloadPopupContentControl control = (DownloadPopupContentControl)downloadPrompt.Body;
            control.SetTitleText(AppResources.PopupLevelsDownloadInstallationProgress);
            control.SetPercentProgress(e.Data);
            //control.PercentTextblock.Visibility = System.Windows.Visibility.Collapsed;
            //control.ProgressBar.Visibility = System.Windows.Visibility.Collapsed;
        }


        private void CurrentViewModel_InstallTerminatedEvent(object sender, SimpleMvvmToolkit.NotificationEventArgs<bool> e)
        {
            if (downloadPrompt == null)
                return;

            EasyTracker.GetTracker().SendEvent("ui_action", "download_install_level_terminated", e.Data.ToString(), 0);

            Debug.WriteLine("CurrentViewModel_InstallTerminatedEvent");
            DownloadPopupContentControl control = (DownloadPopupContentControl)downloadPrompt.Body;

            if (e.Data == true)
            {
                control.SetTitleText(AppResources.PopupLevelsDownloadTerminatedOk);
            }
            else
            {
                control.SetTitleText(AppResources.PopupLevelsDownloadTerminatedError);
            }

            Debug.WriteLine("Launch Timer");
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1.2);
            dt.Tick += (s, evt) => { downloadPrompt.Hide(); dt.Stop(); };
            dt.Start();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = ((CheckBox)sender);

            var parent = ((CheckBox)sender).Parent as Grid;
            var childrenGrid = parent.Children[1] as Grid;

            childrenGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = ((CheckBox)sender);

            var parent = ((CheckBox)sender).Parent as Grid;
            var childrenGrid = parent.Children[1] as Grid;

            childrenGrid.Visibility = System.Windows.Visibility.Collapsed;
        }



    }



}