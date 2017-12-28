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
                this.TextBlockLevel.Text = string.Empty;
            else
                this.TextBlockLevel.Text = e.NewValue.ToString();            
        }





        public string Title1
        {
            get { return (string)GetValue(Title1Property); }
            set { SetValue(Title1Property, value); }
        }

        // Using a DependencyProperty as the backing store for Title1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Title1Property =
            DependencyProperty.Register("Title1", typeof(string), typeof(MQLevelButton), new PropertyMetadata(null, OnTitle1PropertyChanged));

        private static void OnTitle1PropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQLevelButton myUserControl = dependencyObject as MQLevelButton;
            myUserControl.OnTitle1Changed(e);
        }

        private void OnTitle1Changed(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                this.TextBlockTitle1.Text = string.Empty;
            else
                this.TextBlockTitle1.Text = e.NewValue.ToString();        
        }



        public Brush Title1Brush
        {
            get { return (Brush)GetValue(Title1BrushProperty); }
            set { SetValue(Title1BrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title1Brush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Title1BrushProperty =
            DependencyProperty.Register("Title1Brush", typeof(Brush), typeof(MQLevelButton), new PropertyMetadata(null, OnTitle1BrushPropertyChanged));

        private static void OnTitle1BrushPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQLevelButton myUserControl = dependencyObject as MQLevelButton;
            myUserControl.OnTitle1BrushChanged(e);
        }

        private void OnTitle1BrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                this.TextBlockTitle1.Foreground = new SolidColorBrush(Colors.Black);
            else
                this.TextBlockTitle1.Foreground = e.NewValue as Brush;        
        }





        
        public string Title2
        {
            get { return (string)GetValue(Title2Property); }
            set { SetValue(Title2Property, value); }
        }

        // Using a DependencyProperty as the backing store for Title1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Title2Property =
            DependencyProperty.Register("Title2", typeof(string), typeof(MQLevelButton), new PropertyMetadata(null, OnTitle2PropertyChanged));

        private static void OnTitle2PropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQLevelButton myUserControl = dependencyObject as MQLevelButton;
            myUserControl.OnTitle2Changed(e);
        }

        private void OnTitle2Changed(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                this.TextBlockTitle2.Text = string.Empty;
            else
                this.TextBlockTitle2.Text = e.NewValue.ToString();        
        }


        public Brush Title2Brush
        {
            get { return (Brush)GetValue(Title2BrushProperty); }
            set { SetValue(Title2BrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title2Brush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Title2BrushProperty =
            DependencyProperty.Register("Title2Brush", typeof(Brush), typeof(MQLevelButton), new PropertyMetadata(null, OnTitle2BrushPropertyChanged));

        private static void OnTitle2BrushPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQLevelButton myUserControl = dependencyObject as MQLevelButton;
            myUserControl.OnTitle2BrushChanged(e);
        }

        private void OnTitle2BrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                this.TextBlockTitle2.Foreground = new SolidColorBrush(Colors.Black);
            else
                this.TextBlockTitle2.Foreground = e.NewValue as Brush;
        }




        public string Title3
        {
            get { return (string)GetValue(Title3Property); }
            set { SetValue(Title3Property, value); }
        }

        // Using a DependencyProperty as the backing store for Title1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Title3Property =
            DependencyProperty.Register("Title3", typeof(string), typeof(MQLevelButton), new PropertyMetadata(null, OnTitle3PropertyChanged));

        private static void OnTitle3PropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQLevelButton myUserControl = dependencyObject as MQLevelButton;
            myUserControl.OnTitle3Changed(e);
        }

        private void OnTitle3Changed(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                this.TextBlockTitle3.Text = string.Empty;
            else
                this.TextBlockTitle3.Text = e.NewValue.ToString();    
        }


        public Brush Title3Brush
        {
            get { return (Brush)GetValue(Title3BrushProperty); }
            set { SetValue(Title3BrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title2Brush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Title3BrushProperty =
            DependencyProperty.Register("Title3Brush", typeof(Brush), typeof(MQLevelButton), new PropertyMetadata(null, OnTitle3BrushPropertyChanged));

        private static void OnTitle3BrushPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQLevelButton myUserControl = dependencyObject as MQLevelButton;
            myUserControl.OnTitle3BrushChanged(e);
        }

        private void OnTitle3BrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                this.TextBlockTitle3.Foreground = new SolidColorBrush(Colors.Black);
            else
                this.TextBlockTitle3.Foreground = e.NewValue as Brush;
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
