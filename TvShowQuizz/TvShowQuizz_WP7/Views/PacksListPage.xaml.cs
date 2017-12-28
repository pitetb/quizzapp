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
using QuizzApp.WP.ViewModels;
using QuizzApp.Core.Entities;
using Microsoft.Phone.Info;



namespace TvShowQuizz
{
    public partial class PacksListPage : PhoneApplicationPage
    {
       
        // Constructeur
        public PacksListPage()
        {
            InitializeComponent();
            this.PrepareApplicationBar();
            this.CurrentViewModel.LongLoadingStartEvent += CurrentViewModel_LongLoadingStartEvent;
            this.CurrentViewModel.LongLoadingStopEvent += CurrentViewModel_LongLoadingStopEvent;
        }

        

        public PacksListViewModel CurrentViewModel {
            get
            {
                if (this.DataContext is PacksListViewModel)
                    return (PacksListViewModel)this.DataContext;
                else
                    return null;
            }        
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            EasyTracker.GetTracker().SendView("PacksList View");


            // retrieve parameters
            IDictionary<string, object> pageValues;
            if (NavigationHelper.PageValues.TryGetValue(this.GetType().Name, out pageValues))
            {
                int levelId = 0;
                object propertyValue;
                if (pageValues.TryGetValue(QuizzApp.WP.ViewModels.Helpers.PacksListPage.PAGE_PARAM_LEVEL_ID_INT, out propertyValue))
                {
                    int.TryParse(propertyValue.ToString(), out levelId);                        
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
                            pageValues.Remove(NavigationHelper.PAGE_PARAM_REMOVE_PREVIOUS_PAGE_FROM_HISTORY);
                        }
                    }
                }

                this.CurrentViewModel.CurrentLevelId = levelId;
            }

            // Line to call event on view model
            PersistenceManager.Instance.PageNavigated(this.CurrentViewModel);
            Debug.WriteLine("ApplicationCurrentMemoryUsage : {0}", DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage"));
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

        private void MQPackagePresenterControl_ButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Pack clicked");
            

            if (sender == null || this.CurrentViewModel == null)
                return;
            MQPackagePresenterControl pckPres = sender as MQPackagePresenterControl;
            EasyTracker.GetTracker().SendEvent("ui_action", "play_pack", pckPres.Package.Title, 0);

            this.CurrentViewModel.NavigateToPackagePage(pckPres.Package);
        }

        private void ReplayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null || this.CurrentViewModel == null)
                return;
            MenuItem menuItem = sender as MenuItem;

            Pack pack = menuItem.DataContext as Pack;
            if (pack == null)
                return;

            EasyTracker.GetTracker().SendEvent("ui_action", "reinit_pack", pack.Title, 0);
            this.CurrentViewModel.ReinitPack(pack);
        }


        private void FinishMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null || this.CurrentViewModel == null)
                return;
            MenuItem menuItem = sender as MenuItem;

            Pack pack = menuItem.DataContext as Pack;
            if (pack == null)
                return;

            this.CurrentViewModel.FinishPack(pack);
        }

    }


    
}