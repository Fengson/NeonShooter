using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter.Utils
{
    public class NotifyingList<T> : INotifyingList<T>
    {
        public event NotifyingListEventHandler<T> ListChanged;

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
                    ListChanged(NotifyingListEventArgs<T>.Set(value, oldValue, index));
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
                ListChanged(NotifyingListEventArgs<T>.Add(item));
        }

        public void AddMany(IEnumerable<T> items)
        {
            list.AddRange(items);
            if (ListChanged != null)
                ListChanged(NotifyingListEventArgs<T>.AddMany(items));
        }

        public void Remove(T item)
        {
            list.Remove(item);
            if (ListChanged != null)
                ListChanged(NotifyingListEventArgs<T>.Remove(item));
        }

        public void RemoveMany(IEnumerable<T> items)
        {
            list.RemoveAll(i => items.Contains(i));
            if (ListChanged != null)
                ListChanged(NotifyingListEventArgs<T>.RemoveMany(items));
        }

        public void Clear()
        {
            list.Clear();
            if (ListChanged != null)
                ListChanged(NotifyingListEventArgs<T>.Clear());
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
