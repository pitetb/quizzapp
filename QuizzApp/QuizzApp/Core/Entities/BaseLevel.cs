using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizzApp.Core.Entities
{
    public class BaseLevel : ModelBase<BaseLevel>
    {
        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged(m => m.Id);
            }
        }

        private int val;
        public int Val
        {
            get { return val; }
            set
            {
                val = value;
                NotifyPropertyChanged(m => m.Val);
            }
        }

        private int difficultyId;
        public int DifficultyId
        {
            get { return difficultyId; }
            set
            {
                difficultyId = value;
                NotifyPropertyChanged(m => m.DifficultyId);
            }
        }


        private DateTime releaseDate;
        public DateTime ReleaseDate
        {
            get { return releaseDate; }
            set
            {
                releaseDate = value;
                NotifyPropertyChanged(m => m.ReleaseDate);
            }
        }

        private string language;
        public string Language
        {
            get { return language; }
            set
            {
                language = value;
                NotifyPropertyChanged(m => m.Language);
            }
        }

        private string md5;
        public string Md5
        {
            get { return md5; }
            set
            {
                md5 = value;
                NotifyPropertyChanged(m => m.Md5);
            }
        }


        private long zipSize;
        public long ZipSize
        {
            get { return zipSize; }
            set
            {
                zipSize = value;
                NotifyPropertyChanged(m => m.ZipSize);
            }
        }
                
        
        private List<int> packsIds;
        public List<int> PackIds
        {
            get { return packsIds; }
            set
            {
                packsIds = value;
                NotifyPropertyChanged(m => m.PackIds);
            }
        }

    }


}
