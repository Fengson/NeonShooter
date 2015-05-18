using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Utils;
using System;

namespace NeonShooter.AppWarp.States
{
    public class PropertyState<TProperty, TState> : IState
    {
        public static PropertyState<TProperty, TState> FromJSONNode(
            JSONNode jsonNode, Func<JSONNode, TState> toStateConverter,
            Action<NotifyingProperty<TProperty>, TState> stateApplier)
        {
            var propertyState = new PropertyState<TProperty, TState>(stateApplier);
            if (jsonNode != null)
            {
                propertyState.Value = toStateConverter(jsonNode);
                propertyState.Changed = true;
            }
            return propertyState;
        }

        public bool Changed { get; private set; }
        public IJsonObject RelativeJson { get { return Changed ? AbsoluteJson : new JsonNull(); } }
        public IJsonObject AbsoluteJson { get { return converter(Value); } }

        Func<TProperty, TState> stateSelector;
        Func<TState, IJsonObject> converter;
        Action<NotifyingProperty<TProperty>, TState> stateApplier;

        TState value;
        public TState Value
        {
            get { return value; }
            set
            {
                if (System.Object.Equals(value, this.value)) return;

                this.value = value;
                Changed = true;
            }
        }

        private PropertyState()
        {
        }

        private PropertyState(Action<NotifyingProperty<TProperty>, TState> stateApplier)
        {
            this.stateApplier = stateApplier;
        }

        public PropertyState(NotifyingProperty<TProperty> property,
            Func<TProperty, TState> stateSelector,
            Func<TState, IJsonObject> converter)
        {
            this.stateSelector = stateSelector;
            this.converter = converter;

            Value = stateSelector(property.Value);

            property.ValueChanged += property_ValueChanged;
        }

        public void ClearChanges()
        {
            Changed = false;
        }

        public void ApplyTo(object o)
        {
            var property = o as NotifyingProperty<TProperty>;
            if (property != null) ApplyTo(property);
        }

        public void ApplyTo(NotifyingProperty<TProperty> property)
        {
            if (Changed) stateApplier(property, Value);
        }

        void property_ValueChanged(TProperty oldValue, TProperty newValue)
        {
            Value = stateSelector(newValue);
        }
    }
}
