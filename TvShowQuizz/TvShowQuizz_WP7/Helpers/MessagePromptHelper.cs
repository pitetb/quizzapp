using Coding4Fun.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace TvShowQuizz.Helpers
{
    public class MessagePromptHelper
    {
        public static MessagePrompt GetNewMessagePromptWithNoTitle()
        {
            MessagePrompt promt = new MessagePrompt();
            var style = App.Current.Resources["MQMessagePromptNoTitleStyle"];
            if (style != null)
                promt.Style = (Style)style;

            
            return promt;
        }


        public static MessagePrompt GetNewMessagePromptWithNoTitleAndWhiteStyle()
        {
            MessagePrompt promt = new MessagePrompt();
            var style = App.Current.Resources["MQMessagePromptNoTitleStyle"];
            if (style != null)
                promt.Style = (Style)style;
            promt.Background = new SolidColorBrush(Colors.White);

            return promt;
        }
    }
}
