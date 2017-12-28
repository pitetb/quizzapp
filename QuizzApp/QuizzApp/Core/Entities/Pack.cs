using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;
using System.Globalization;
using QuizzApp.Core.Helpers.Converters;
using QuizzApp.Core.Helpers;
using System.Collections.ObjectModel;

namespace QuizzApp.Core.Entities
{
    public class Pack : ModelBase<Pack>
    {

        public Pack()
        {
            this.Medias = new ObservableCollection<Media>();
        }

        public void AddMedia(Media media)
        {
            if (media == null)
                return;
            this.Medias.Add(media);
            media.PropertyChanged += media_PropertyChanged;
        }

        private void media_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsCompleted"))
            {
                NotifyPropertyChanged(m => m.IsTerminated);
                NotifyPropertyChanged(m => m.Points);
                NotifyPropertyChanged(m => m.IsTerminated);
            }
        }

        public void RemoveMedia(Media media)
        {
            if (media == null)
                return;
            if (this.Medias.Contains(media))
            {
                this.Medias.Remove(media);
                media.PropertyChanged -= media_PropertyChanged;
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


        private int levelId;
        public int LevelId
        {
            get { return levelId; }
            set
            {
                levelId = value;
                NotifyPropertyChanged(m => m.LevelId);
            }
        }
        

        private double difficulty;
        public double Difficulty
        {
            get { return difficulty; }
            set
            {
                difficulty = value;
                NotifyPropertyChanged(m => m.Difficulty);
            }
        }

        public double SortIndexWithDifficultyAndLock
        {
            get
            {
                return this.Difficulty;
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

        private string author;
        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                NotifyPropertyChanged(m => m.Author);
            }
        }

        private ObservableCollection<Media> medias;
        public ObservableCollection<Media> Medias
        {
            get { return medias; }
            private set
            {
                medias = value;
                NotifyPropertyChanged(m => m.Medias);
            }
        }

        public int NbCompletedItems
        {
            get 
            {
                int completed = 0;
                if (this.Medias == null)
                    return completed;

                foreach (var movie in this.Medias)
                {
                    if (movie.IsCompleted)
                    {
                        completed++;
                    }
                }

                return completed;
            }
        }

        public int Points
        {
            get { 
     
		        int points = 0;
                if (this.Medias == null)
                    return points;

                //foreach (Media movie in this.Medias)
                //{
                //    if (movie.IsCompleted)
                //    {
                //        points += movie.Points;
                //    }
                //}
		        if(this.IsTerminated)
		        {
			        points += (int)(Math.Ceiling(this.Difficulty) * 5);
		        }
		        return points;
	        }           
        }

        public int PossiblePoints
        {
            get
            {
                return (int) Math.Round((this.Difficulty * 50), 0);
                //int points = 0;
                //if (this.Medias == null)
                //    return points;

                //foreach (Media movie in this.Medias)
                //    points += (int)(Math.Ceiling(this.Difficulty) * 5);
            
                //return points;
            }
        }

        public bool IsTerminated
        {
            get 
            {
                if (this.Medias != null)
                    return this.NbCompletedItems == this.Medias.Count;
                else
                    return false;
            }
        }



        private bool isRemotePack = false;
        public bool IsRemotePack
        {
            get { return isRemotePack; }
            set
            {
                isRemotePack = value;
                NotifyPropertyChanged(m => m.IsRemotePack);
            }
        }
               
        
        public double TestTitle(int movieIndex, string titre)
	    {
            Media movie = this.Medias[movieIndex];

		    int distance = 0;
		    double justesse = 0;

		    string titreDonne = titre.ToLower(CultureInfo.CurrentUICulture);
            titreDonne = StringTools.KeepLettersAndNumbers((StringTools.RemoveAccents(titreDonne)));

		    foreach (var item in movie.Variants)
	        {
                string titreVrai = item.ToLower(CultureInfo.CurrentUICulture);
			    titreVrai = StringTools.KeepLettersAndNumbers((StringTools.RemoveAccents(titreVrai)));

                distance = Levenshtein.LevenshteinDistance(titreDonne, titreVrai);
			    justesse = Math.Max(justesse, 1.0 - ((double) distance / (double) (titreVrai.Length + titreDonne.Length)));
		    }

		    return justesse;
	    }


        public double TestIncompleteTitle(int movieIndex, string titre)
	    {
            Media movie = this.Medias[movieIndex];

		    string titreDonne = titre.ToLower(CultureInfo.CurrentUICulture);
            titreDonne = StringTools.KeepLettersAndNumbers((StringTools.RemoveAccents(titreDonne)));

		    String titreVraiDebut;
		    String titreVraiFin;

		    int distanceDebut;
		    int distanceFin;
		    double justesseDebut;
		    double justesseFin;
		    double justesse = 0;

            foreach (string item in movie.Variants)
            {
                string titreVrai = item.ToLower(CultureInfo.CurrentUICulture);
                titreVrai = StringTools.KeepLettersAndNumbers((StringTools.RemoveAccents(titreVrai)));

                titreVraiDebut = titreVrai.ToLower(CultureInfo.CurrentUICulture);
                titreVraiDebut = StringTools.KeepLettersAndNumbers((StringTools.RemoveAccents(titreVraiDebut)));

                titreVraiFin = titreVrai.ToLower(CultureInfo.CurrentUICulture);
                titreVraiFin = StringTools.KeepLettersAndNumbers((StringTools.RemoveAccents(titreVraiFin)));

			    if (titreDonne.Length >= titreVrai.Length)
			    {
				    continue;
			    }

			    titreVraiDebut = titreVraiDebut.Substring(0, titreDonne.Length);
                int startIndex = titreVraiFin.Length - titreDonne.Length;
                titreVraiFin = titreVraiFin.Substring(startIndex, titreVraiFin.Length - startIndex );

                distanceDebut = Levenshtein.LevenshteinDistance(titreDonne, titreVraiDebut);
                justesseDebut = 1.0 - ((double)distanceDebut / (double)(titreVraiDebut.Length + titreDonne.Length));

                distanceFin = Levenshtein.LevenshteinDistance(titreDonne, titreVraiFin);
                justesseFin = 1.0 - ((double)distanceFin / (double)(titreVraiFin.Length + titreDonne.Length));

			    justesse = Math.Max(justesse, justesseDebut);
			    justesse = Math.Max(justesse, justesseFin);
		    }

		    return justesse;
	    }
    }
}
