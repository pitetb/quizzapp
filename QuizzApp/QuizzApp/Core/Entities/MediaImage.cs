using Microsoft.Phone;
using QuizzApp.Core.Helpers;
using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace QuizzApp.Core.Entities
{
    public class MediaImage : ModelBase<MediaImage>
    {

        private static int BITMAP_WIDTH = 600;
        private static int BITMAP_HEIGHT = 800;
        private static double THUMBNAIL_RATIO = (double)1 / 8;
        private static string THUMBNAIL_PREFIX = "thumb_";
        private static int PIXELISATION_FACTOR_THUMB = 10;
        private static int PIXELISATION_FACTOR = 75;

        public string PathToFullBitmap;
        public string PathToThumbBitmap;
        public bool HasThumbnail;

        private string pathToDirectory;

        public List<Rect> FloutedAreas;
        public string FloutedAreasString;
        public Point FloutedPercentPointA;
        public Point FloutedPercentPointB;

        // Only here for serialization
        public MediaImage()
        { 
        }

        public MediaImage(string pathToFullBitmapFile, List<Rect> floutedAreas, bool isPixelized, bool hasThumbnail)
            : this(floutedAreas, isPixelized, hasThumbnail)
        {
            this.PathToFullBitmap = pathToFullBitmapFile;
            this.pathToDirectory = Path.GetDirectoryName(pathToFullBitmapFile) + Path.DirectorySeparatorChar;
            this.PathToThumbBitmap = pathToDirectory + THUMBNAIL_PREFIX + pathToFullBitmapFile.Split(Path.DirectorySeparatorChar).Last();

            //this.RefreshFullBitmap();
        }

        #region DesignTime support
        public string DesignPicture { get; set; }

        /// <summary>
        /// This constructor is here just for design time support, do not use in "normal" case, use with "DesignPicture" property
        /// </summary>
        /// <param name="floutedArea"></param>
        /// <param name="isPixelized"></param>
        public MediaImage(List<Rect> floutedAreas, bool isPixelized, bool hasThumbnail)
        {            
            this.FloutedAreas = floutedAreas;
            this.isPixelized = isPixelized;
            this.HasThumbnail = hasThumbnail;
        }

        public void SetParametersAndNotifyChange(List<Rect> floutedAreas, bool isPixelized, bool hasThumbnail)
        {
            this.FloutedAreas = floutedAreas;
            this.isPixelized = isPixelized;
            this.HasThumbnail = hasThumbnail;

            NotifyPropertyChanged(m => m.IsPixelized);
        }

        private static WriteableBitmap GetSampleImageBytes(string fileName)
        {
            var resourceUri = new Uri("FakeData/Images/" + fileName, UriKind.Relative);
            using (var stream = Application.GetResourceStream(resourceUri).Stream)
            {
                //return ReadFully(stream);

                WriteableBitmap bi = new WriteableBitmap(0, 0).FromStream(stream);
                return bi;
            }
        }

        #endregion


        private bool isPixelized;
        public bool IsPixelized
        {
            get { return isPixelized; }
            set
            {
                if (value != isPixelized)
                {
                    isPixelized = value;
                    RefreshFullBitmap();
                    NotifyPropertyChanged(m => m.IsPixelized);

                    if (HasThumbnail)
                        RefreshThumbBitmap();
                }
            }
        }


        //private byte[] fullImage;
        //public byte[] FullImage
        //{
        //    get 
        //    {
        //        if (!string.IsNullOrEmpty(DesignPicture) || App.IsInDesignModeStatic)
        //        {
        //            return GetSampleImageBytes(this.DesignPicture);
        //        }
        //        else if (fullImage == null)
        //        {
        //            //Task.Factory.StartNew(() => RefreshFullBitmap());
        //            RefreshFullBitmap();
        //            return null;
        //        }
        //        else
        //        {
        //            return fullImage;
        //        }
        //    }           
        //}

        //private byte[] thumbImage;
        //public byte[] ThumbImage
        //{
        //    get
        //    {
        //        if (HasThumbnail == false)
        //            return null;

        //        if (!string.IsNullOrEmpty(DesignPicture) || App.IsInDesignModeStatic)
        //        {
        //            return GetSampleImageBytes(this.DesignPicture);
        //        }
        //        else if (thumbImage == null)
        //        {
        //            //Task.Factory.StartNew(() => RefreshThumbBitmap());
        //            RefreshThumbBitmap();
        //            return null;
        //        }
        //        else
        //        {
        //            return thumbImage;
        //        }
        //    }
        //}

        private WriteableBitmap fullImage;
        public WriteableBitmap FullImage
        {
            get
            {
                Debug.WriteLine("Getting full image of " + this.PathToFullBitmap);
                if (!string.IsNullOrEmpty(DesignPicture) || AppDesignMode.IsInDesignModeStatic)
                {
                    return GetSampleImageBytes(this.DesignPicture);
                }
                else if (fullImage == null)
                {
                    //Task.Factory.StartNew(() => RefreshFullBitmap());
                    RefreshFullBitmap();
                    return null;
                }
                else
                {
                    return fullImage;
                }
            }
        }


        private WriteableBitmap thumbImage;
        public WriteableBitmap ThumbImage
        {
            get
            {
                if (HasThumbnail == false)
                    return null;

                if (!string.IsNullOrEmpty(DesignPicture) || AppDesignMode.IsInDesignModeStatic)
                {
                    return GetSampleImageBytes(this.DesignPicture);
                }
                else if (thumbImage == null)
                {
                    //Task.Factory.StartNew(() => RefreshThumbBitmap());
                    RefreshThumbBitmap();
                    return null;
                }
                else
                {
                    return thumbImage;
                }
            }
        }

        private object thumbLock = new object();

        public void GenerateThumbnail()
        {
            // This hack comes from the problem that classes like BitmapImage, WritableBitmap, Image used here could 
            // only be created or accessed from the UI thread. And now this code called from the threadpool. To avoid
            // cross-thread access exceptions, I dispatch the code back to the UI thread, waiting for it to complete
            // using the Monitor and a lock object, and then return the value from the method. Quite hacky, but the only
            // way to make this work currently. It's quite stupid that MS didn't provide any classes to do image 
            // processing on the non-UI threads.

            //var waitHandle = new object();
            //lock (waitHandle)
            //{

                //Deployment.Current.Dispatcher.BeginInvoke(() =>
                //{

                    //lock (waitHandle)
                    //{
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                                {

                                    lock (thumbLock)
                                    {
                                        bool fullBitmapExist = myIsolatedStorage.FileExists(PathToFullBitmap);
                                        bool thumbBitmapExist = myIsolatedStorage.FileExists(PathToThumbBitmap);
                                        if (fullBitmapExist && !thumbBitmapExist)
                                        {
                                            using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(PathToFullBitmap, FileMode.Open, FileAccess.Read))
                                            {
                                                WriteableBitmap bi = null;
                                                Task task = Deployment.Current.Dispatcher.InvokeAsync(() =>
                                                { bi = new WriteableBitmap(0, 0).FromStream(fileStream); });
                                                task.Wait();

                                                using (IsolatedStorageFileStream writeStream = myIsolatedStorage.OpenFile(PathToThumbBitmap, FileMode.Create, FileAccess.Write))
                                                {
                                                    bi.SaveJpeg(writeStream, (int)(BITMAP_WIDTH * THUMBNAIL_RATIO), (int)(BITMAP_HEIGHT * THUMBNAIL_RATIO), 0, 100);
                                                }
                                                //bi.Invalidate();
                                            }
                                        }
                                    }
                                }
                                //Monitor.Pulse(waitHandle);
                            //}
                    });

                //});

                //Monitor.Wait(waitHandle);
            //}
        }





        private bool isRefreshingFullBitmap = false;
        public async void RefreshFullBitmap()
        {
            Debug.WriteLine("RefreshFullBitmap : " + PathToFullBitmap); 
            if (string.IsNullOrEmpty(PathToFullBitmap) || isRefreshingFullBitmap)
                return;

            isRefreshingFullBitmap = true;
            byte[] bitmap = null;
            using (var fileStream = await LoadImageAsync(this.PathToFullBitmap).ConfigureAwait(false))
            {
                if (fileStream != null)
                    bitmap = ReadFully(fileStream);
            }

            await Deployment.Current.Dispatcher.InvokeAsync(() =>
            {
                WriteableBitmap bi = null;

                if (bitmap != null)
                {
                    bi = PictureDecoder.DecodeJpeg(new MemoryStream(bitmap));
                    if (this.isPixelized)
                    {
                        Debug.WriteLine("Pixelise " + this.PathToFullBitmap);
                        bi = Pixelate(bi, this.FloutedAreas, PIXELISATION_FACTOR);
                    }
                }

                //return bi;
                this.fullImage = bi;
                NotifyPropertyChanged(m => m.FullImage);
                isRefreshingFullBitmap = false;
            }).ConfigureAwait(false);


        }

        private bool isRefreshingThumbBitmap = false;
        public async void RefreshThumbBitmap()
        {
            Debug.WriteLine("RefreshThumbBitmap of " + this.PathToFullBitmap);
            if (string.IsNullOrEmpty(PathToThumbBitmap) || isRefreshingThumbBitmap)
                return;

            //if (string.IsNullOrEmpty(this.PathToThumbBitmap))
            //{
            //    WriteableBitmap bi = PictureDecoder.DecodeJpeg(new MemoryStream(QuizzApp.Ressources.QuizzAppResource.noThumb));
            //    this.thumbImage = bi;
            //    NotifyPropertyChanged(m => m.ThumbImage);
            //    return;
            //}

            isRefreshingThumbBitmap = true;
            byte[] bitmap = null;
            using (var fileStream = await LoadImageAsync(this.PathToThumbBitmap).ConfigureAwait(false))
            {
                if (fileStream != null)
                    bitmap = ReadFully(fileStream);
            }



            await Deployment.Current.Dispatcher.InvokeAsync(() =>
            {
                WriteableBitmap bi = null;
                if (bitmap != null)
                {
                    bi = PictureDecoder.DecodeJpeg(new MemoryStream(bitmap));

                    if (this.isPixelized)
                    {
                        Debug.WriteLine("Pixelise " + this.PathToThumbBitmap);
                        bi = Pixelate(bi, this.FloutedAreas, PIXELISATION_FACTOR_THUMB);
                    }
                }

                //return bi;
                this.thumbImage = bi;
                NotifyPropertyChanged(m => m.ThumbImage);
                isRefreshingThumbBitmap = false;
            }).ConfigureAwait(false);
        }


        /*public async void RefreshThumbBitmap()
        {
            if (string.IsNullOrEmpty(PathToFullBitmap))
                return;

            byte[] bitmap = null;
            using (var fileStream = await LoadImageAsync(this.PathToThumbBitmap))
            {
                if (fileStream != null)
                    bitmap = ReadFully(fileStream);
            }


            WriteableBitmap bi = null;
            await Deployment.Current.Dispatcher.InvokeAsync(() =>
            {
                if (bitmap != null)
                {
                    bi = PictureDecoder.DecodeJpeg(new MemoryStream(bitmap));
                }
            });

            ThreadPool.QueueUserWorkItem(state =>
            {
                // Apply Effect on int[] since WriteableBitmap can't be used in background thread
                //var width = bitmap.PixelWidth;
                //var height = bitmap.PixelHeight;
                //var resultPixels = effect.Process(bitmap.Pixels, width, height);

                // Present result
                // WriteableBitmap ctor has to be invoked on the UI thread
                //dispatcher.BeginInvoke(() => ShowImage(resultPixels.ToWriteableBitmap(width, height)));

                int[] resultPixels = null;
                if (bi != null)
                {
                    if (this.isPixelized)
                    {
                        Debug.WriteLine("Pixelise " + this.PathToThumbBitmap);
                        GaussianBlurEffect effect = new GaussianBlurEffect();
                        resultPixels = effect.Process(bi.Pixels, bi.PixelWidth, bi.PixelHeight);
                        
                        //bi = Pixelate(bi, this.FloutedAreas, PIXELISATION_FACTOR_THUMB);
                    }
                }

                //return bi;
                //this.thumbImage = bi;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    bi = resultPixels.ToWriteableBitmap(bi.PixelWidth, bi.PixelHeight);
                    this.thumbImage = bi;
                    NotifyPropertyChanged(m => m.ThumbImage);
                });
                

            });

            /*await Deployment.Current.Dispatcher.InvokeAsync(() =>
            {
                WriteableBitmap bi = null;
                if (bitmap != null)
                {
                    bi = PictureDecoder.DecodeJpeg(new MemoryStream(bitmap));

                    if (this.isPixelized)
                    {
                        Debug.WriteLine("Pixelise " + this.PathToThumbBitmap);
                        bi = Pixelate(bi, this.FloutedAreas, PIXELISATION_FACTOR_THUMB);
                    }
                }

                //return bi;
                this.thumbImage = bi;
                NotifyPropertyChanged(m => m.ThumbImage);
            });
        }*/




        private Task<Stream> LoadImageAsync(string filename)
        {
            return Task.Factory.StartNew<Stream>(() =>
            {
                if (filename == null)
                {
                    throw new ArgumentException("one of parameters is null");
                }

                Stream stream = null;

                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {

                    lock (thumbLock)
                    {
                        if (isoStore.FileExists(filename))
                        {
                            stream = isoStore.OpenFile(filename, System.IO.FileMode.Open, FileAccess.Read);
                        }
                    }
                }
                return stream;
            });
        }


        private static WriteableBitmap Pixelate(WriteableBitmap image, List<Rect> floutedAreas, int pixelisationFactor)
        {
            //WriteableBitmap returnedBitmap = image;
            foreach (var aRect in floutedAreas)
            {
                //Point p1 = new Point((int)(floutedPercentPointA.X * image.PixelWidth), (int)(floutedPercentPointA.Y * image.PixelHeight));
                //Point p2 = new Point((int)(floutedPercentPointB.X * image.PixelWidth), (int)(floutedPercentPointB.Y * image.PixelHeight));

                Point p1 = new Point((int)(aRect.Left * image.PixelWidth), (int)(aRect.Top * image.PixelHeight));
                Point p2 = new Point((int)(aRect.Right * image.PixelWidth), (int)(aRect.Bottom * image.PixelHeight));
                Rect pixelsRectangle = new Rect(p1, p2);

                int floutedPixWidth = ((int)(pixelsRectangle.Width / pixelisationFactor));
                int floutedPixHeight = ((int)(pixelsRectangle.Width / pixelisationFactor));

                if (floutedPixWidth == 0)
                    floutedPixWidth = 1;

                if (floutedPixHeight == 0)
                    floutedPixHeight = 1;

                WriteableBitmap resizedBitmap = image.Crop(pixelsRectangle).Resize(floutedPixWidth, floutedPixHeight, WriteableBitmapExtensions.Interpolation.NearestNeighbor)
                    .Resize((int)pixelsRectangle.Width, (int)pixelsRectangle.Height, WriteableBitmapExtensions.Interpolation.NearestNeighbor);

                //resizedBitmap = resizedBitmap.Resize(floutedPixWidth, floutedPixHeight, WriteableBitmapExtensions.Interpolation.NearestNeighbor);
                //resizedBitmap = resizedBitmap.Resize((int)pixelsRectangle.Width, (int)pixelsRectangle.Height, WriteableBitmapExtensions.Interpolation.NearestNeighbor);

                image.Blit(pixelsRectangle, resizedBitmap, new Rect(0, 0, resizedBitmap.PixelWidth, resizedBitmap.PixelHeight));
            }
            
            
            return image;
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[8 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        public void UnloadBitmap()
        {
            this.fullImage = null;
            this.thumbImage = null;

            byte[] OneXOneImage = QuizzApp.Ressources.QuizzAppResource.oneXone;  
            try
            {
                this.fullImage = BitmapFactory.New(1,1).FromStream(new MemoryStream(OneXOneImage));
                this.thumbImage = BitmapFactory.New(1, 1).FromStream(new MemoryStream(OneXOneImage));
                this.NotifyPropertyChanged(m => m.FullImage);
                this.NotifyPropertyChanged(m => m.ThumbImage);
                this.fullImage = null;
                this.thumbImage = null;
            }
            catch (Exception e) {
                Debug.WriteLine("Exception");
            }
        }
    }
}
