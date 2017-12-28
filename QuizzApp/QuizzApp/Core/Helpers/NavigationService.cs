using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Microsoft.Phone.Controls;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace QuizzApp.Core.Helpers
{
    

	public static class QANavigationHelper
	{

        public static bool AvoidCircularNavigation = false;

        // const to facilitate navigation between page        
        public const string PAGE_PARAM_REMOVE_PREVIOUS_PAGE_FROM_HISTORY = "removePreviousPage";

        public static bool CanGoBack
        {
            get
            {
                PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (frame == null)
                    return false;

                return frame.CanGoBack;
            }
        }

        public static void GoBack()
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame == null)
                return;

            frame.GoBack();
        }

        public static System.Windows.Navigation.JournalEntry RemoveBackEntry()
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame == null)
                return null;

            return frame.RemoveBackEntry();
        }

		public static void Navigate(string uri)
		{
			Navigate(uri, new Dictionary<string, object>());
		}

                  
        public static void Navigate(string uri, IDictionary<string, object> parameters)
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame == null)
                return;

            string wellFormedUti = uri;
            if (!wellFormedUti.StartsWith("/"))
                wellFormedUti = "/" + uri;

            StringBuilder uriBuilder = new StringBuilder(wellFormedUti);

            if (parameters != null && parameters.Count > 0)
            {
                if (pageValues.ContainsKey(uri))
                    pageValues.Remove(uri);

                pageValues.Add(uri, parameters);
            }
            wellFormedUti = uriBuilder.ToString();
            

            // Before test
            Debug.WriteLine("----Before test-----------");
            foreach (var item in frame.BackStack)
            {
                Debug.WriteLine("Backstack : " + item.Source);
            }
            Debug.WriteLine("------End----------");

            // Clean back stack
            var backstack = frame.BackStack.ToList();
            if (backstack.Count >= 1)
            {
                // if previous equals destination, we unstack the previous
                if (backstack[0].Source.Equals(wellFormedUti))
                {
                    RemoveBackEntry();
                }                
            }

            // Before test
            Debug.WriteLine("----After test-----------");
            foreach (var item in frame.BackStack)
            {
                Debug.WriteLine("Backstack : " + item.Source);
            }
            Debug.WriteLine("------End----------");

            frame.Navigate(new Uri(wellFormedUti, UriKind.RelativeOrAbsolute));
        }


        public static IDictionary<string, IDictionary<string, object>> PageValues
        {
            get { return pageValues; }
            set { pageValues = value; }
        }

        private static IDictionary<string, IDictionary<string, object>> pageValues =
            new Dictionary<string, IDictionary<string, object>>();
	}
}
