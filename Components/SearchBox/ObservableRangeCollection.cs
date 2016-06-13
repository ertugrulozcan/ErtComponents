using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Eigen.Core.Utility
{
    [Serializable]
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");
 
            _suppressNotification = true;

            foreach (T item in list)
            {
                Add(item);
            }

            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
          }

        public ObservableRangeCollection() : base()
        {

        }

        public ObservableRangeCollection(IEnumerable<T> list) : base(list)
        {

        }
        


        //    public ObservableRangeCollection()
        //        :base()
        //    {
        //        CollectionChanged += new NotifyCollectionChangedEventHandler(TrulyObservableCollection_CollectionChanged);
        //    }

        //void TrulyObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.NewItems != null)
        //    {
        //        foreach (Object item in e.NewItems)
        //        {
        //            if((item as INotifyPropertyChanged) != null)
        //                (item as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
        //        }
        //    }
        //    if (e.OldItems != null)
        //    {
        //        foreach (Object item in e.OldItems)
        //        {
        //            if ((item as INotifyPropertyChanged) != null)
        //                (item as INotifyPropertyChanged).PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
        //        }
        //    }
        //}

        //void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    NotifyCollectionChangedEventArgs a = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        //    OnCollectionChanged(a);
        //}
    }


        //public ObservableRangeCollection() 
        //    :base()
        //{ }

        //public ObservableRangeCollection(List<T> vals)
        //    : base()
        //{
        //    this.AddRange(vals);
        //}

        //public void AddRange(IEnumerable<T> items)
        //{
        //    this.CheckReentrancy();
        //    foreach (var item in items)
        //        this.Items.Add(item);
        //    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //}

        ///// <summary> 
        ///// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        ///// </summary> 
        //public void AddRange(IEnumerable<T> collection)
        //{
        //    if (collection == null) return; // throw new ArgumentNullException("collection");

        //    foreach (var i in collection) Items.Add(i);
        //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection.ToList()));
        //}

        ///// <summary> 
        ///// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
        ///// </summary> 
        //public void RemoveRange(IEnumerable<T> collection)
        //{
        //    if (collection == null) throw new ArgumentNullException("collection");

        //    foreach (var i in collection) Items.Remove(i);
        //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, collection.ToList()));
        //}

        ///// <summary> 
        ///// Clears the current collection and replaces it with the specified item. 
        ///// </summary> 
        //public void Replace(T item)
        //{
        //    ReplaceRange(new T[] { item });
        //}

        ///// <summary> 
        ///// Clears the current collection and replaces it with the specified collection. 
        ///// </summary> 
        //public void ReplaceRange(IEnumerable<T> collection)
        //{
        //    if (collection == null) throw new ArgumentNullException("collection");

        //    Items.Clear();
        //    foreach (var i in collection) Items.Add(i);
        //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        //}

        ///// <summary> 
        ///// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class. 
        ///// </summary> 
        //public ObservableRangeCollection()
        //    : base() { }

        ///// <summary> 
        ///// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
        ///// </summary> 
        ///// <param name="collection">collection: The collection from which the elements are copied.</param> 
        ///// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
        //public ObservableRangeCollection(IEnumerable<T> collection)
        //    : base(collection) { }
    
}
