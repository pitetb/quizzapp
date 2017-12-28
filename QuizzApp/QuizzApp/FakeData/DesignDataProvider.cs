using QuizzApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QuizzApp.FakeData
{
    public class DesignDataProvider
    {
        public MediaImage Poster1 { get; set; }
        public MediaImage Poster2 { get; set; }
        public MediaImage Poster3 { get; set; }

        public string DesignString { get; set; }
        public string PackageTitle { get; set; }
        public int NbItemsResolved { get; set; }
        public int NbItemsTotal { get; set; }

        public Media Movie1 { get; set; }
        public Media Movie2 { get; set; }
        public Media Movie3 { get; set; }

        public Pack APackage { get; set; }
        public Color APackageColor1 { get; set; }
        public Color APackageColor2 { get; set; }
        public List<Pack> APackageList { get; set; }

        public DlPackage ADlPack { get; set; }
        public List<DlPackage> ADlPackList { get; set; }

        public Level ALevel { get; set; }

        public DesignDataProvider()
        {
            this.DesignString = "A design string";

            // Create first Poster
            //0.21388888888889;0.72291666666667;0.78333333333333;0.87291666666667
            List<Rect> rects = new List<Rect>();
            rects.Add(new Rect(new Point(0.21388888888889,0.72291666666667), new Point(0.78333333333333,0.87291666666667)));
            this.Poster1 = new MediaImage(rects, true, true);
            //this.Poster1.Image = GetSampleImageBytes("avatar.jpg");
            this.Poster1.DesignPicture = "themask.jpg";
            
            // Create second Poster
            List<Rect> rects2 = new List<Rect>();
            rects2.Add(new Rect(new Point(0.21388888888889, 0.72291666666667), new Point(0.78333333333333, 0.87291666666667)));
            this.Poster2 = new MediaImage(rects2, true, true);
            //this.Poster2.Image = GetSampleImageBytes("themask.jpg");
            this.Poster2.DesignPicture = "themask.jpg";

            // Create third Poster
            List<Rect> rects3 = new List<Rect>();
            rects3.Add(new Rect(new Point(0.21388888888889, 0.72291666666667), new Point(0.78333333333333, 0.87291666666667)));
            this.Poster3 = new MediaImage(rects3, true, true);
            //this.Poster3.Image = GetSampleImageBytes("fightclub.jpg");
            this.Poster3.DesignPicture = "themask.jpg";

            this.PackageTitle = "Les grands classiques";
            this.NbItemsResolved = 8;
            this.NbItemsTotal = 10;

            this.Movie1 = new Media(){ Title = "THe First Movie", Poster = Poster1 };
            this.Movie1.Difficulty = 1;
            this.Movie1.Position = 1;
            this.Movie1.Time = new TimeSpan(0, 1, 25, 30, 2456);
            this.Movie2 = new Media() { Title = "THe Second Movie", Poster = Poster2 };
            this.Movie2.Difficulty = 1; 
            this.Movie2.Position = 2;
            this.Movie2.IsCompleted = true;
            this.Movie3 = new Media() { Title = "THe Third Movie", Poster = Poster3 };
            this.Movie3.Difficulty = 1; 
            this.Movie3.Position = 3;
            this.APackage = new Pack();
            this.APackage.Title = "A Design Package 1";
            this.APackage.Id = 1;
            this.APackage.AddMedia(this.Movie1);
            this.APackage.AddMedia(this.Movie2);
            this.APackage.AddMedia(this.Movie3);
            this.APackage.AddMedia(this.Movie1);
            this.APackage.AddMedia(this.Movie1);
            this.APackage.AddMedia(this.Movie1);
            this.APackage.AddMedia(this.Movie1);
            this.APackage.AddMedia(this.Movie1);
            this.APackage.AddMedia(this.Movie1);
            this.APackage.AddMedia(this.Movie1);
            this.APackage.Difficulty = 1.9;
            this.APackageList = new List<Pack>();
            this.APackageList.Add(APackage);

            Pack aPackage2 = new Pack();
            aPackage2.Title = "A Design Package 2";
            aPackage2.Id = 2;
            aPackage2.AddMedia(this.Movie1);
            aPackage2.AddMedia(this.Movie2);
            aPackage2.AddMedia(this.Movie3);
            this.APackageList.Add(aPackage2);

            Pack aPackage3 = new Pack();
            aPackage3.Title = "A Design Package 3";
            aPackage3.Id = 2;
            aPackage3.AddMedia(this.Movie1);
            aPackage3.AddMedia(this.Movie2);
            aPackage3.AddMedia(this.Movie3);
            this.APackageList.Add(aPackage3);
            
            this.APackageColor2 = Colors.Orange;
            this.APackageColor1 = Colors.Green;
            //this.Movie2 = new Movie("", textArea, false, "THe Second Movie") { Poster = Poster2 };
            //this.Movie3 = new Movie("", textArea, false, "THe Third Movie") { Poster = Poster3 };

            ADlPackList = new List<DlPackage>();
            ADlPack = new DlPackage();
            ADlPack.Checksum = "156798";
            ADlPack.CoverUrl = new List<string>() { "http://virtapp.fr/android/moviequizz/films/covers_mini/34569.jpg" };
            ADlPack.CreationDate = DateTime.Now;
            ADlPack.Difficulty = 1;
            ADlPack.Id = 1;
            ADlPack.MoviesCount = 10;
            ADlPack.PackSize = 13224;           
            ADlPack.Price = "0";
            ADlPack.Title = "Un Package a telecharger";
            ADlPackList.Add(ADlPack);
            ADlPackList.Add(ADlPack);

            this.ALevel = new Level();
            this.ALevel.DifficultyId = 1;
            this.ALevel.Id = 1;
            this.ALevel.Language = "fr";
            this.ALevel.NbPacksTerminated = 3;
            this.ALevel.PackIds = new List<int>();
            this.ALevel.Packs = this.APackageList;
            //this.ALevel.Progression = 0.3f;
            this.ALevel.ReleaseDate = DateTime.Now;
            this.ALevel.Val = 1;
        }





        private static WriteableBitmap GetSampleImageBytes(string fileName)
        {
            //var assemblyName = new AssemblyName(
            //    Assembly.GetExecutingAssembly().FullName);
            //var resourceUri = new Uri("FakeData/Images/lavieestbelle.jpg");
            //using (var stream = Application.GetResourceStream(resourceUri).Stream)
            //{
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        stream.CopyTo(memoryStream);
            //        return memoryStream.ToArray();
            //    }
            //}

           var resourceUri = new Uri("FakeData/Images/" + fileName, UriKind.Relative);
           using (var stream = Application.GetResourceStream(resourceUri).Stream)
           {
               return new WriteableBitmap(0, 0).FromStream(stream);               
           }
        }
    }
}
