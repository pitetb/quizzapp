using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace QuizzApp.Core.Entities
{
    public class Media : ModelBase<Media>
    {

        // Media
        // 'id' int(11),
        //'title' text ,
        //'rects' text ,
        //'difficulty' int(11) ,
        //'language' text ,
        //'time' real,
        //'completed' int(11), // IGNORE IT
        //'variants' text ,
        //'extra1' text ,
        //'extra2' text ,
        //'extra3' text ,
        //'fextra1' text ,
        //'fextra2' text ,
        //'fextra3' text ,

        public Media()
        {
            this.Variants = new List<string>();
        }

        public Media(string pathToFile, List<Rect> floutedAreas, bool isPixelized, string title, int position) : this()
        {
            this.Title = title;
            this.Position = position;
            this.Poster = new MediaImage(pathToFile, floutedAreas, isPixelized, position <= 3);
        }

        private MediaImage poster;
        public MediaImage Poster
        {
            get { return poster; }
            set
            {
                poster = value;
                NotifyPropertyChanged(m => m.Poster);
            }
        }

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

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyPropertyChanged(m => m.Title);
            }
        }

        
        private int difficulty;
        public int Difficulty
        {
            get { return difficulty; }
            set
            {
                difficulty = value;
                NotifyPropertyChanged(m => m.Difficulty);
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


        private string extra1;
        public string Extra1
        {
            get { return extra1; }
            set
            {
                extra1 = value;
                NotifyPropertyChanged(m => m.Extra1);
            }
        }

        private string extra2;
        public string Extra2
        {
            get { return extra2; }
            set
            {
                extra2 = value;
                NotifyPropertyChanged(m => m.Extra2);
            }
        }
        
        private string extra3;
        public string Extra3
        {
            get { return extra3; }
            set
            {
                extra3 = value;
                NotifyPropertyChanged(m => m.Extra3);
            }
        }



        private float? fextra1;
        public float? FExtra1
        {
            get { return fextra1; }
            set
            {
                fextra1 = value;
                NotifyPropertyChanged(m => m.FExtra1);
            }
        }

        private float? fextra2;
        public float? FExtra2
        {
            get { return fextra2; }
            set
            {
                fextra2 = value;
                NotifyPropertyChanged(m => m.FExtra2);
            }
        }

        private float? fextra3;
        public float? FExtra3
        {
            get { return fextra3; }
            set
            {
                fextra3 = value;
                NotifyPropertyChanged(m => m.FExtra3);
            }
        }
        


        
        private TimeSpan time;
        public TimeSpan Time
        {
            get { return time; }
            set
            {
                time = value;
                NotifyPropertyChanged(m => m.Time);
                CalculateIfHelpAvailable(time);
            }
        }

        private bool isHelpAvailable = false;
        public bool IsHelpAvailable
        {
            get
            {
                return isHelpAvailable;
            }
            private set
            {
                isHelpAvailable = value;
                NotifyPropertyChanged(m => m.IsHelpAvailable);
            }
        }

        private void CalculateIfHelpAvailable(TimeSpan timePassed)
        {
            int nbSecBefHelp = this.GetTotalSecondsBeforeHelp();
            if (timePassed >= TimeSpan.FromSeconds(nbSecBefHelp)
                && IsCompleted == false)
            {
                this.IsHelpAvailable = true;
            }
            else
                this.IsHelpAvailable = false;
        }

        
        public int GetTotalSecondsBeforeHelp()
        {     
#if DEBUG
            // HACK FOR DEBUG
            return 20;
#else
            // NORMAL CASE
            double secondes = this.Difficulty * (2 * 60 - 1 * 60.0) / 10 + (1 * 60.0);
            secondes = Math.Min(secondes, 2 * 60.0);
            secondes = Math.Max(secondes, 1 * 60.0);
            return (int) secondes;
#endif
        }

        private bool isCompleted = false;
        public bool IsCompleted
        {
            get { return isCompleted; }
            set
            {
                if (isCompleted != value)
                {
                    isCompleted = value;
                    NotifyPropertyChanged(m => m.IsCompleted);

                    if (IsCompleted)
                        this.IsHelpAvailable = false;                   

                    if (this.Poster != null)
                        this.Poster.IsPixelized = ! isCompleted;
                }
            }
        }


        private string variantsString;
        public string VariantsString
        {
            get { return variantsString; }
            set
            {
                variantsString = value;
                NotifyPropertyChanged(m => m.VariantsString);
            }
        }

        private List<string> variants;
        public List<string> Variants
        {
            get {
                //if ( (variants != null) && ! string.IsNullOrEmpty(this.Title) && (! variants.Contains(this.Title)))
                //{
                //    variants.Add(this.Title);
                //}
                return variants; 
            }
            set
            {
                variants = value;
                NotifyPropertyChanged(m => m.Variants);
            }
        }


        private int position;
        public int Position
        {
            get { return position; }
            set
            {
                position = value;
                NotifyPropertyChanged(m => m.Position);
            }
        }

        public string GetHelpString()
        {

            var chars = CultureInfo.CurrentCulture.TextInfo.ToUpper(this.Title).ToCharArray();

            StringBuilder sb = new StringBuilder();

            bool nextIsClear = true;

            for (int i = 0; i < chars.Length; i++)
            {
                //string caract = StringTools.RemoveAccents(chars[i].ToString());

                if (chars[i] == ' ')
                {
                    sb.Append(chars[i]);
                    nextIsClear = true;
                }
                else if (nextIsClear)
                {
                    sb.Append(chars[i]);
                    nextIsClear = false;
                }
                else
                {
                    sb.Append("_");
                }
                sb.Append(" ");
            }            
            return sb.ToString();
        }

          
    }

}
