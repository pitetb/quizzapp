using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace QuizzApp.Core.Helpers.Collections
{  
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private AsyncOperation asyncOp = null;
                
        public AsyncObservableCollection()
        {
            CreateAsyncOp();
        }

        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        {
            CreateAsyncOp();
        }

        private void CreateAsyncOp()
        {
            // Create the AsyncOperation to post events on the creator thread
            asyncOp = AsyncOperationManager.CreateOperation(null);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {            
            // Post the CollectionChanged event on the creator thread
            asyncOp.Post(RaiseCollectionChanged, e);
        }

        private void RaiseCollectionChanged(object param)
        {
            if (_suppressNotification)
                return;

            // We are in the creator thread, call the base implementation directly
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {            
            // Post the PropertyChanged event on the creator thread
            asyncOp.Post(RaisePropertyChanged, e);
        }

        private void RaisePropertyChanged(object param)
        {
            if (_suppressNotification)
                return;

            // We are in the creator thread, call the base implementation directly
            base.OnPropertyChanged((PropertyChangedEventArgs)param);
        }



        private bool _suppressNotification = false;


        /// <summary>
        /// Adds a collection suppressing notification per item and just raising a notification
        /// for the whole collection
        /// </summary>
        /// <param name="list"></param>
        public void AddRange(IEnumerable<T> list)
        {
            if (list == null) throw new ArgumentNullException("list");
            _suppressNotification = true;
            foreach (T item in list)
            {
                Add(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }
    }



    


   
}
