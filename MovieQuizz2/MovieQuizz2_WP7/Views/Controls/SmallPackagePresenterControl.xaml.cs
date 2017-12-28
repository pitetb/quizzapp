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

namespace MovieQuizz2
{
    public partial class SmallPackagePresenterControl : UserControl
    {
        public SmallPackagePresenterControl()
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
            DependencyProperty.Register("Package", typeof(Pack), typeof(SmallPackagePresenterControl), new PropertyMetadata(null, OnPackagePropertyChanged));

        private static void OnPackagePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            SmallPackagePresenterControl myUserControl = dependencyObject as SmallPackagePresenterControl;
            myUserControl.OnCaptionPropertyChanged(e);
        }

        private void OnCaptionPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CurrentViewModel != null)
                this.CurrentViewModel.Package = (Pack)e.NewValue;
        }


        
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SmallPackagePresenterControl), new PropertyMetadata(false, OnIsSelectedPropertyChanged));

        private static void OnIsSelectedPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            SmallPackagePresenterControl myUserControl = dependencyObject as SmallPackagePresenterControl;
            myUserControl.OnIsSelectedChanged(e);
        }

        private void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CurrentViewModel != null)
            {
                if ((bool)e.NewValue == true && this.IsSelectedColor != null)
                {
                    this.CurrentViewModel.Color = this.IsSelectedColor;
                }
                else
                {
                    this.CurrentViewModel.RecalculatePackageColor(); ;
                }                
            }
        }

        

        public Color IsSelectedColor
        {
            get { return (Color)GetValue(IsSelectedColorProperty); }
            set { SetValue(IsSelectedColorProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedColorProperty =
            DependencyProperty.Register("IsSelectedColor", typeof(Color), typeof(SmallPackagePresenterControl), new PropertyMetadata(Color.FromArgb(255, 255, 126, 0), OnIsSelectedColorPropertyChanged));

        private static void OnIsSelectedColorPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            SmallPackagePresenterControl myUserControl = dependencyObject as SmallPackagePresenterControl;
            myUserControl.OnIsSelectedColorChanged(e);
        }

        private void OnIsSelectedColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsSelected)
            {
                this.CurrentViewModel.Color = ((Color)(e.NewValue));                
            }            
        }

                
        
    }
}
