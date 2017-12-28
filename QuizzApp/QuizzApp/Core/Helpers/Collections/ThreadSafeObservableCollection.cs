using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace QuizzApp.Core.Helpers.Collections
{

    

    /// <summary>
    /// Generic interface for observable collection
    /// </summary>
    /// <typeparam name="T">The type being used for binding.
    /// </typeparam>
    public interface IThreadSafeObservableCollection<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Adds a range of items to the observable collection.
        /// </summary>
        /// <param name="items">
        /// The items to be added to the observable collection.
        /// </param>
        void AddRange(IEnumerable<T> items);
    }


    /// <summary>
    /// Thread safe observable collection
    /// </summary>
    /// <typeparam name="T">The type being used for binding.
    /// </typeparam>
    public class ThreadSafeObservableCollection<T> : BaseCollectionModel, IThreadSafeObservableCollection<T>
    {
        /// <summary>
        /// The internal collection.
        /// </summary>
        protected readonly List<T> collection = new List<T>();

        /// <summary>
        /// Used for thread synchronisation purposes.
        /// </summary>
        protected readonly object sync = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollection{T}"/> class.
        /// </summary>
        public ThreadSafeObservableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="items">
        /// Initializes a new instance with containing the items.
        /// </param>
        public ThreadSafeObservableCollection(IEnumerable<T> items)
        {
            this.AddRange(items);
        }

        #region IObservableCollection<T> Members

        /// <summary>
        /// Adds an item to the collection, raises a RaiseCollectionChanged event with a NotifyCollectionChangedAction.Add 
        /// parameter.
        /// </summary>
        /// <param name="item">
        /// The item being added.
        /// </param>
        public void Add(T item)
        {
            this.AddImpl(item);
        }

        /// <summary>
        /// Adds an item to the collection, raises a RaiseCollectionChanged event with a NotifyCollectionChangedAction.Add 
        /// parameter.
        /// </summary>
        /// <param name="item">
        /// The item being added.
        /// </param>
        int IList.Add(object item)
        {
            return this.AddImpl((T)item);
        }

        /// <summary>
        /// Adds a range of items to the collection, internally calls the Add(T item) method.
        /// </summary>
        /// <param name="items">The items being added.
        /// The items.
        /// </param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }

            items.ToList().ForEach(this.Add);
        }

        /// <summary>
        /// Clears the contents of the collection, raises a RaiseCollectionChanged event with a NotifyCollectionChangedAction.Reset 
        /// </summary>
        public void Clear()
        {
            lock (this.sync)
            {
                this.collection.Clear();
                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
            }

            
        }

        /// <summary>
        /// Determines if the collection contains an item.
        /// </summary>
        /// <param name="item">
        /// The item to locate in the collection. 
        /// </param>
        /// <returns>
        /// Returns true if the collection contains the item.
        /// </returns>
        public bool Contains(T item)
        {
            return this.collection.Contains(item);
        }

        /// <summary>
        /// Determines if the collection contains an item.
        /// </summary>
        /// <param name="item">
        /// The item to locate in the collection. 
        /// </param>
        /// <returns>
        /// Returns true if the collection contains the item.
        /// </returns>
        bool IList.Contains(object item)
        {
            return Contains((T)item);
        }

        /// <summary>
        /// Copy the collection to an array.
        /// </summary>
        /// <param name="array">
        /// he one-dimensional Array that is the destination of the items copied from IObservableCollection. The Array must have zero-based indexing. 
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins. 
        /// </param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.sync)
            {
                this.collection.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// Copy the collection to an array.
        /// </summary>
        /// <param name="array">
        /// he one-dimensional Array that is the destination of the items copied from IObservableCollection. The Array must have zero-based indexing. 
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins. 
        /// </param>
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            this.CopyTo(array.Cast<T>().ToArray(), arrayIndex);
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return this.collection.Count; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public object SyncRoot
        {
            get { return this.sync; }
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the collection read only status, false is always returned.
        /// </summary>
        bool IList.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the collection read only status, false is always returned.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the IList has a fixed size.
        /// </summary>
        bool IList.IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Removes an item from the collection. If the item does not exist in the collection the false is returned.
        /// </summary>
        /// <param name="item">
        /// The item to be removed from the collection
        /// </param>
        /// <returns>
        /// Returns the result of removing an item from the collection, if the item existed it returns true else false.
        /// </returns>
        public bool Remove(T item)
        {
            int index;
            bool result;

            lock (this.sync)
            {
                index = this.collection.IndexOf(item);
                if (index == -1)
                {
                    return false;
                }

                result = this.collection.Remove(item);

                if (result)
                {
                    this.RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
                }
            }

            

            return result;
        }

        /// <summary>
        /// Removes an item from the collection. If the item does not exist in the collection the false is returned.
        /// </summary>
        /// <param name="item">
        /// The item to be removed from the collection
        /// </param>
        /// <returns>
        /// Returns the result of removing an item from the collection, if the item existed it returns true else false.
        /// </returns>
        void IList.Remove(object item)
        {
            this.Remove((T)item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// Returns an enumerator for the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (this.sync)
            {
                var tmp = new T[this.collection.Count];
                this.collection.CopyTo(tmp, 0);

                return tmp.ToList().GetEnumerator();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// Returns an enumerator for the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this.sync)
            {
                var tmp = new T[this.collection.Count];
                this.collection.CopyTo(tmp, 0);

                return tmp.GetEnumerator();
            }
        }

        /// <summary>
        /// Returns the index of an item in the collection.
        /// </summary>
        /// <param name="item">
        /// The item to locate in the collection. 
        /// </param>
        /// <returns>
        /// The index of item if found in the collection; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            return this.collection.IndexOf(item);
        }

        /// <summary>
        /// Returns the index of an item in the collection.
        /// </summary>
        /// <param name="item">
        /// The item to locate in the collection. 
        /// </param>
        /// <returns>
        /// The index of item if found in the collection; otherwise, -1.
        /// </returns>
        int IList.IndexOf(object item)
        {
            return this.IndexOf((T)item);
        }

        /// <summary>
        /// Inserts an item into the collection at a particular index, raises a RaiseCollectionChanged event with a
        /// NotifyCollectionChangedAction.Add parameter and the index.
        /// </summary>
        /// <param name="index">
        /// The index to insert the item at in the collection.
        /// </param>
        /// <param name="item">
        /// The item to be inserted into the collection.
        /// </param>
        public void Insert(int index, T item)
        {
            lock (this.sync)
            {
                this.collection.Insert(index, item);
                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            }

            
        }

        /// <summary>
        /// Inserts an item into the collection at a particular index, raises a RaiseCollectionChanged event with a
        /// NotifyCollectionChangedAction.Add parameter and the index.
        /// </summary>
        /// <param name="index">
        /// The index to insert the item at in the collection.
        /// </param>
        /// <param name="item">
        /// The item to be inserted into the collection.
        /// </param>
        void IList.Insert(int index, object item)
        {
            this.Insert(index, (T)item);
        }

        /// <summary>
        /// Removes an item from the collection at a particular index, raises a RaiseCollectionChanged event with a
        /// NotifyCollectionChangedAction.Remove parameter and the index.
        /// </summary>
        /// <param name="index">
        /// The index of the item to be removed.
        /// </param>
        public void RemoveAt(int index)
        {
            T item;

            lock (this.sync)
            {
                item = this.collection[index];
                this.collection.RemoveAt(index);
                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
            }

            
        }

        /// <summary>
        /// Gets or sets the item at the specified index. 
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set. 
        /// </param>
        object IList.this[int index]
        {
            get { return ((IList<T>)this)[index]; }
            set { ((IList<T>)this)[index] = (T)value; }
        }

        /// <summary>
        /// Gets or sets the item at the specified index. 
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the element to get or set. 
        /// </param>
        public T this[int index]
        {
            get
            {
                lock (this.sync)
                {
                    return this.collection[index];
                }
            }

            set
            {
                lock (this.sync)
                {
                    this.collection[index] = value;
                }
            }
        }

        #endregion

        private int AddImpl(T item)
        {
            int index;

            lock (this.sync)
            {
                this.collection.Add(item);
                index = this.collection.IndexOf(item);

                if (index < 0)
                    throw new ArgumentException(string.Format("Index of returned a value of less than zero, check GetHashCode for type '{0}'", item.GetType().Name));

                this.RaiseCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            }

            return index;
        }
    }



    /// <summary>
    /// Base model providing methods to raise property change event and provides method for generating hash code
    /// in derived classes.
    /// </summary>
    [DataContract]
    public abstract class BaseModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T">The property type.
        /// </typeparam>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.RaisePropertyChanged(ExpressionHelper.ExpressionName(expression));
        }

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }

            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// The set property and notify.
        /// </summary>
        /// <param name="currentValue">
        /// The current value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <param name="propertyExpression">
        /// The property expression.
        /// </param>
        /// <typeparam name="T">The property type.
        /// </typeparam>
        protected virtual void SetPropertyAndNotify<T>(ref T currentValue, T newValue, Expression<Func<T>> propertyExpression)
        {
            this.SetPropertyAndNotify(ref currentValue, newValue, ExpressionHelper.ExpressionName(propertyExpression));
        }

        /// <summary>
        /// The set property and notify.
        /// </summary>
        /// <param name="currentValue">
        /// The current value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <typeparam name="T">The property type.
        /// </typeparam>
        protected virtual void SetPropertyAndNotify<T>(ref T currentValue, T newValue, string propertyName)
        {
            if (Equals(currentValue, newValue))
            {
                return;
            }

            currentValue = newValue;
            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Combines the values in the object array into a unique hash code.
        /// </summary>
        /// <param name="objects">
        /// The array of objects and there hash codes to be combined into a unique hash code.
        /// </param>
        /// <returns>
        /// Returns the combined hash codes.
        /// </returns>
        protected int CombineHashCodes(params object[] objects)
        {
            var hash = 0;

            foreach (var t in objects)
            {
                hash = (hash << 5) + hash;
                hash ^= this.GetEntryHash(t);
            }

            return hash;
        }

        /// <summary>
        /// Calculates the hash for an object.
        /// </summary>
        /// <param name="entry">
        /// The object to calculate the hash for.
        /// </param>
        /// <returns>
        /// Returns the hash for an object.
        /// </returns>
        private int GetEntryHash(object entry)
        {
            var entryHash = 0x61E04917; // slurped from .Net runtime internals...

            if (entry != null)
            {
                var subObjects = entry as object[];
                entryHash = subObjects != null ? this.CombineHashCodes(subObjects) : entry.GetHashCode();
            }

            return entryHash;
        }
    }


    /// <summary>
    /// Base collection model providing methods to raise property and collection change events.
    /// </summary>
    [DataContract]
    public abstract class BaseCollectionModel : BaseModel, INotifyCollectionChanged
    {
        /// <summary>
        /// The collection changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// The raise collection changed.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action)
        {
            var collectionChanged = this.CollectionChanged;
            if (collectionChanged == null)
            {
                return;
            }

            collectionChanged(this, new NotifyCollectionChangedEventArgs(action));
        }

        /// <summary>
        /// The raise collection changed.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="changedItem">
        /// The changed item.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            var collectionChanged = this.CollectionChanged;
            if (collectionChanged == null)
            {
                return;
            }

            collectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem, index));
        }

        /// <summary>
        /// The raise collection changed.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="newItem">
        /// The new item.
        /// </param>
        /// <param name="changedItem">
        /// The changed item.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object newItem, object changedItem, int index)
        {
            var collectionChanged = this.CollectionChanged;
            if (collectionChanged == null)
            {
                return;
            }

            collectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, changedItem, index));
        }
    }

    /// <summary>
    /// Expression helper class.
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// Extracts the name associated with an Expression.
        /// </summary>
        /// <param name="expression">
        /// The expression to be evaluated.
        /// </param>
        /// <typeparam name="T">The function type for the expression.
        /// </typeparam>
        /// <returns>
        /// Returns the expression name.
        /// </returns>
        public static string ExpressionName<T>(Expression<Func<T>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member.Name;
        }
    }
}
