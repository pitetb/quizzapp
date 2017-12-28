using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using QuizzApp.Core.Helpers.Converters;
using System.Windows.Media;

namespace QuizzApp.WP.ViewModels
{
    public class DifficultyPresenter : ModelBase<DifficultyPresenter>
    {
        public DifficultyPresenter()
        {
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

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged(m => m.Name);
            }
        }

        private ObservableCollection<LevelPresenter> levels = new ObservableCollection<LevelPresenter>();
        public ObservableCollection<LevelPresenter> Levels
        {
            get { return levels; }
            set
            {
                levels = value;
                NotifyPropertyChanged(m => m.Levels);
            }
        }

        private int totalTerminated;
        public int TotalTerminated
        {
            get { return totalTerminated; }
            set
            {
                totalTerminated = value;
                NotifyPropertyChanged(m => m.TotalTerminated);
            }
        }


    }

    public class LevelPresenter : ModelBase<LevelPresenter>
    {
        public LevelPresenter()
        {
            this.PacksIdTerminatedOnV1 = new List<int>();
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

        private int number;
        public int Number
        {
            get { return number; }
            set
            {
                number = value;
                NotifyPropertyChanged(m => m.Number);
            }
        }

        private double percentProgress;
        public double PercentProgress
        {
            get { return percentProgress; }
            set
            {
                percentProgress = value;
                NotifyPropertyChanged(m => m.PercentProgress);
            }
        }

        private bool isLocal;
        public bool IsLocal
        {
            get { return isLocal; }
            set
            {
                isLocal = value;
                NotifyPropertyChanged(m => m.IsLocal);
            }
        }

        public List<int> PacksIdTerminatedOnV1 { get; private set; }


        private string title1;
        public string Title1
        {
            get { return title1; }
            set
            {
                title1 = value;
                NotifyPropertyChanged(m => m.Title1);
            }
        }

        private double difficultyTitle1;
        public double DifficultyTitle1
        {
            get { return difficultyTitle1; }
            set
            {
                difficultyTitle1 = value;
                NotifyPropertyChanged(m => m.DifficultyTitle1);
                NotifyPropertyChanged(m => m.ColorTitle1);
            }
        }

        public Brush ColorTitle1
        {
            get
            {
                return new SolidColorBrush(PackDifficultyToColor.GetPackColor(this.DifficultyTitle1, false));
            }
        }

        private string title2;
        public string Title2
        {
            get { return title2; }
            set
            {
                title2 = value;
                NotifyPropertyChanged(m => m.Title2);
            }
        }
       


        private double difficultyTitle2;
        public double DifficultyTitle2
        {
            get { return difficultyTitle2; }
            set
            {
                difficultyTitle2 = value;
                NotifyPropertyChanged(m => m.DifficultyTitle2);
                NotifyPropertyChanged(m => m.ColorTitle2);
            }
        }

       
        public Brush ColorTitle2
        {
            get 
            { 
                return new SolidColorBrush(PackDifficultyToColor.GetPackColor(this.DifficultyTitle2, false)); 
            }            
        }


        private string title3;
        public string Title3
        {
            get { return title3; }
            set
            {
                title3 = value;
                NotifyPropertyChanged(m => m.Title3);
            }
        }


        private double difficultyTitle3;
        public double DifficultyTitle3
        {
            get { return difficultyTitle3; }
            set
            {
                difficultyTitle3 = value;
                NotifyPropertyChanged(m => m.DifficultyTitle3);
                NotifyPropertyChanged(m => m.ColorTitle3);
            }
        }

        public Brush ColorTitle3
        {
            get
            {
                return new SolidColorBrush(PackDifficultyToColor.GetPackColor(this.DifficultyTitle3, false));
            }
        }
    }
}
