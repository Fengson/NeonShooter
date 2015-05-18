using System.Collections.Generic;

namespace NeonShooter.Utils
{
    /// <summary>
    /// List that is bi-directional. It consists of two parts: forward and backward. Forward part starts at index 0 and goes positive, backward part at -1 and goes negative. When using methods with "Forward" or "Backward" in their names, apropriate parts will be used. When using indices, there will be used a part, that is indicated by the sign of a given index.
    /// </summary>
    /// <typeparam name="T">Type of items in the TwoWayList&lt;T&gt;.</typeparam>
    public class TwoWayList<T> : IEnumerable<T>
        where T : class
    {
        protected List<T> backwardItems;
        protected List<T> forwardItems;

        /// <summary>
        /// Creates new instance of TwoWayList&lt;T&gt;.
        /// </summary>
        public TwoWayList()
        {
            backwardItems = new List<T>();
            forwardItems = new List<T>();
        }

        /// <summary>
        /// Gives number of items in backward part of the TwoWayList&lt;T&gt;.
        /// </summary>
        public int BackwardCount
        {
            get { return backwardItems.Count; }
        }

        /// <summary>
        /// Gives number of items in forward part of the TwoWayList&lt;T&gt;.
        /// </summary>
        public int ForwardCount
        {
            get { return forwardItems.Count; }
        }

        /// <summary>
        /// Gives total number of items in both parts of the TwoWayList&lt;T&gt;.
        /// </summary>
        public int Count
        {
            get { return BackwardCount + ForwardCount; }
        }

        /// <summary>
        /// Allows to get or replace an item on given index. Throws IndexOutOfBoundsException, when given index is out of bounds of the TwoWayList.
        /// </summary>
        /// <param name="index">
        /// Index of item to get or replace. Indices 0 and greater apply to the forward part. Indices -1 and lower apply to the backward part.
        /// </param>
        public T this[int index]
        {
            get
            {
                int innerIndex = ToInnerIndex(index);
                return GetInnerList(index)[innerIndex];
            }
            set
            {
                int innerIndex = ToInnerIndex(index);
                GetInnerList(index)[innerIndex] = value;
            }
        }

        /// <summary>
        /// Gets an item on given index. Returns null, when given index is out of bounds of the TwoWayList&lt;T&gt;.
        /// </summary>
        /// <param name="index">
        /// Index of item to get. Indices 0 and greater apply to the forward part. Indices -1 and lower apply to the backward part.
        /// </param>
        public T TryGet(int index)
        {
            int innerIndex = ToInnerIndex(index);
            var innerList = GetInnerList(index);
            if (innerIndex >= innerList.Count) return null;
            return innerList[innerIndex];
        }

        /// <summary>
        /// Replaces an item on given index. Returns false, when given index is out of bounds of the TwoWayList&lt;T&gt;, otherwise true.
        /// </summary>
        /// <param name="index">
        /// Index of item to replace. Indices 0 and greater apply to the forward part. Indices -1 and lower apply to the backward part.
        /// </param>
        public bool TrySet(int index, T value)
        {
            int innerIndex = ToInnerIndex(index);
            var innerList = GetInnerList(index);
            if (innerIndex >= innerList.Count) return false;
            innerList[innerIndex] = value;
            return true;
        }

        /// <summary>
        /// Returns index of given item if found, or null otherwise. Backward part is always checked first.
        /// </summary>
        /// <param name="item">Item to be found in the TwoWayList&lt;T&gt;.</param>
        public int? IndexOf(T item)
        {
            int innerIndex = backwardItems.IndexOf(item);
            bool isBackward = innerIndex != -1;
            if (!isBackward) innerIndex = forwardItems.IndexOf(item);
            if (innerIndex == -1) return null;
            return ToOuterIndex(innerIndex, isBackward);
        }

        /// <summary>
        /// Inserts an item at the given index of the TwoWayList&lt;T&gt;. Note that depending on given index items in the apropriate part will be moved by one. For index 0 it will be the forward part.
        /// </summary>
        /// <param name="index">Index at which item should be inserted. Indices 0 and greater affect the forward part. Indices -1 and lower affect the backward part.</param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            int innerIndex = ToInnerIndex(index);
            var innerList = GetInnerList(index);
            innerList.Insert(innerIndex, item);
        }

