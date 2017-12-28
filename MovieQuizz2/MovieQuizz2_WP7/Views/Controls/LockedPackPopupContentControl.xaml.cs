using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Documents;

namespace MovieQuizz2
{
    public partial class LockedPackPopupContentControl : UserControl
    {
        public LockedPackPopupContentControl()
        {
            InitializeComponent();
        }


        public string PackageName
        {
            get { return (string)GetValue(PackageNameProperty); }
            set { SetValue(PackageNameProperty, value); }
        }

        public static readonly DependencyProperty PackageNameProperty =
            DependencyProperty.Register("PackageName", typeof(string), typeof(LockedPackPopupContentControl), new PropertyMetadata(null, OnPackageNamePropertyChanged));

        private static void OnPackageNamePropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            LockedPackPopupContentControl myUserControl = dependencyObject as LockedPackPopupContentControl;
            myUserControl.OnPackageNameChanged(e);
        }

        private void OnPackageNameChanged(DependencyPropertyChangedEventArgs e)
        {
            this.packNameTextBlock.Text = this.packNameTextBlock.Text.Replace("#packname", e.NewValue.ToString());
            //this.mainText.
            //this.packNameTextBlock.Text = e.NewValue.ToString();
        }
    }
}
