using System.Linq;
using NeonShooter.AppWarp.States;
using NeonShooter.AppWarp.Json;

namespace NeonShooter.AppWarp.Serializing.Json
{
    public class ListStateJsonSerializer<TList, TState> : BaseJsonSerializer
        where TState : class, IState
    {
        public const string AddedKey = "Added";
        public const string ObservedKey = "Observed";
        public const string RemovedKey = "Removed";
        
        public ListStateJsonSerializer(JsonSerializationDict jsonSerializationDict)
            : base(jsonSerializationDict)
        {
        }

        public override IJsonObject SerializeAbsolute(IState state)
        {
            if (!(state is BaseListState<TList, TState>))
                throw new System.Exception("Parameter state must be of type BaseListState<" + typeof(TList) + ", " + typeof(TState) + ">.");
            return SerializeAbsolute((BaseListState<TList, TState>)state);
        }

        public override IJsonObject SerializeRelative(IState state)
        {
            if (!(state is BaseListState<TList, TState>))
                throw new System.Exception("Parameter state must be of type BaseListState<" + typeof(TList) + ", " + typeof(TState) + ">.");
            return SerializeRelative((BaseListState<TList, TState>)state);
        }

        public IJsonObject SerializeAbsolute(BaseListState<TList, TState> state)
        {
            var serializer = JsonSerializationDict.GetStateSerializer<TState>();

            var json = new JsonObject();
            if (state.AllItems.Count > 0)
                json.Append(new JsonPair(AddedKey, new JsonArray(
                    from i in state.AllItems select serializer.SerializeRelative(i.Value))));
            return json;
        }

        public IJsonObject SerializeRelative(BaseListState<TList, TState> state)
        {
            var serializer = JsonSerializationDict.GetStateSerializer<TState>();

            var json = new JsonObject();

            if (state.AddedItems.Count > 0)
                json.Append(new JsonPair(AddedKey, new JsonArray(
                    from i in state.AddedItems select serializer.SerializeRelative(i.Value))));
            if (state.ObservedItems.Count > 0)
                json.Append(new JsonPair(ObservedKey, new JsonArray(
                    from i in state.ObservedItems where i.Value.Changed select serializer.SerializeRelative(i.Value))));
            if (state.RemovedItems.Count > 0)
                json.Append(new JsonPair(RemovedKey, new JsonArray(
                    from i in state.RemovedItems select serializer.SerializeRelative(i.Value))));

            return json;
        }
    }
}
