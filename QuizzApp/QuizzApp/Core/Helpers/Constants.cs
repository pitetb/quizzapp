using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace QuizzApp.Core.Helpers
{
    public static class Constants
    {
        public const string PROPOSE_PACK_URL = @"http://www.quizz-app.com/pre_packs/propose";
        public const string QUIZZ_APP_WEB_SITE = @"http://www.quizz-app.com";

        public const string FR_IDENTIFIER = "fr";
        public const string EN_IDENTIFIER = "en";
        public static CultureInfo FR_CU = new CultureInfo(FR_IDENTIFIER);
        public static CultureInfo EN_CU = new CultureInfo(EN_IDENTIFIER);

        public const int QUIZZ_APP_HELP_LIMIT = 30;
    }
}
