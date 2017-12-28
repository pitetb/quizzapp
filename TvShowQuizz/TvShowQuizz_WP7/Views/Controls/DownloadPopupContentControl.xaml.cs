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
using System.Threading;
using System.Globalization;
using QuizzApp.Core.Helpers;
using Coding4Fun.Toolkit.Controls;
using System.Diagnostics;
using TvShowQuizz.Helpers;
using TvShowQuizz.Resources;

namespace TvShowQuizz
{
    public partial class DownloadPopupContentControl : UserControl
    {
        public DownloadPopupContentControl()
        {
            InitializeComponent();
        }

        public void SetPercentProgress(int value)
        {
            this.ProgressBar.Value = value;
            this.PercentTextblock.Text = "" + value + " %";
        }

        public void SetTitleText(string text)
        {
            this.TitleTextBox.Text = text;
        }
    }
}
