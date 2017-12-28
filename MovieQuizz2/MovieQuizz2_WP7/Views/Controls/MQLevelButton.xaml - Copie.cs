using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using QuizzApp.Core.Helpers;
using System.Diagnostics;

namespace MovieQuizz2
{
    public partial class MQLevelButton : UserControl
    {
        public MQLevelButton()
        {
            InitializeComponent();
            this.Label = "1";
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(MQLevelButton), new PropertyMetadata(null, OnLabelNamePropertyChanged));

        private static void OnLabelNamePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQLevelButton myUserControl = dependencyObject as MQLevelButton;
            myUserControl.OnLabelChanged(e);
        }

        private void OnLabelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                this.TextBlock1.Text = string.Empty;
            else
                this.TextBlock1.Text = e.NewValue.ToString();            
        }


        private void userControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //this.aButton.Height = e.NewSize.Width + 5;
        }









        public bool IsLocal
        {
            get { return (bool)GetValue(IsLocalProperty); }
            set { SetValue(IsLocalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLocal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLocalProperty =
            DependencyProperty.Register("IsLocal", typeof(bool), typeof(MQLevelButton), new PropertyMetadata(true, OnIsLocalPropertyChanged));

        private static void OnIsLocalPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQLevelButton myUserControl = dependencyObject as MQLevelButton;
            myUserControl.OnIsLocalChanged(e);
        }

        private void OnIsLocalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (((bool)e.NewValue))
            {
                this.Opacity = 1;
                //this.ProgressRectangle.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.Opacity = 0.5d;
                //this.ProgressRectangle.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.CalculateProgressDisplay();
        }




        public Color ProgressColor
        {
            get { return (Color)GetValue(ProgressColorProperty); }
            set { SetValue(ProgressColorProperty, value); }
        }

        public static readonly DependencyProperty ProgressColorProperty =
            DependencyProperty.Register("ProgressColor", typeof(Color), typeof(MQLevelButton), new PropertyMetadata(OnProgressColorPropertyChanged));

        private static void OnProgressColorPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQLevelButton myUserControl = dependencyObject as MQLevelButton;
            myUserControl.OnProgressColorChanged(e);
        }

        private void OnProgressColorChanged(DependencyPropertyChangedEventArgs e)
        {
            //var elem = (FrameworkElement)this.ProgressRectangle;
            //var myProgressBorder = VisualTreeHelpers.FindName<GradientStop>("Gradient1", elem);

            var myProgressBorder = this.SliderPercentProgress;
            if (myProgressBorder == null || e.NewValue == null)
                return;

            myProgressBorder.Background = new SolidColorBrush(((Color)e.NewValue));
            //myProgressBorder.BorderBrush = new SolidColorBrush(((Color)e.NewValue));
            //myProgressBorder.BorderBrush.Opacity = 0.5;

        }





        public double ProgressPercent
        {
            get { return (double)GetValue(ProgressPercentProperty); }
            set { SetValue(ProgressPercentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressPercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressPercentProperty =
            DependencyProperty.Register("ProgressPercent", typeof(double), typeof(MQLevelButton), new PropertyMetadata(0.3d, OnProgressPercentPropertyChanged));

        private static void OnProgressPercentPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQLevelButton myUserControl = dependencyObject as MQLevelButton;
            myUserControl.OnProgressPercentChanged(e);
        }

        private void OnProgressPercentChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CalculateProgressDisplay();
        }


        private void CalculateProgressDisplay()
        {
            var myProgressBar = this.SliderPercentProgress;

            if (myProgressBar == null)
                return;

            double progressPercent = ProgressPercent;
            if (progressPercent > 1)
                progressPercent = 1d;
            if (progressPercent < 0)
                progressPercent = 0d;
            myProgressBar.Value = progressPercent;
            

            //var myButtonBck = this.ButtonBackground;
            //var myProgressBorder = this.ProgressGridToMarge;
            //var myProgressBar = this.ProgressBar;

            //if (myButtonBck == null || myProgressBorder == null || myProgressBar == null)
            //    return;

            //double progressPercent = ProgressPercent;
            //if (progressPercent > 1)
            //    progressPercent = 1d;
            //if (progressPercent < 0)
            //    progressPercent = 0d;

            //var currentWidth = myButtonBck.ActualWidth;
            //double percentWidth = currentWidth * progressPercent;
            //double leftMargin = currentWidth - percentWidth;

            //if (leftMargin < myProgressBar.CornerRadius.BottomLeft)
            //{
            //    myProgressBar.CornerRadius = new CornerRadius(myProgressBar.CornerRadius.TopLeft, myProgressBar.CornerRadius.BottomLeft, myProgressBar.CornerRadius.BottomLeft, myProgressBar.CornerRadius.BottomLeft);
            //}
            //else
            //{
            //    myProgressBar.CornerRadius = new CornerRadius(myProgressBar.CornerRadius.TopLeft, 0, 0, myProgressBar.CornerRadius.BottomLeft);
            //}

            //myProgressBorder.Margin = new Thickness(0, 0, leftMargin, 0);
        }

        public void Test()
        {
            
        }


        public delegate void ButtonClickEventHandler(object sender, EventArgs e);
        public event ButtonClickEventHandler ButtonClicked;

        private void NotifyButtonClicked(EventArgs e)
        {
            if (this.ButtonClicked != null)
                ButtonClicked(this, e);
        }

        //private bool movedPressed = false;
        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Debug.WriteLine("Grid_MouseEnter");
            VisualStateManager.GoToState(this, "Pressed", true);            
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Debug.WriteLine("Grid_MouseLeave");
            VisualStateManager.GoToState(this, "Normal", true);        
        }

        private void Grid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Debug.WriteLine("Grid_Tap");
            this.NotifyButtonClicked(new EventArgs());
        }

        

    }
}
