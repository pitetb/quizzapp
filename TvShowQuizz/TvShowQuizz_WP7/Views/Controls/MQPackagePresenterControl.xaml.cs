using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.Windows.Media;
using QuizzApp.Core.Entities;
using QuizzApp.WP.ViewModels;

namespace TvShowQuizz
{
    public partial class MQPackagePresenterControl : UserControl
    {
        public MQPackagePresenterControl()
        {
            InitializeComponent();
            
        }

        public PackagePresenterViewModel CurrentViewModel
        {
            get
            {
                if (this.LayoutRoot.DataContext is PackagePresenterViewModel)
                    return (PackagePresenterViewModel)this.LayoutRoot.DataContext;
                else
                    return null;
            }
        }

        
        public Pack Package
        {
            get { return (Pack)GetValue(PackageProperty); }
            set { SetValue(PackageProperty, value); }
        }
        
        // Using a DependencyProperty as the backing store for Poster1.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PackageProperty =
            DependencyProperty.Register("Package", typeof(Pack), typeof(MQPackagePresenterControl), new PropertyMetadata(null, OnPackagePropertyChanged));

        private static void OnPackagePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            MQPackagePresenterControl myUserControl = dependencyObject as MQPackagePresenterControl;
            myUserControl.OnCaptionPropertyChanged(e);
        }

        private void OnCaptionPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CurrentViewModel != null && e.NewValue is Pack)
                this.CurrentViewModel.Package = (Pack)e.NewValue;
        }




        public delegate void ButtonClickEventHandler(object sender, EventArgs e);
        public event ButtonClickEventHandler ButtonClicked;

        private void NotifyButtonClicked(EventArgs e)
        {
            if (this.ButtonClicked != null)
                ButtonClicked(this, e);
        }


        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Pressed", true);
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        private void Grid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.NotifyButtonClicked(new EventArgs());
        }
        
    }
}
