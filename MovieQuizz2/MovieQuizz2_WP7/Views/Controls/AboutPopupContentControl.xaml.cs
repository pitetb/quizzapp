using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Reflection;
using GoogleAnalytics;
using Microsoft.Phone.Tasks;

namespace MovieQuizz2
{
    public partial class AboutPopupContentControl : UserControl
    {
        public AboutPopupContentControl()
        {
            InitializeComponent();
            this.versionTextblock.Text = GetVersionNumber();
        }

        private static string GetVersionNumber()
        {
            var asm = Assembly.GetExecutingAssembly();
            var parts = asm.FullName.Split(',');
            return parts[1].Split('=')[1];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EasyTracker.GetTracker().SendEvent("ui_action", "contact_author", "about_screen", 0);

            // Send email
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "ben.pit@gmail.com";

            emailComposeTask.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EasyTracker.GetTracker().SendEvent("ui_action", "evaluate", "about_screen", 0);

            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
    }
}
