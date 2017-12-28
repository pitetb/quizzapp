using System;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;

// Toolkit namespace
using SimpleMvvmToolkit;

// Toolkit extension methods
using SimpleMvvmToolkit.ModelExtensions;
using System.Diagnostics;
using QuizzApp.FakeData;
using System.Windows.Media;
using QuizzApp.Core.Helpers.Converters;
using QuizzApp.Core.Entities;
using System.Collections.Generic;

namespace QuizzApp.WP.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvmprop</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// </summary>
    public class PackagePresenterViewModel : ViewModelBaseWithDesign<PackagePresenterViewModel>
    {
        #region Initialization and Cleanup

        private Pack package;
        public Pack Package
        {
            get { return package; }
            set
            {
                if (package != value)
                {
                    if (package != null)
                        package.PropertyChanged -= package_PropertyChanged;

                    package = value;
                    RebindCurrentPackageProperties();

                    if (package != null)
                        package.PropertyChanged += package_PropertyChanged;
                }
            }
        }

        private void package_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RebindCurrentPackageProperties();
        }



        // Default ctor
        public PackagePresenterViewModel()
        {
            if (this.IsInDesignMode)
            {
                DesignDataProvider data = new DesignDataProvider();
                this.Package = data.APackage;
                this.Title = data.PackageTitle;
                this.totalItems = 10;
                this.SolvedItems = 8;
                this.DebugString = "Design Mode";
                this.Movie1 = data.Movie1;
                this.Movie2 = data.Movie2;
                this.Movie3 = data.Movie3;
                this.Color = data.APackageColor1;
                this.MediasResolved = new List<bool>();
                for (int i = 0; i < 10; i++)
                {
                    if (i == 0)
                        this.MediasResolved.Add(true);
                    else if (i == 2)
                        this.MediasResolved.Add(true);
                    else if (i == 3)
                        this.MediasResolved.Add(true);
                    else if (i == 6)
                        this.MediasResolved.Add(true);
                    else if (i == 7)
                        this.MediasResolved.Add(true);
                    else
                        this.MediasResolved.Add(false);
                }
                this.TotalTime = new TimeSpan(1, 2, 3, 4, 5);
            }
        }

        #endregion

        #region Notifications

        // Add events to notify the view or obtain data from the view
        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        #endregion

        #region Properties



        public double PercentProgress
        {
            get
            {
                if (TotalItems == 0)
                    return 100;
                else
                {
                    return (((double)this.SolvedItems) / ((double)this.TotalItems) * 100);
                }
            }
        }


        private bool isLocked;
        public bool IsLocked
        {
            get { return isLocked; }
            set
            {
                isLocked = value;
                NotifyPropertyChanged(m => m.IsLocked);
                this.RecalculatePackageColor();
            }
        }

        public bool IsCompleted
        {
            get
            {
                return TotalItems == SolvedItems;
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

        private int totalItems = 1;
        public int TotalItems
        {
            get { return totalItems; }
            set
            {
                totalItems = value;
                NotifyPropertyChanged(m => m.TotalItems);
                NotifyPropertyChanged(m => m.PercentProgress);
                NotifyPropertyChanged(m => m.IsCompleted);
            }
        }

        private int solvedItems;
        public int SolvedItems
        {
            get { return solvedItems; }
            set
            {
                solvedItems = value;
                NotifyPropertyChanged(m => m.SolvedItems);
                NotifyPropertyChanged(m => m.PercentProgress);
                NotifyPropertyChanged(m => m.IsCompleted);
            }
        }

        private string debugString;
        public string DebugString
        {
            get { return debugString; }
            set
            {
                debugString = value;
                NotifyPropertyChanged(m => m.DebugString);
            }
        }



        private Media movie1;
        public Media Movie1
        {
            get { return movie1; }
            set
            {
                movie1 = value;
                NotifyPropertyChanged(m => m.Movie1);
            }
        }


        private Media movie2;
        public Media Movie2
        {
            get { return movie2; }
            set
            {
                movie2 = value;
                NotifyPropertyChanged(m => m.Movie2);
            }
        }

        private Media movie3;
        public Media Movie3
        {
            get { return movie3; }
            set
            {
                movie3 = value;
                NotifyPropertyChanged(m => m.Movie3);
            }
        }


        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                NotifyPropertyChanged(m => m.Color);
            }
        }


        private List<bool> mediasResolved;
        public List<bool> MediasResolved
        {
            get { return mediasResolved; }
            set
            {
                mediasResolved = value;
                NotifyPropertyChanged(m => m.MediasResolved);
            }
        }


        public void RecalculatePackageColor()
        {
            this.Color = PackDifficultyToColor.GetPackColor(this.Difficulty, this.IsLocked);

        }

        private double difficulty;
        public double Difficulty
        {
            get { return difficulty; }
            set
            {
                difficulty = value;
                NotifyPropertyChanged(m => m.Difficulty);
                this.RecalculatePackageColor();
            }
        }

        private int points;
        public int Points
        {
            get { return points; }
            set
            {
                points = value;
                NotifyPropertyChanged(m => m.Points);
            }
        }

        private int totalPoints;
        public int TotalPoints
        {
            get { return totalPoints; }
            set
            {
                totalPoints = value;
                NotifyPropertyChanged(m => m.TotalPoints);
            }
        }

        private TimeSpan totalTime;
        public TimeSpan TotalTime
        {
            get { return totalTime; }
            set
            {
                totalTime = value;
                NotifyPropertyChanged(m => m.TotalTime);
            }
        }


        // STEP: Add an INavigate property to CustomerViewModel
        private INavigator navigator;
        public INavigator Navigator
        {
            get
            {
                if (navigator == null)
                    navigator = new Navigator();

                return navigator;
            }
        }

        #endregion

        #region Methods

        public void RebindCurrentPackageProperties()
        {
            if (this.Package == null)
            {
                this.TotalItems = 0;
                this.SolvedItems = 0;
                this.Title = string.Empty;
                this.Movie1 = null;
                this.Movie2 = null;
                this.Movie3 = null;
                this.Color = Colors.Black;
                this.Points = 0;
                this.TotalTime = new TimeSpan();
                this.IsLocked = false;
                this.Difficulty = 0;
                this.MediasResolved = new List<bool>();
                this.TotalPoints = 0;
            }
            else
            {
                if (package.Medias != null)
                {
                    this.TotalItems = package.Medias.Count;
                    this.SolvedItems = package.NbCompletedItems;
                    this.Title = package.Title;
                    
                    this.Difficulty = package.Difficulty;
                    this.TotalTime = new TimeSpan();
                    this.IsLocked = false;
                    this.TotalPoints = package.PossiblePoints;

                    List<bool> items = new List<bool>();
                    for (int i = 0; i < this.Package.Medias.Count; i++)
                    {
                        var item = this.Package.Medias[i];
                    
                        if (i == 0 )
                            this.Movie1 = package.Medias[i];
                        else if (i == 1)
                            this.Movie2 = package.Medias[i];
                        else if (i == 2)
                            this.Movie3 = package.Medias[i];
    

                        this.Points = package.Points;
                        if (item.IsCompleted)
                            items.Add(true);
                        else
                            items.Add(false);
                    }
                    this.MediasResolved = items;
                    this.RecalculatePackageColor();
                }
                else
                {
                    this.MediasResolved = new List<bool>();
                }
            }
        }

        // STEP: Add a Navigate method accepting a page name
        public void NavigateToPackagesTypesPage()
        {
            // Navigator.NavigateTo(typeof(PackagesTypesPage).Name);
        }

        #endregion

        #region Completion Callbacks

        #endregion

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}