using System;

namespace NeonShooter.Utils
{
    public class NotifyingProperty<T>
    {
        public static NotifyingProperty<T> PublicBoth(T value = default(T))
        {
            return new NotifyingProperty<T>(value);
        }

        public static NotifyingProperty<T> PublicGetPrivateSet(object access, T value = default(T))
        {
            return new NotifyingProperty<T>(access, true, false, value);
        }

        private object access;

        public event NotifyingPropertyEventHandler<T> ValueChanged;

        public bool PublicGet { get; private set; }
        public bool PublicSet { get; private set; }

        T value;
        public T this[object access]
        {
            get
            {
                if (!PublicGet && (access == null || !access.Equals(this.access)))
                    throw new InvalidOperationException("Cannot get property value - it is defined as private and given access object does not match the required one.");
                return value;
            }
            set
            {
                if (!PublicSet && (access == null || !access.Equals(this.access)))
                    throw new InvalidOperationException("Cannot set property value - it is defined as private and given access object does not match the required one.");
                if (this.value == null && value == null ||
                    this.value.Equals(value)) return;

                var oldValue = this.value;
                this.value = value;
                if (ValueChanged != null)
                    ValueChanged(oldValue, value);
            }
        }
        public T Value
        {
            get { return this[new object()]; }
            set { this[new object()] = value; }
        }

        NotifyingProperty(T value = default(T))
            : this(null, true, true, value)
        {
        }

        NotifyingProperty(object access, bool publicGet, bool publicSet, T value = default(T))
        {
            this.access = access;
            PublicGet = publicGet;
            PublicSet = publicSet;
            this.value = value;
        }
    }

    public delegate void NotifyingPropertyEventHandler<T>(T oldValue, T newValue);
}