        /// <summary>
        /// Adds an item at the end of the backward part of the TwoWayList&lt;T&gt;. It receives the most negative index.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void AddBackward(T item)
        {
            backwardItems.Add(item);
        }

        /// <summary>
        /// Adds an item at the end of the forward part of the TwoWayList&lt;T&gt;. It receives the most positive index.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void AddForward(T item)
        {
            forwardItems.Add(item);
        }

        /// <summary>
        /// Finds an item and removes it from the TwoWayList&lt;T&gt;. Backward part is always checked for the item first. Returns true, if item has been successfully removed; otherwise (e.g. when there is no given item) - false. Note that items in modified part will be moved by one. For item that was in index 0 it will be the forward part.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public bool Remove(T item)
        {
            bool success = backwardItems.Remove(item);
            if (!success) forwardItems.Remove(item);
            return success;
        }

        /// <summary>
        /// Removes an item from the end of the backward part of the TwoWayList&lt;T&gt; and returns it as a result. If the backward part has no items, null is returned.
        /// </summary>
        public T RemoveBackward()
        {
            if (backwardItems.Count == 0) return null;
            T item = backwardItems[backwardItems.Count - 1];
            backwardItems.RemoveAt(backwardItems.Count - 1);
            return item;
        }

        /// <summary>
        /// Removes an item from the end of the forward part of the TwoWayList&lt;T&gt; and returns it as a result. If the forward part has no items, null is returned.
        /// </summary>
        public T RemoveForward()
        {
            if (forwardItems.Count == 0) return null;
            T item = forwardItems[forwardItems.Count - 1];
            forwardItems.RemoveAt(forwardItems.Count - 1);
            return item;
        }

        /// <summary>
        /// Removes an item from the given index of the TwoWayList&lt;T&gt; and returns it as a result. If the part indicated by index has no items, null is returned. Note that depending on given index items in the apropriate part will be moved by one. For index 0 it will be the forward part.
        /// </summary>
        /// <param name="index">Index from which item should be removed. Indices 0 and greater affect the forward part. Indices -1 and lower affect the backward part.</param>
        public T RemoveAt(int index)
        {
            int innerIndex = ToInnerIndex(index);
            var innerList = GetInnerList(index);
            if (innerList.Count == 0) return null;
            T item = innerList[innerIndex];
            innerList.RemoveAt(innerIndex);
            return item;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the TwoWayList&lt;T&gt;.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return new TwoWayListEnumerator<T>(backwardItems.GetEnumerator(), forwardItems.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected List<T> GetInnerList(int outerIndex)
        {
            return outerIndex < 0 ? backwardItems : forwardItems;
        }

        protected int ToInnerIndex(int outerIndex)
        {
            if (outerIndex < 0) outerIndex = -outerIndex - 1;
            return outerIndex;
        }

        int ToOuterIndex(int innerIndex, bool isBackward)
        {
            if (isBackward) innerIndex = -(innerIndex + 1);
            return innerIndex;
        }

        private class TwoWayListEnumerator<U> : IEnumerator<U>
            where U : T
        {
            IEnumerator<U> backwardEnumerator;
            IEnumerator<U> forwardEnumerator;

            IEnumerator<U> currentEnumerator;

            public TwoWayListEnumerator(
                IEnumerator<U> backwardEnumerator,
                IEnumerator<U> forwardEnumerator)
            {
                this.backwardEnumerator = backwardEnumerator;
                this.forwardEnumerator = forwardEnumerator;

                currentEnumerator = backwardEnumerator;
            }

            public U Current
            {
                get { return currentEnumerator.Current; }
            }

            public void Dispose()
            {
                backwardEnumerator.Dispose();
                forwardEnumerator.Dispose();
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                bool success = currentEnumerator.MoveNext();
                if (!success && currentEnumerator == backwardEnumerator)
                {
                    currentEnumerator = forwardEnumerator;
                    success = currentEnumerator.MoveNext();
                }
                return success;
            }

            public void Reset()
            {
                backwardEnumerator.Reset();
                forwardEnumerator.Reset();

                currentEnumerator = backwardEnumerator;
            }
        }
    }
}