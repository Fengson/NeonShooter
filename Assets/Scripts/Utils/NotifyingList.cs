using System;
using System.Collections;
using System.Collections.Generic;

namespace NeonShooter.Utils
{
    public class NotifyingList<T> : IEnumerable<T>, IEnumerable
    {
        public event NotifyingListEventHandler ListChanged;

        List<T> list;
        public T this[int index]
        {
            get { return list[index]; }
            set
            {
                if (Object.Equals(value, list[index])) return;

                var oldValue = list[index];
                list[index] = value;
                if (ListChanged != null)
                    ListChanged(new NotifyingListEventArgs(ListChange.ItemSet, value, oldValue, index));
            }
        }

        public NotifyingList()
        {
            list = new List<T>();
        }

        public void Add(T item)
        {
            list.Add(item);
            if (ListChanged != null)
                ListChanged(new NotifyingListEventArgs(ListChange.ItemAdd, item));
        }

        public void Remove(T item)
        {
            list.Remove(item);
            if (ListChanged != null)
                ListChanged(new NotifyingListEventArgs(ListChange.ItemRemove, item));
        }

        public void Clear()
        {
            list.Clear();
            if (ListChanged != null)
                ListChanged(new NotifyingListEventArgs(ListChange.Clear));
        }

        public delegate void NotifyingListEventHandler(NotifyingListEventArgs e);

        public class NotifyingListEventArgs
        {
            public ListChange Change { get; private set; }
            public T NewItem { get; private set; }
            public T OldItem { get; private set; }
            public int Index { get; private set; }

            public NotifyingListEventArgs(ListChange change, T newItem = default(T), T oldItem = default(T), int index = -1)
            {
                Change = change;
                NewItem = newItem;
                OldItem = oldItem;
                Index = index;
            }
        }

        public enum ListChange
        {
            ItemAdd,
            ItemRemove,
            ItemSet,
            Clear
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
