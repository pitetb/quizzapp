using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace QuizzApp.WP.Controls
{
    public partial class ImagesButton : UserControl
    {

        public ImagesButton()
        {
            InitializeComponent();
            this.SetToNormalState();
            
        }

        #region control properties
        public BitmapImage BitmapImage
        {
            get { return (BitmapImage)GetValue(BitmapImageProperty); }
            set { SetValue(BitmapImageProperty, value); }
        }

        public static readonly DependencyProperty BitmapImageProperty =
            DependencyProperty.Register("BitmapImage", typeof(BitmapImage), typeof(ImagesButton), new PropertyMetadata(null, OnBitmapImagePropertyChanged));

        private static void OnBitmapImagePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ImagesButton myUserControl = dependencyObject as ImagesButton;
            myUserControl.OnBitmapImagePropertyChanged(e);
        }

        private void OnBitmapImagePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBitmap();
        }


        public BitmapImage BitmapImageOnClick
        {
            get { return (BitmapImage)GetValue(BitmapImageOnClickProperty); }
            set { SetValue(BitmapImageOnClickProperty, value); }
        }

        public static readonly DependencyProperty BitmapImageOnClickProperty =
            DependencyProperty.Register("BitmapImageOnClick", typeof(BitmapImage), typeof(ImagesButton), new PropertyMetadata(null, OnBitmapImageOnClickPropertyChanged));

        private static void OnBitmapImageOnClickPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ImagesButton myUserControl = dependencyObject as ImagesButton;
            myUserControl.OnBitmapImageOnClickPropertyChanged(e);
        }

        private void OnBitmapImageOnClickPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBitmap();
        }

        
        public delegate void ImageClickedEventHandler(object sender, RoutedEventArgs e);

        public event ImageClickedEventHandler ImageClick;

        private void RaiseImageClick()
        {
            if (ImageClick != null)
                this.ImageClick(this, new RoutedEventArgs());
        }





        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(ImagesButton), new PropertyMetadata(string.Empty, OnButtonTextPropertyChanged));

        private static void OnButtonTextPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            ImagesButton myUserControl = dependencyObject as ImagesButton;
            if (e.NewValue == null)
                myUserControl.ButtonTextTextBlock.Text = string.Empty;
            else
                myUserControl.ButtonTextTextBlock.Text = e.NewValue.ToString();
        }

        
        
        #endregion


        private bool isOnClick = false;

        private void ImageView_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.isOnClick = true;
            RefreshBitmap();
        }


        private void ImageView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {            
            this.isOnClick = true;
            RefreshBitmap();
        }

        private void ImageView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("ImageView_MouseLeftButtonUp");
            //bool raiseEvent = this.isOnClick;
            
            this.isOnClick = false;
            RefreshBitmap();
        }


        private void ImageView_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // Raise event "on click"
            this.RaiseImageClick(); 
        }

        private void ImageView_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Debug.WriteLine("ImageView_MouseLeave");
            if (this.isOnClick)
                SetToNormalState();            
        }

        private void SetToClickedState()
        {
            this.ImageView.Source = this.BitmapImageOnClick; 
        }

        private void SetToNormalState()
        {
            this.ImageView.Source = this.BitmapImage; 
        }

        private void RefreshBitmap()
        {
            if (this.isOnClick)
                this.SetToClickedState();
            else
                this.SetToNormalState();
        }

        
    }
}
