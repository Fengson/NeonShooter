using System;
using System.Linq;
using NeonShooter.AppWarp.States;
using NeonShooter.AppWarp.Json;

namespace NeonShooter.AppWarp.Serializing.Json
{
    public class ReadOnlyStateJsonSerializer<TPropertyOwner, TState> : BaseJsonSerializer
    {
        public ReadOnlyStateJsonSerializer(JsonSerializationDict jsonSerializationDict)
            : base(jsonSerializationDict)
        {
        }

        public override IJsonObject SerializeAbsolute(IState state)
        {
            if (!(state is BasePropertyState<TPropertyOwner, TState>))
                throw new System.Exception("Parameter state must be of type BaseProperty<" + typeof(TPropertyOwner) + ", " + typeof(TState) + ">.");
            return SerializeAbsolute((BasePropertyState<TPropertyOwner, TState>)state);
        }

        public override IJsonObject SerializeRelative(IState state)
        {
            if (!(state is BasePropertyState<TPropertyOwner, TState>))
                throw new System.Exception("Parameter state must be of type " + typeof(TState) + ".");
            return SerializeRelative((BasePropertyState<TPropertyOwner, TState>)state);
        }

        public IJsonObject SerializeAbsolute(BasePropertyState<TPropertyOwner, TState> state)
        {
            return new JsonValue(state.Value);
        }

        public IJsonObject SerializeRelative(BasePropertyState<TPropertyOwner, TState> state)
        {
            return state.Changed ? SerializeAbsolute(state) : new JsonNull();
        }
    }
}
