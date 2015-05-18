using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using System;

namespace NeonShooter.AppWarp.States
{
    public class ReadOnlyState<TPropertyOwner, TState> : IState
    {
        public static ReadOnlyState<TPropertyOwner, TState> FromJSONNode(
            JSONNode jsonNode, Func<JSONNode, TState> toStateConverter,
            Action<TPropertyOwner, TState> stateApplier)
        {
            var readOnlyState = new ReadOnlyState<TPropertyOwner, TState>(stateApplier);
            if (jsonNode != null)
            {
                readOnlyState.Value = toStateConverter(jsonNode);
                readOnlyState.Changed = true;
            }
            return readOnlyState;
        }

        Action<TPropertyOwner, TState> stateApplier;

        public bool Changed { get; private set; }
        public IJsonObject RelativeJson { get { return Changed ? AbsoluteJson : new JsonNull(); } }
        public IJsonObject AbsoluteJson { get { return new JsonValue(Value); } }

        public TState Value { get; private set; }

        private ReadOnlyState()
        {
        }

        private ReadOnlyState(Action<TPropertyOwner, TState> stateApplier)
        {
            this.stateApplier = stateApplier;
        }

        public ReadOnlyState(TState value)
        {
            Value = value;
            Changed = true;
        }

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
