using QuizzApp.Core.Controllers;
using QuizzApp.Tombstone;
using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;

namespace QuizzApp.WP.ViewModels
{
    public abstract class QAViewModelBase<T> : ViewModelBaseWithDesign<T>, IPersistent
    {
        

        protected GameProvider GameProvider { get; set; }

        protected QAViewModelBase(GameProvider gameProvider)
        {
            this.GameProvider = gameProvider;

            // Register View Model with PersistenceManager
            PersistenceManager.Instance.Register(this.GetType().Name, this);
        }

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;
        public event EventHandler<NotificationEventArgs> LongLoadingStartEvent;
        public event EventHandler<NotificationEventArgs> LongLoadingStopEvent;
        public event EventHandler<NotificationEventArgs<string>> AlertNotice;

        public virtual void NotifyError(string message, Exception error)
        {
            // Notify view of an error
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        public virtual void NotifyLongLoadingStart()
        {
            Notify(LongLoadingStartEvent, new NotificationEventArgs());
        }

        public virtual void NotifyLongLoadingStop()
        {
            Notify(LongLoadingStopEvent, new NotificationEventArgs());
        }

        public virtual void NotifyAlertNotice(string title, string message)
        {
            // Notify view of an error
            Notify(AlertNotice, new NotificationEventArgs<string>(title, message));
        }

        public virtual void CleanViewModel()
        {
        }

        private bool isLongLoadingInProgress;
        public bool IsLongLoadingInProgress
        {
            get { return isLongLoadingInProgress; }
            private set
            {
                isLongLoadingInProgress = value;
                // Fire PropertyChanged event
                NotifyPropertyChanged(m => this.IsLongLoadingInProgress);
                NotifyPropertyChanged(m => this.IsLongLoadingInProgressVisibility);
                
                /*BindingHelper.NotifyPropertyChanged(this, m => m.IsLongLoadingInProgress, base.propertyChanged);
                BindingHelper.NotifyPropertyChanged(this, m => m.IsLongLoadingInProgressVisibility, base.propertyChanged);  */              
            }
        }
                

        public Visibility IsLongLoadingInProgressVisibility
        {
            get 
            { 
                return IsLongLoadingInProgress ? Visibility.Visible : Visibility.Collapsed; 
            }
            
        }
               
        
        /// <summary>
        /// Occurs after a class has been instantiated for the first time
        /// </summary>
        public virtual void OnLaunched()
        {

        }

        /// <summary>
        /// Occurs after an instance has been rehydrated after an activation
        /// </summary>
        public virtual void OnActivated(bool preserved)
        {

        }

        /// <summary>
        /// Occurs before an instance is dehydrated the tombstoned
        /// </summary>
        public virtual void OnDeactivated()
        {

        }

        /// <summary>
        /// Occurs when the application is closing
        /// </summary>
        public virtual void OnClosing()
        {

        }



    }
}
