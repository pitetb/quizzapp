using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace QuizzApp.Core.Helpers
{
    public static class LanguagesUtils
    {

        public static string GetCurrentLanguageOnQuizzAppFormat()
        {
            CultureInfo cu = AppSettings.Instance.AppCurrentLanguage;
            return cu.TwoLetterISOLanguageName.ToLower();
        }


        public static void BackupLanguageOnAppLaunch()
        {
            AppSettings.Instance.LanguageOnAppLaunchBck = Thread.CurrentThread.CurrentUICulture;
        }

        public static CultureInfo DetermineLanguageToUse()
        {
            if (AppSettings.Instance.HasAppLanguageSetted)
            {
                CultureInfo newCu = AppSettings.Instance.AppCurrentLanguage;
                Thread.CurrentThread.CurrentCulture = newCu;
                Thread.CurrentThread.CurrentUICulture = newCu;
                return AppSettings.Instance.AppCurrentLanguage;
            }

            CultureInfo cu = Thread.CurrentThread.CurrentUICulture;
            if (cu.TwoLetterISOLanguageName.ToLower().Equals(Constants.FR_IDENTIFIER))
                AppSettings.Instance.AppCurrentLanguage = Constants.FR_CU;
            else
                AppSettings.Instance.AppCurrentLanguage = Constants.EN_CU;

            CultureInfo cu2 = AppSettings.Instance.AppCurrentLanguage;
            Thread.CurrentThread.CurrentCulture = cu2;
            Thread.CurrentThread.CurrentUICulture = cu2;

            return AppSettings.Instance.AppCurrentLanguage;
        }

        public static bool SetCurrentLanguage(CultureInfo cu)
        {
            if (AppSettings.Instance.AppCurrentLanguage.Equals(cu)
                && Thread.CurrentThread.CurrentCulture.Equals(cu)
                && Thread.CurrentThread.CurrentUICulture.Equals(cu))
                return false;

            AppSettings.Instance.AppCurrentLanguage = cu;
            Thread.CurrentThread.CurrentCulture = cu;
            Thread.CurrentThread.CurrentUICulture = cu;
            return true;
        }
    }
}
