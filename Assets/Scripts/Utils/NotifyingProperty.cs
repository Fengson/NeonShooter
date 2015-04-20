using System;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter.Utils
{
    public class NotifyingProperty<T>
    {
        public event NotifyingPropertyEventHandler<T> OnValueChanged;

        public Object Owner { get; private set; }

        public bool PublicGet { get; private set; }
        public bool PublicSet { get; private set; }

        T value;
        public T this[Object owner]
        {
            get
            {
                if (!PublicGet && (owner == null || !owner.Equals(Owner)))
                    throw new InvalidOperationException("Cannot get property value - it is defined as private and given object is not its owner.");
                return value;
            }
            set
            {
                if (!PublicSet && (owner == null || !owner.Equals(Owner)))
                    throw new InvalidOperationException("Cannot set property value - it is defined as private and given object is not its owner.");
                if (this.value == null && value == null ||
                    this.value.Equals(value)) return;

                var oldValue = this.value;
                this.value = value;
                if (OnValueChanged != null)
                    OnValueChanged(oldValue, value);
            }
        }
        public T Value
        {
            get { return this[new Object()]; }
            set { this[new Object()] = value; }
        }

        public NotifyingProperty()
            : this(null, true, true, default(T))
        {
        }

        public NotifyingProperty(T value)
            : this(null, true, true, value)
        {
        }

        public NotifyingProperty(Object owner, bool publicGet, bool publicSet)
            : this(owner, publicGet, publicSet, default(T))
        {
        }

        public NotifyingProperty(Object owner, bool publicGet, bool publicSet, T value)
        {
            Owner = owner;
            PublicGet = publicGet;
            PublicSet = publicSet;
            this.value = value;
        }
    }

    public delegate void NotifyingPropertyEventHandler<T>(T oldValue, T newValue);
}
