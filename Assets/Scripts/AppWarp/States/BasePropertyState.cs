using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Utils;
using System;
using System.IO;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public abstract class BasePropertyState<TProperty, TState> : IState
    {
        Func<TProperty, TState> stateSelector;
        Func<TState, IJsonObject> converter;
        Action<NotifyingProperty<TProperty>, TState> stateApplier;

        public bool Changed { get; private set; }
        public IJsonObject RelativeJson { get { return Changed ? AbsoluteJson : new JsonNull(); } }
        public IJsonObject AbsoluteJson { get { return converter(Value); } }

        public int AbsoluteBinarySize { get { throw new NotImplementedException(); } }

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

        protected BasePropertyState(
            JSONNode jsonNode, Func<JSONNode, TState> toStateConverter,
            Action<NotifyingProperty<TProperty>, TState> stateApplier)
        {
            this.stateApplier = stateApplier;

            if (jsonNode != null)
            {
                value = toStateConverter(jsonNode);
                Changed = true;
            }
        }

        protected BasePropertyState(bool valueInReader,
            BinaryReader br, Func<BinaryReader, TState> toStateReader,
            Action<NotifyingProperty<TProperty>, TState> stateApplier)
        {
            this.stateApplier = stateApplier;

            if (valueInReader)
            {
                value = toStateReader(br);
                Changed = true;
            }
        }

        protected BasePropertyState(NotifyingProperty<TProperty> property,
            Func<TProperty, TState> stateSelector,
            Func<TState, IJsonObject> toJsonConverter)
        {
            this.stateSelector = stateSelector;
            this.converter = toJsonConverter;

            Value = stateSelector(property.Value);

            property.ValueChanged += property_ValueChanged;
        }

        public abstract void WriteRelativeBinaryTo(BinaryWriter bw);
        public abstract void WriteAbsoluteBinaryTo(BinaryWriter bw);

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
