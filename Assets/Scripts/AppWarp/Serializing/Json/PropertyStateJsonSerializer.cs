using System;
using System.Linq;
using NeonShooter.AppWarp.States;
using NeonShooter.AppWarp.Json;

namespace NeonShooter.AppWarp.Serializing.Json
{
    public class PropertyStateJsonSerializer<TProperty, TState> : BaseJsonSerializer
    {
        public PropertyStateJsonSerializer(JsonSerializationDict jsonSerializationDict)
            : base(jsonSerializationDict)
        {
        }

        public override IJsonObject SerializeAbsolute(IState state)
        {
            if (!(state is BasePropertyState<TProperty, TState>))
                throw new System.Exception("Parameter state must be of type BaseProperty<" + typeof(TProperty) + ", " + typeof(TState) + ">.");
            return SerializeAbsolute((BasePropertyState<TProperty, TState>)state);
        }

        public override IJsonObject SerializeRelative(IState state)
        {
            if (!(state is BasePropertyState<TProperty, TState>))
                throw new System.Exception("Parameter state must be of type " + typeof(TState) + ".");
            return SerializeRelative((BasePropertyState<TProperty, TState>)state);
        }

        public IJsonObject SerializeAbsolute(BasePropertyState<TProperty, TState> state)
        {
            return JsonSerializationDict.StateToJson<TState>()(state.Value);
        }

        public IJsonObject SerializeRelative(BasePropertyState<TProperty, TState> state)
        {
            return state.Changed ? SerializeAbsolute(state) : new JsonNull();
        }
    }
}
