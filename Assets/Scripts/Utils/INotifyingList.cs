using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter.Utils
{
    public interface INotifyingList<T> : IEnumerable<T>, IEnumerable
    {
        event NotifyingListEventHandler<T> ListChanged;

        T this[int index] { get; set; }

        void Add(T item);
        void AddMany(IEnumerable<T> items);
        void Remove(T item);
        void RemoveMany(IEnumerable<T> items);
        void Clear();
    }

    public delegate void NotifyingListEventHandler<T>(NotifyingListEventArgs<T> e);

    public abstract class NotifyingListEventArgs
    {
        public ListChange Change { get; private set; }
        public int Index { get; private set; }

        protected NotifyingListEventArgs(ListChange change, int index)
        {
            Change = change;
            Index = index;
        }

        public enum ListChange
        {
            Add,
            AddMany,
            Remove,
            RemoveMany,
            Set,
            Clear
        }
    }

    public class NotifyingListEventArgs<T> : NotifyingListEventArgs
    {
        public static NotifyingListEventArgs<T> Add(T item) { return new NotifyingListEventArgs<T>(ListChange.Add, item); }
        public static NotifyingListEventArgs<T> AddMany(IEnumerable<T> items) { return new NotifyingListEventArgs<T>(ListChange.AddMany, items); }
        public static NotifyingListEventArgs<T> Remove(T item) { return new NotifyingListEventArgs<T>(ListChange.Remove, item); }
        public static NotifyingListEventArgs<T> RemoveMany(IEnumerable<T> items) { return new NotifyingListEventArgs<T>(ListChange.RemoveMany, items); }
        public static NotifyingListEventArgs<T> Set(T newItem, T oldItem, int index) { return new NotifyingListEventArgs<T>(ListChange.Set, newItem, oldItem, index); }
        public static NotifyingListEventArgs<T> Clear() { return new NotifyingListEventArgs<T>(ListChange.Clear); }
        
        public T Item { get; private set; }
        public T NewItem { get; private set; }
        public T OldItem { get; private set; }
        public IEnumerable<T> Items { get; private set; }

        NotifyingListEventArgs(ListChange change, T item)
            : this(change)
        {
            Item = item;
        }

        NotifyingListEventArgs(ListChange change, T newItem, T oldItem, int index)
            : base(change, index)
        {
            NewItem = newItem;
            OldItem = oldItem;
        }

        NotifyingListEventArgs(ListChange change, IEnumerable<T> items)
            : this(change)
        {
            Items = items.ToList();
        }

        NotifyingListEventArgs(ListChange change)
            : base(change, -1)
        {
        }
    }
}
