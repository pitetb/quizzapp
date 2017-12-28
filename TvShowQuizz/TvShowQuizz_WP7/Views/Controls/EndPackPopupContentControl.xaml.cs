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
    public partial class EndPackPopupContentControl : UserControl
    {
        public EndPackPopupContentControl()
        {
            InitializeComponent();            
        }

        public void SetNbPointsOnMessage(int nbPoints)
        {
            this.MessageTextblock.Text = AppResources.PopupEndPackMessage.Replace("#nbpoints", nbPoints.ToString());
        }

    }
}
