using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Media;
using System.ComponentModel;
using SimpleMvvmToolkit;
using QuizzApp.WP.ViewModels.MQKeyboard;

namespace TvShowQuizz.Views.Controls.MQKeyboard
{
    public partial class KeyboardControl : UserControl
    {
        private List<KeyboardLetterControl> letterControls = new List<KeyboardLetterControl>();

        public KeyboardControl()
        {
            InitializeComponent();

            // Keep references on all keys
            letterControls.Add(this.L1);
            letterControls.Add(this.L2);
            letterControls.Add(this.L3);
            letterControls.Add(this.L4);
            letterControls.Add(this.L5);
            letterControls.Add(this.L6);
            letterControls.Add(this.L7);
            letterControls.Add(this.L8);
            letterControls.Add(this.L9);
            letterControls.Add(this.L10);
            letterControls.Add(this.L11);
            letterControls.Add(this.L12);
            letterControls.Add(this.L13);
            letterControls.Add(this.L14);

            
            // Inject Letter controls VM to KeyboardControlVM
            this.CurrentViewModel.KeysViewModels = letterControls.Select(s => s.CurrentViewModel).ToList();
            this.CurrentViewModel.PropertyChanged += CurrentViewModel_PropertyChanged;
            this.CurrentViewModel.GoodAnswer += CurrentViewModel_GoodAnswer;
            this.CurrentViewModel.BadWord += CurrentViewModel_BadWord;
        }

        private void CurrentViewModel_BadWord(object sender, NotificationEventArgs e)
        {
            this.NotifyBadWord();
        }

        private void CurrentViewModel_GoodAnswer(object sender, NotificationEventArgs e)
        {
            this.NotifyGoodAnswer();
        }

        private void CurrentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsTitleFound"))
            {
                if (this.CurrentViewModel.IsTitleFound)
                    this.KeyboardVisibility = System.Windows.Visibility.Collapsed;
                else if (this.KeyboardVisibility == System.Windows.Visibility.Collapsed)
                    this.KeyboardVisibility = System.Windows.Visibility.Visible;                
            }
        }

        public void InitializeState(string title, bool isComplete, int nbSecondsForHelp)
        {
            if (this.CurrentViewModel == null)
                return;

            this.CurrentViewModel.InitializeTitle(title, isComplete, nbSecondsForHelp);
        }

