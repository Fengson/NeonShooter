using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using System;
using System.IO;

namespace NeonShooter.AppWarp.States
{
    public abstract class BaseReadOnlyState<TPropertyOwner, TState> : IState
    {
        Action<TPropertyOwner, TState> stateApplier;

        public bool Changed { get; private set; }
        public IJsonObject RelativeJson { get { return Changed ? AbsoluteJson : new JsonNull(); } }
        public IJsonObject AbsoluteJson { get { return new JsonValue(Value); } }

        public int AbsoluteBinarySize { get { throw new NotImplementedException(); } }

        public TState Value { get; private set; }

        private BaseReadOnlyState(Action<TPropertyOwner, TState> stateApplier)
        {
            this.stateApplier = stateApplier;
        }

        protected BaseReadOnlyState(
            JSONNode jsonNode, Func<JSONNode, TState> toStateConverter,
            Action<TPropertyOwner, TState> stateApplier)
            : this(stateApplier)
        {
            if (jsonNode != null)
            {
                Value = toStateConverter(jsonNode);
                Changed = true;
            }
        }

        protected BaseReadOnlyState(bool valueInReader,
            BinaryReader br, Func<BinaryReader, TState> toStateReader,
            Action<TPropertyOwner, TState> stateApplier)
            : this(stateApplier)
        {
            if (valueInReader)
            {
                Value = toStateReader(br);
                Changed = true;
            }
        }

        protected BaseReadOnlyState(TState value)
        {
            Value = value;
            Changed = true;
        }

        public abstract void WriteRelativeBinaryTo(BinaryWriter bw);
        public abstract void WriteAbsoluteBinaryTo(BinaryWriter bw);

        public void ClearChanges()
        {
            Changed = false;
        }

        public void ApplyTo(object o)
        {
            if (o is TPropertyOwner)
                ApplyTo((TPropertyOwner)o);
        }

        public void ApplyTo(TPropertyOwner propertyOwner)
        {
            stateApplier(propertyOwner, Value);
        }
    }
}
