using System;
using UnityEngine;

namespace NeonShooter.Utils
{
    public class NotifyingProperty<T>
    {
        public event NotifyingPropertyEventHandler<T> ValueChanged;

        T value;
        public T Value
        {
            get
            {
                return value;
            }
            set
            {
                if (System.Object.Equals(this.value, value)) return;

                var oldValue = this.value;
                this.value = value;
                if (ValueChanged != null)
                    ValueChanged(oldValue, value);
            }
        }

        public NotifyingProperty(T value = default(T))
        {
            this.value = value;
        }
    }

    public delegate void NotifyingPropertyEventHandler<T>(T oldValue, T newValue);
}
