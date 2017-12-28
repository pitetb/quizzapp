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
using MovieQuizz2.Views.Controls.MQKeyboard;
using QuizzApp.Core.Entities;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using QuizzApp.WP.ViewModels;



namespace MovieQuizz2
{
    public partial class GamePage : PhoneApplicationPage
    {
        
        // Constructeur
        public GamePage()
        {
            InitializeComponent();
            this.PrepareApplicationBar();
           
            if (this.CurrentViewModel != null)
            {
                this.CurrentViewModel.LongLoadingStartEvent += CurrentViewModel_LongLoadingStartEvent;
                this.CurrentViewModel.LongLoadingStopEvent += CurrentViewModel_LongLoadingStopEvent;
                this.CurrentViewModel.ClosedAnswer += CurrentViewModel_ClosedAnswer;
                this.CurrentViewModel.GoodAnswer += CurrentViewModel_GoodAnswer;
                this.CurrentViewModel.BadAnswer += CurrentViewModel_BadAnswer;
                this.CurrentViewModel.PackTerminated += CurrentViewModel_PackTerminated;
            }
        }

        
               

        public GamePageViewModel CurrentViewModel {
            get
            {
                if (this.LayoutRoot.DataContext is GamePageViewModel)
                    return (GamePageViewModel)this.LayoutRoot.DataContext;
                else
                    return null;
            }        
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            EasyTracker.GetTracker().SendView("Game View");


            // retrieve parameters if any, and set VM value
            IDictionary<string, object> pageValues;
            if (QANavigationHelper.PageValues.TryGetValue(this.GetType().Name, out pageValues))
            {
                object propertyValue;
                if (pageValues.TryGetValue(QuizzApp.WP.ViewModels.Helpers.GamePage.PAGE_PARAM_PACKID, out propertyValue))
                {
                    int packId;
                    if (int.TryParse(propertyValue.ToString(), out packId))
                        this.CurrentViewModel.CurrentPackId = packId;
                }


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
            
            if (this.CurrentViewModel != null)
                this.CurrentViewModel.CleanViewModel();        
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.CurrentViewModel != null)
                this.CurrentViewModel.CleanViewModel();
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

        
        private void CurrentViewModel_BadAnswer(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            EasyTracker.GetTracker().SendEvent("game_event", "bad_answer", this.CurrentViewModel.SelectedMedia.Title, this.CurrentViewModel.SelectedMedia.Id);
        }

        private void CurrentViewModel_GoodAnswer(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            EasyTracker.GetTracker().SendEvent("game_event", "good_answer", this.CurrentViewModel.SelectedMedia.Title, this.CurrentViewModel.SelectedMedia.Id);
        }

        private void CurrentViewModel_ClosedAnswer(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            EasyTracker.GetTracker().SendEvent("game_event", "closed_answer", this.CurrentViewModel.SelectedMedia.Title, this.CurrentViewModel.SelectedMedia.Id);
        }


        MessagePrompt messagePackTerminatedPrompt;
        private void CurrentViewModel_PackTerminated(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            EasyTracker.GetTracker().SendEvent("game_event", "pack_terminated", this.CurrentViewModel.Pack.Title, this.CurrentViewModel.Pack.Id);
            
            // We show the "packterminated popup" app info
            messagePackTerminatedPrompt = MessagePromptHelper.GetNewMessagePromptWithNoTitleAndWhiteStyle();
            messagePackTerminatedPrompt.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            messagePackTerminatedPrompt.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            var body = new EndPackPopupContentControl();
            body.SetNbPointsOnMessage(this.CurrentViewModel.Pack.PossiblePoints);
            Button ok = body.OkButton;
            Button packs = body.PacksButton;
            ok.Click += ((bt, ev) => { messagePackTerminatedPrompt.Hide(); });
            packs.Click += ((bt, ev) => { this.CurrentViewModel.NavigateToPacksListPage(true); messagePackTerminatedPrompt.Hide(); });
            //  Prompt if sure of reinit            
            messagePackTerminatedPrompt.ActionPopUpButtons.Clear();
            messagePackTerminatedPrompt.Body = body;
            messagePackTerminatedPrompt.Show();    
        }


        private void KeyboardControl_IsTitleFoundChanged(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            Debug.WriteLine("Keyboard event : IsTitleFoundChanged");
            KeyboardControl keyboardControl = sender as KeyboardControl;
            if (keyboardControl == null || this.CurrentViewModel == null || this.CurrentViewModel.SelectedMedia == null)
                return;

            if (keyboardControl.IsFound)
            {
                this.CurrentViewModel.CurrentMediaTitleFound();


                if (AppSettings.Instance.SoundOnOffSetting)
                {
                    // Load the SoundEffect
                    var info = App.GetResourceStream(new Uri("Resources/Sounds/success.wav", UriKind.Relative));
                    SoundEffect effect = SoundEffect.FromStream(info.Stream);
                    // Tell the XNA Libraries to continue to run
                    FrameworkDispatcher.Update();
                    // Play the Sound
                    effect.Play();
                }
            }

            //this.CurrentViewModel.SelectedMedia.IsCompleted = keyboardControl.IsFound;
            //this.CurrentViewModel.ProposerTitre();
        }

        private void imageMovie_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var keyboard = VisualTreeHelpers.FindName<KeyboardControl>("keyboardControl", (sender as Image).Parent);
            if (keyboard == null)
                return;

            keyboard.SwitchKeyboardVisibility();
        }

        private void keyboardControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.CurrentViewModel == null)
                return;
            
            KeyboardControl keyboard = sender as KeyboardControl;
            if (keyboard == null)
                return;

            Media dataContext = keyboard.DataContext as Media;
            if (dataContext == null)
                return;

            keyboard.InitializeState(dataContext.Title, dataContext.IsCompleted, Constants.QUIZZ_APP_HELP_LIMIT);
        }

        private void keyboardControl_HelpNotAvailableYet(object sender, SimpleMvvmToolkit.NotificationEventArgs<DateTime> e)
        {
            EasyTracker.GetTracker().SendEvent("game_event", "help_notavailable", this.CurrentViewModel.SelectedMedia.Title, this.CurrentViewModel.SelectedMedia.Id);

            TimeSpan nbSeconds = e.Data - DateTime.Now;
            var text = AppResources.GamePageHelpPopupText.Replace("#time", ((int)nbSeconds.TotalSeconds).ToString());
            MessageBox.Show(text, AppResources.GamePageHelpPopupTitle, MessageBoxButton.OK);
        }


        private void Viewbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void keyboardControl_BadWord(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            if (AppSettings.Instance.SoundOnOffSetting == false)
                return;

            // Load the SoundEffect
            var info = App.GetResourceStream(new Uri("Resources/Sounds/wrong.wav", UriKind.Relative));
            SoundEffect effect = SoundEffect.FromStream(info.Stream);
             // Tell the XNA Libraries to continue to run
            FrameworkDispatcher.Update();
            // Play the Sound
            effect.Play();          
        }

       
    }


    
}