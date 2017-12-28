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
using System.Windows.Threading;
using TvShowQuizz.Views.Controls.MQKeyboard;
using QuizzApp.Core.Entities;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using QuizzApp.WP.ViewModels;



namespace TvShowQuizz
{
    public partial class HelpPage : PhoneApplicationPage
    {
       
        // Constructeur
        public HelpPage()
        {
            InitializeComponent();
            this.PrepareApplicationBar();
           
            
        }

       
        public HelpViewModel CurrentViewModel {
            get
            {
                if (this.DataContext is HelpViewModel)
                    return (HelpViewModel)this.DataContext;
                else
                    return null;
            }        
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            IDictionary<string, object> pageValues;
            if (NavigationHelper.PageValues.TryGetValue(this.GetType().Name, out pageValues))
            {
                object propertyValue;
                if (pageValues.TryGetValue(QuizzApp.WP.ViewModels.Helpers.HelpPage.PAGE_PARAM_NEXT_PAGE_NAME, out propertyValue))
                {
                    this.CurrentViewModel.NextPageName = propertyValue.ToString();

                    if (pageValues.TryGetValue(QuizzApp.WP.ViewModels.Helpers.HelpPage.PAGE_PARAM_NEXT_PAGE_PARAMS, out propertyValue))
                    {
                        this.CurrentViewModel.NextPageParams = propertyValue as Dictionary<string, object>;
                    }
                }


                object removePreviousPageFromHist;
                if (pageValues.TryGetValue(NavigationHelper.PAGE_PARAM_REMOVE_PREVIOUS_PAGE_FROM_HISTORY, out removePreviousPageFromHist))
                {
                    bool? removePreviousPage = removePreviousPageFromHist as bool?;
                    if (removePreviousPage.HasValue)
                    {
                        if (removePreviousPage == true)
                        {
                            NavigationHelper.RemoveBackEntry();
                        }
                    }
                }
            }          

            // Line to call event on view model
            PersistenceManager.Instance.PageNavigated(this.CurrentViewModel);

            EasyTracker.GetTracker().SendView("Help View");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            
        }

        private void PhoneApplicationPage_Unloaded(object sender, RoutedEventArgs e)
        {
           
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
            for (int i = 0; i < ApplicationBar.Buttons.Count; i++)
            {
                if (i == 0)
                    ((ApplicationBarIconButton)ApplicationBar.Buttons[i]).Text = AppResources.AppBarCloseButton;
            }
        }

        
        

        private void imageMovie_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.CurrentViewModel == null)
                return;
            if (this.CurrentViewModel.SelectedPageIndex == (this.PivotControl.Items.Count - 1))
                this.CurrentViewModel.SelectedPageIndex = 0;
            else
                this.CurrentViewModel.SelectedPageIndex = this.CurrentViewModel.SelectedPageIndex + 1;
        }



        private void ApplicationBarCloseButton_Click(object sender, EventArgs e)
        {
            if (this.CurrentViewModel == null)
                return;

            this.CurrentViewModel.GoToPreviousPage();
        }



        

    }


    
}