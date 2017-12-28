using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleMvvmToolkit;
using System.Windows.Media;
using QuizzApp.Core.Helpers;
using QuizzApp.Core.Helpers.Converters;

namespace QuizzApp.Core.Entities
{
    public class DlPackage : ModelBase<DlPackage>
    {
        public DlPackage()
        {
            this.CoverUrl = new List<string>();
            this.RecentPacksDateTime = DateTime.Now;
        }
       
        private int id;
        [JsonProperty("id")]
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged(m => m.Id);
            }
        }

        private int type;
        [JsonProperty("type")]
        public int Type
        {
            get { return type; }
            set
            {
                type = value;
                NotifyPropertyChanged(m => m.Type);
            }
        }

        
        private string title;
        [JsonProperty("title")]
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyPropertyChanged(m => m.Title);
            }
        }


        private long unixCreationDate;
        [JsonProperty("creationDate")]
        public long UnixCreationDate
        {
            get { return unixCreationDate; }
            set
            {
                if (unixCreationDate != value)
                {
                    unixCreationDate = value;
                    DateTime creationDate = DateTimeHelpers.FromUnixTime(unixCreationDate);
                    this.CreationDate = creationDate;
                    NotifyPropertyChanged(m => m.UnixCreationDate);
                    NotifyPropertyChanged(m => m.CreationDate);
                }
            }
        }      

        private DateTime creationDate;
        public DateTime CreationDate
        {
            get { return creationDate; }
            set
            {
                if (creationDate != value)
                {
                    creationDate = value;
                    this.UnixCreationDate = DateTimeHelpers.ToUnixTime(value);
                    NotifyPropertyChanged(m => m.CreationDate);
                    NotifyPropertyChanged(m => m.UnixCreationDate);
                }
            }
        }

        private double difficulty;
        [JsonProperty("difficulty")]
        public double Difficulty
        {
            get { return difficulty; }
            set
            {
                difficulty = value;
                NotifyPropertyChanged(m => m.Difficulty);
                NotifyPropertyChanged(m => m.Color);
            }
        }

        private int requiredPoints;
        [JsonProperty("requiredPoints")]
        public int RequiredPoints
        {
            get { return requiredPoints; }
            set
            {
                requiredPoints = value;
                NotifyPropertyChanged(m => m.RequiredPoints);
            }
        }


        private int moviesCount;
        [JsonProperty("moviesCount")]
        public int MoviesCount
        {
            get { return moviesCount; }
            set
            {
                moviesCount = value;
                NotifyPropertyChanged(m => m.MoviesCount);
            }
        }



        private int requiredPackId;
        [JsonProperty("requiredPackId")]
        public int RequiredPackId
        {
            get { return requiredPackId; }
            set
            {
                requiredPackId = value;
                NotifyPropertyChanged(m => m.RequiredPackId);
            }
        }


        private string checksum;
        [JsonProperty("checksum")]
        public string Checksum
        {
            get { return checksum; }
            set
            {
                checksum = value;
                NotifyPropertyChanged(m => m.Checksum);
            }
        }


        private long packSize;
        [JsonProperty("packSize")]
        public long PackSize
        {
            get { return packSize; }
            set
            {
                packSize = value;
                NotifyPropertyChanged(m => m.PackSize);
            }
        }

        private string price;
        [JsonProperty("price")]
        public string Price
        {
            get { return price; }
            set
            {
                price = value;
                NotifyPropertyChanged(m => m.Price);
                NotifyPropertyChanged(m => m.IsFree);
            }
        }


        private List<string> coverUrl;
        [JsonProperty("coverUrl")]
        public List<string> CoverUrl
        {
            get { return coverUrl; }
            set
            {
                coverUrl = value;
                NotifyPropertyChanged(m => m.CoverUrl);
            }
        }

        public bool IsFree
        {
            get 
            {
                if (price.Equals("") || price.Equals("0"))
                    return true;
                else
                    return false;            
            }            
        }


        public Color Color
        {
            get
            {
                return PackDifficultyToColor.GetPackColor(this.Difficulty, false);
            }
        }


        public bool IsNewPack
        {
            get 
            {
                if (this.CreationDate >= this.RecentPacksDateTime)
                    return true;
                else
                    return false;            
            }            
        }        

        private DateTime recentPacksDateTime;
        public DateTime RecentPacksDateTime
        {
            get { return recentPacksDateTime; }
            set
            {
                recentPacksDateTime = value;
                NotifyPropertyChanged(m => m.IsNewPack);
            }
        }

        private bool isDownloading;
        public bool IsDownloading
        {
            get { return isDownloading; }
            set
            {
                isDownloading = value;
                NotifyPropertyChanged(m => m.IsDownloading);
            }
        }
    }
}
