using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizzApp.WP.ViewModels.Helpers
{
    public static class GamePage
    {
        public const string PAGE_NAME = "GamePage";
        public const string PAGE_PARAM_PACKID = "packId";        
    }

    public static class SettingsPage
    {
        public const string PAGE_NAME = "SettingsPage";
        public const string PAGE_PARAM_PACKID = "packId";
    }


    public static class HelpPage
    {
        public const string PAGE_NAME = "HelpPage";
        public const string PAGE_PARAM_NEXT_PAGE_NAME = "next";
        public const string PAGE_PARAM_NEXT_PAGE_PARAMS = "nextParams";
        
    }

    public static class LevelsPage
    {
        public const string PAGE_NAME = "LevelsPage";
    }

    public static class PacksListPage
    {
        public const string PAGE_NAME = "PacksListPage";
        public const string PAGE_PARAM_LEVEL_ID_INT = "levelId";
    }

    public static class MainPage
    {
        public const string PAGE_NAME = "MainPage";
    }

    
}