        public KeyboardControlViewModel CurrentViewModel
        {
            get
            {
                return this.LayoutRoot.DataContext as KeyboardControlViewModel;
            }
        }

        
        public Visibility KeyboardVisibility
        {
            get { return (Visibility)GetValue(KeyboardVisibilityProperty); }
            set { SetValue(KeyboardVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyboardVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyboardVisibilityProperty =
            DependencyProperty.Register("KeyboardVisibility", typeof(Visibility), typeof(KeyboardControl), new PropertyMetadata(Visibility.Visible,
                new PropertyChangedCallback(KeyboardVisibilityChanged)));

        
        private static void KeyboardVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KeyboardControl control = d as KeyboardControl;
            if (control == null)
                return;

            control.OnKeyboardVisibilityChanged(e);
        }

        private void OnKeyboardVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;
            if (((System.Windows.Visibility)e.NewValue) == Visibility.Collapsed)
            {
                this.HideKeyboard.Begin();
            }
            else
                this.ShowKeyboard.Begin();
        }
        
        private void TitleGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("TitleGrid_MouseLeftButtonDown");
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), this.keysGrid);
            if (!elements.Contains(this.keysGrid))
            {
                if (this.CurrentViewModel.IsTitleFound)
                    this.KeyboardVisibility = System.Windows.Visibility.Collapsed;
                else
                    this.SwitchKeyboardVisibility();
            }
        }

        public void SwitchKeyboardVisibility()
        {            
            if (this.KeyboardVisibility == System.Windows.Visibility.Visible || this.CurrentViewModel.IsTitleFound)
                this.KeyboardVisibility = System.Windows.Visibility.Collapsed;
            else
                this.KeyboardVisibility = System.Windows.Visibility.Visible;
        }





        //public string Title
        //{
        //    get { return (string)GetValue(TitleProperty); }
        //    set { SetValue(TitleProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TitleProperty =
        //    DependencyProperty.Register("Title", typeof(string), typeof(KeyboardControl), new PropertyMetadata(TitleChanged));

        //private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    KeyboardControl control = d as KeyboardControl;
        //    if (control == null)
        //        return;
        //    control.OnTitleChanged(e);      
        //}

        //private void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    this.CurrentViewModel.SetTitle(e.NewValue as string);
        //}



        public Brush TitleAreaBackgroundBrush
        {
            get { return (Brush)GetValue(TitleAreaBackgroundBrushProperty); }
            set { SetValue(TitleAreaBackgroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultBackgroundLetterBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleAreaBackgroundBrushProperty =
            DependencyProperty.Register("TitleAreaBackgroundBrush", typeof(Brush), typeof(KeyboardControl), new PropertyMetadata(TitleAreaBackgroundBrushChanged));


        private static void TitleAreaBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KeyboardControl control = d as KeyboardControl;
            if (control == null)
                return;
            control.OnTitleAreaBackgroundBrushChanged(e);
        }

        private void OnTitleAreaBackgroundBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CurrentViewModel.TitleAreaBackgroundBrush = e.NewValue as Brush;
        }


        public Brush DefaultBackgroundLetterBrush
        {
            get { return (Brush)GetValue(DefaultBackgroundLetterBrushProperty); }
            set { SetValue(DefaultBackgroundLetterBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultBackgroundLetterBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultBackgroundLetterBrushProperty =
            DependencyProperty.Register("DefaultBackgroundLetterBrush", typeof(Brush), typeof(KeyboardControl), new PropertyMetadata(DefaultBackgroundLetterBrushChanged));


        private static void DefaultBackgroundLetterBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KeyboardControl control = d as KeyboardControl;
            if (control == null)
                return;
            control.OnDefaultBackgroundLetterBrushChanged(e);
        }

        private void OnDefaultBackgroundLetterBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CurrentViewModel.DefaultBackgroundLetterBrush = e.NewValue as Brush;
        }



        public Brush GoodLetterBackgroundBrush
        {
            get { return (Brush)GetValue(GoodLetterBackgroundBrushProperty); }
            set { SetValue(GoodLetterBackgroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultBackgroundLetterBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GoodLetterBackgroundBrushProperty =
            DependencyProperty.Register("GoodLetterBackgroundBrush", typeof(Brush), typeof(KeyboardControl), new PropertyMetadata(GoodLetterBackgroundBrushChanged));


        private static void GoodLetterBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KeyboardControl control = d as KeyboardControl;
            if (control == null)
                return;
            control.OnGoodLetterBackgroundBrushChanged(e);
        }

        private void OnGoodLetterBackgroundBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CurrentViewModel.GoodLetterBackgroundColorBrush = e.NewValue as Brush;
        }


        public Brush BadLetterBackgroundBrush
        {
            get { return (Brush)GetValue(BadLetterBackgroundBrushProperty); }
            set { SetValue(BadLetterBackgroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultBackgroundLetterBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BadLetterBackgroundBrushProperty =
            DependencyProperty.Register("BadLetterBackgroundBrush", typeof(Brush), typeof(KeyboardControl), new PropertyMetadata(BadLetterBackgroundBrushChanged));


        private static void BadLetterBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KeyboardControl control = d as KeyboardControl;
            if (control == null)
                return;
            control.OnBadLetterBackgroundBrushChanged(e);
        }

        private void OnBadLetterBackgroundBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CurrentViewModel.BadLetterBackgroundColorBrush = e.NewValue as Brush;
        }


        private void ImagesDeleteButton_ImageClick(object sender, RoutedEventArgs e)
        {
            if (this.CurrentViewModel == null)
                return;
            this.CurrentViewModel.DeletePreviousLetter();
        }


        private void ImagesHelpButton_ImageClick(object sender, RoutedEventArgs e)
        {
            if (this.CurrentViewModel == null)
                return;

            if (this.CurrentViewModel.IsHelpAvailable())
                this.CurrentViewModel.MakeBlinkHelpLetters();
            else
                this.NotifHelpNotAvailableYet(this.CurrentViewModel.NextHelpDateAvailability);
        }




        public bool IsFound
        {
            get 
            { 
                return this.CurrentViewModel.IsTitleFound; 
            }            
        }

        //// Using a DependencyProperty as the backing store for IsFound.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsFoundProperty =
        //    DependencyProperty.Register("IsFound", typeof(bool), typeof(KeyboardControl), new PropertyMetadata(false, IsFoundChanged));

        //private static void IsFoundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    KeyboardControl control = d as KeyboardControl;
        //    if (control == null)
        //        return;
        //    control.IsFoundChanged(e);
        //}

        //private void IsFoundChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    this.CurrentViewModel.IsTitleFound = (bool)e.NewValue;
        //    this.NotifyIsTitleFoundChanged();
        //}

        //public void SetTitle(string title)
        //{
        //    this.CurrentViewModel.SetTitle(title);
        //}



        public event EventHandler<NotificationEventArgs> GoodAnswer;
        private void NotifyGoodAnswer()
        {
            if (GoodAnswer != null)
                GoodAnswer(this, new NotificationEventArgs());
        }


        public event EventHandler<NotificationEventArgs<DateTime>> HelpNotAvailableYet;
        private void NotifHelpNotAvailableYet(DateTime nextTimeAvailability)
        {
            if (HelpNotAvailableYet != null)
                HelpNotAvailableYet(this, new NotificationEventArgs<DateTime>(string.Empty, nextTimeAvailability));
        }


        public event EventHandler<NotificationEventArgs> BadWord;
        private void NotifyBadWord()
        {
            if (BadWord != null)
                BadWord(this, new NotificationEventArgs());
        }
    }
}
