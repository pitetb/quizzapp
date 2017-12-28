using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Diagnostics;
using System.ComponentModel;
using QuizzApp.WP.ViewModels.MQKeyboard;

namespace TvShowQuizz.Views.Controls.MQKeyboard
{
    public partial class KeyboardLetterControl : UserControl
    {
        public KeyboardLetterControl()
        {
            InitializeComponent();
            this.CurrentViewModel.PropertyChanged += new PropertyChangedEventHandler(CurrentViewModel_PropertyChanged);
            this.CurrentViewModel.ReplaceLetterAnimation += CurrentViewModel_ReplaceLetterAnimation;
            this.CurrentViewModel.LaunchBlink += CurrentViewModel_LaunchBlink;
        }

        private void CurrentViewModel_LaunchBlink(object sender, EventArgs e)
        {
            this.MakeBlinkAnimation.Begin();
        }

        private void CurrentViewModel_ReplaceLetterAnimation(object sender, SimpleMvvmToolkit.NotificationEventArgs e)
        {
            this.BeginReplaceCharAnimation();
        }

        private void CurrentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible")
            {
                if (this.CurrentViewModel.IsVisible)
                    this.Show();
                else
                    this.Hide();
            }            
        }

        public KeyboardLetterControlViewModel CurrentViewModel
        {
            get
            {
                return this.mainGrid.DataContext as KeyboardLetterControlViewModel;
            }
        }


        
        //private void grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        private void grid_Tap(object sender, RoutedEventArgs e)
        {
            if (this.CurrentViewModel == null)
                return;

            this.CurrentViewModel.OnLetterClicked();
            
            // For debug purpose
            //this.HideAndThenReplaceChar(GetRandomLetter());
            //e.Handled = true;
        }

        
        private void Hide()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;            
        }

        private void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;
        }
        
		private void BeginReplaceCharAnimation()
        {            
            //this.Visibility = System.Windows.Visibility.Collapsed;
            //this.CurrentViewModel.Letter = newChar;
            //this.Letter = newChar;

            // Make animation
            this.ShowAfterLetterChangedAnimation.SpeedRatio = 2;
            this.ShowAfterLetterChangedAnimation.Begin();
            //this.Visibility = System.Windows.Visibility.Visible;
        }

        public char Letter
        {
            get { return (char)GetValue(LetterProperty); }
            set { SetValue(LetterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Letter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LetterProperty =
            DependencyProperty.Register("Letter", typeof(char), typeof(KeyboardLetterControl), new PropertyMetadata(LetterChanged));


        private static void LetterChanged(DependencyObject obj,
                        DependencyPropertyChangedEventArgs args)
        {
            KeyboardLetterControl control = obj as KeyboardLetterControl;
            if (control != null)
                control.OnLetterChanged(args);
        }

        private void OnLetterChanged(DependencyPropertyChangedEventArgs args)
        {
            //if (args.NewValue != null)
            //    this.textBlock.Text = args.NewValue.ToString().First().ToString();
            //else
            //    this.textBlock.Text = string.Empty;

            if (args.NewValue != null)
                this.CurrentViewModel.ReplaceLetter((char) args.NewValue, false) ;
            else
                this.CurrentViewModel.ReplaceLetter(' ', false);            
        }


    }
}
