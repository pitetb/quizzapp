using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleMvvmToolkit;
using QuizzApp.Core.Helpers;

namespace QuizzApp.Core.Entities.Json
{
    public class InitInfosRep : ModelBase<InitInfosRep>
    {   
        private long unixLevelRecentDate;
        [JsonProperty("level_recent_date")]
        public long UnixRecentPackDate
        {
            get { return unixLevelRecentDate; }
            set
            {
                if (unixLevelRecentDate != value)
                {
                    unixLevelRecentDate = value;
                    DateTime creationDate = DateTimeHelpers.FromUnixTime(unixLevelRecentDate);
                    this.RecentLevelDate = creationDate;
                    NotifyPropertyChanged(m => m.UnixRecentPackDate);
                }
            }
        }

        private DateTime recenLevelDate;
        public DateTime RecentLevelDate
        {
            get { return recenLevelDate; }
            set
            {
                if (recenLevelDate != value)
                {
                    recenLevelDate = value;
                    this.UnixRecentPackDate = DateTimeHelpers.ToUnixTime(value);
                    NotifyPropertyChanged(m => m.RecentLevelDate);                    
                }
            }
        }


        [JsonProperty("difficulties")]
        public List<InitDifficultyByLang> Difficulties { get; set; }


        public InitInfosRep()
        {
            this.Difficulties = new List<InitDifficultyByLang>();
        }
    }


    public class InitDifficulty
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("enum_value")]
        public int EnumValue { get; set; }
    }

    public class InitLanguage
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class InitDifficultyByLang
    {

        [JsonProperty("Difficulty")]
        public InitDifficulty Difficulty { get; set; }

        [JsonProperty("Language")]
        public InitLanguage Language { get; set; }
    }

}
