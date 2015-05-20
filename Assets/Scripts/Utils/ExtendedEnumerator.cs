using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonShooter.Utils
{
    public class ExtendedEnumerator<T> : IEnumerator<T>
    {
        IEnumerator<T> innerEnumerator;

        T next;

        public T Current { get; private set; }
        object IEnumerator.Current { get { return Current; } }

        public bool HasNext { get; private set; }

        public ExtendedEnumerator(IEnumerator<T> innerEnumerator)
        {
            this.innerEnumerator = innerEnumerator;
            MoveInnerNext();
        }

        public bool MoveNext()
        {
            Current = next;
            bool hasCurrent = HasNext;

            MoveInnerNext();

            return hasCurrent;
        }

        private void MoveInnerNext()
        {
            HasNext = innerEnumerator.MoveNext();
            if (HasNext) next = innerEnumerator.Current;
            else next = default(T);
        }

        public void Dispose()
        {
            throw new NotSupportedException();
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}
