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



namespace MovieQuizz2
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        // Constructeur
        public SettingsPage()
        {
            InitializeComponent();
            this.PrepareApplicationBar();
            this.CurrentViewModel.LongLoadingStartEvent += CurrentViewModel_LongLoadingStartEvent;
            this.CurrentViewModel.LongLoadingStopEvent += CurrentViewModel_LongLoadingStopEvent;
        }

        

        public SettingsViewModel CurrentViewModel {
            get
            {
                if (this.DataContext is SettingsViewModel)
                    return (SettingsViewModel)this.DataContext;
                else
                    return null;
            }        
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            EasyTracker.GetTracker().SendView("Settings View");

            IDictionary<string, object> pageValues;
            if (QANavigationHelper.PageValues.TryGetValue(this.GetType().Name, out pageValues))
            {
                object removePreviousPageFromHist;
                if (pageValues.TryGetValue(QANavigationHelper.PAGE_PARAM_REMOVE_PREVIOUS_PAGE_FROM_HISTORY, out removePreviousPageFromHist))
                {
                    bool? removePreviousPage = removePreviousPageFromHist as bool?;
                    if (removePreviousPage.HasValue)
                    {
                        if (removePreviousPage == true)
                        {
                            QANavigationHelper.RemoveBackEntry();
                        }
                    }
                }
            }

            // Line to call event on view model
            PersistenceManager.Instance.PageNavigated(this.CurrentViewModel);

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
            
        }

        Brush originalBackground;
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            originalBackground = (sender as Border).Background;
            (sender as Border).Background = new SolidColorBrush(Colors.LightGray);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = originalBackground;
        }

       

        private void BorderChangeLanguage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Border).Background = originalBackground;

            
            EasyTracker.GetTracker().SendEvent("ui_action", "settings", "language", 0);

            // We show the "about" app info
            MessagePrompt langPrompt = MessagePromptHelper.GetNewMessagePromptWithNoTitleAndWhiteStyle(); 
            var body = new ChooseLanguagePopupContentControl();
            body.LinkedPage = this;
            body.LinkedPrompt = langPrompt;

            //  Prompt if sure of reinit            
            langPrompt.ActionPopUpButtons.Clear();
            langPrompt.Body = body;
            langPrompt.Show();             
  
        }

        private void BorderSoundOnToOff_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Border).Background = originalBackground;
            AppSettings.Instance.SoundOnOffSetting = false;
            EasyTracker.GetTracker().SendEvent("ui_action", "settings", "sound", 0);
        }

        private void BorderSoundOffToOn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Border).Background = originalBackground;
            AppSettings.Instance.SoundOnOffSetting = true;
            EasyTracker.GetTracker().SendEvent("ui_action", "settings", "sound", 1);
        }



        private void BorderProposePack_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Border).Background = originalBackground;

            if (this.CurrentViewModel == null)
                return;

            EasyTracker.GetTracker().SendEvent("ui_action", "propose_pack", null, 0); 
            this.CurrentViewModel.NavigateToProposePack();            
        }

        private void BorderQuizzApp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Border).Background = originalBackground;

            if (this.CurrentViewModel == null)
                return;

            EasyTracker.GetTracker().SendEvent("ui_action", "settings", "sound", 1); 
            this.CurrentViewModel.NavigateToQuizzAppWebSite();            
        }


        private void BorderHelp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Border).Background = originalBackground;

            if (this.CurrentViewModel == null)
                return;

            EasyTracker.GetTracker().SendEvent("ui_action", "settings", "help", 0);
            this.CurrentViewModel.NavigateToHelpPage();
        }

        private void BorderContactAuthor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Border).Background = originalBackground;

            if (this.CurrentViewModel == null)
                return;

            EasyTracker.GetTracker().SendEvent("ui_action", "settings", "contact_author", 0);

            // Send email
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "ben.pit@gmail.com";
            emailComposeTask.Show();
        }


        private void BorderMovieQuizzLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Border).Background = originalBackground;

            if (this.CurrentViewModel == null)
                return;

            EasyTracker.GetTracker().SendEvent("ui_action", "settings", "link_to_tv_show_quizz", 0);

            // Go TO market link
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri("http://www.windowsphone.com/s?appid=6330eee1-50a9-46f5-80bc-1f42a502ffe5");
            wbt.Show();
        }
    }


    
}