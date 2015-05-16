using System;
using System.Collections.Generic;

namespace NeonShooter.Utils
{
    public class NotifyingList<T>
    {
        public event NotifyingListItemSetEventHandler<T> OnItemSet;
        public event NotifyingListItemEventHandler<T> OnItemAdded;
        public event NotifyingListItemEventHandler<T> OnItemRemoved;
        public event NotifyingListEventHandler<T> OnCleared;

        List<T> list;
        public T this[int index]
        {
            get { return list[index]; }
            set
            {
                if (Object.Equals(value, list[index])) return;

                var oldValue = list[index];
                list[index] = value;
                if (OnItemSet != null)
                    OnItemSet(index, oldValue, value);
            }
        }

        public NotifyingList()
        {
            list = new List<T>();
        }

        public void Add(T item)
        {
            list.Add(item);
            if (OnItemAdded != null)
                OnItemAdded(item);
        }

        public void Remove(T item)
        {
            list.Remove(item);
            if (OnItemAdded != null)
                OnItemAdded(item);
        }

        public void Clear()
        {
            list.Clear();
            if (OnCleared != null)
                OnCleared();
        }
    }

    public delegate void NotifyingListItemSetEventHandler<T>(int index, T oldItem, T newItem);
    public delegate void NotifyingListItemEventHandler<T>(T item);
    public delegate void NotifyingListEventHandler<T>();
}
