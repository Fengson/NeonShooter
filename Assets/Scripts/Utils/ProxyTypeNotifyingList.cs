using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter.Utils
{
    public class ProxyTypeNotifyingList<TProxy, TOriginal> : INotifyingList<TProxy>
    {
        public event NotifyingListEventHandler<TProxy> ListChanged;

        INotifyingList<TOriginal> originalList;
        Func<TOriginal, TProxy> toProxyType;
        Func<TProxy, TOriginal> toOriginalType;

        public TProxy this[int index]
        {
            get { return toProxyType(originalList[index]); }
            set { originalList[index] = toOriginalType(value); }
        }

        public ProxyTypeNotifyingList(INotifyingList<TOriginal> originalList,
            Func<TOriginal, TProxy> toProxyType, Func<TProxy, TOriginal> toOriginalType)
        {
            this.originalList = originalList;
            this.toProxyType = toProxyType;
            this.toOriginalType = toOriginalType;
        }

        public void Add(TProxy item)
        {
            originalList.Add(toOriginalType(item));
        }

        public void AddMany(IEnumerable<TProxy> items)
        {
            originalList.AddMany(from i in items select toOriginalType(i));
        }

        public void Remove(TProxy item)
        {
            originalList.Remove(toOriginalType(item));
        }

        public void RemoveMany(IEnumerable<TProxy> items)
        {
            originalList.RemoveMany(from i in items select toOriginalType(i));
        }

        public void Clear()
        {
            originalList.Clear();
        }

        public IEnumerator<TProxy> GetEnumerator()
        {
            return (from item in originalList select toProxyType(item)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
