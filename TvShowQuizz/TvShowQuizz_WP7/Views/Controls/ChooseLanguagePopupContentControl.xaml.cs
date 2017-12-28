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
    public partial class ChooseLanguagePopupContentControl : UserControl
    {
        public ChooseLanguagePopupContentControl()
        {
            InitializeComponent();
            Debug.WriteLine(LanguagesUtils.GetCurrentLanguageOnQuizzAppFormat());
        }

        public Page LinkedPage;
        public MessagePrompt LinkedPrompt;

        private void Button_French_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(LanguagesUtils.GetCurrentLanguageOnQuizzAppFormat());
            if (LanguagesUtils.GetCurrentLanguageOnQuizzAppFormat().Equals(Constants.EN_IDENTIFIER, StringComparison.InvariantCultureIgnoreCase))
            {
                LanguagesUtils.SetCurrentLanguage(Constants.FR_CU);
                DisplayRestartMessage();
            }
            else if (LinkedPrompt != null)
            {
                // Close
                LinkedPrompt.Hide();
            }
        }

        private void Button_English_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(LanguagesUtils.GetCurrentLanguageOnQuizzAppFormat());
            if ( LanguagesUtils.GetCurrentLanguageOnQuizzAppFormat().Equals(Constants.FR_IDENTIFIER, StringComparison.InvariantCultureIgnoreCase))
            {                
                LanguagesUtils.SetCurrentLanguage(Constants.EN_CU);
                DisplayRestartMessage();
            }
            else if (LinkedPrompt != null)
            {
                // Close
                LinkedPrompt.Hide();
            }
        }

        private void DisplayRestartMessage()
        {
            MessagePrompt prompt = MessagePromptHelper.GetNewMessagePromptWithNoTitleAndWhiteStyle();
            prompt.Message = AppResources.RestartApplicationMessageText;
            prompt.Completed +=prompt_Completed;
            prompt.Show();
        }

        private void prompt_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (LinkedPage == null)
                return;

            
            throw new QuitException();            
        }


    }
}
