using NeonShooter.AppWarp.Json;
using NeonShooter.AppWarp.States;

namespace NeonShooter.AppWarp.Serializing.Json
{
    public class CellStateJsonSerializer : BaseJsonSerializer
    {
        public CellStateJsonSerializer(JsonSerializationDict jsonSerializationDict)
            : base(jsonSerializationDict)
        {
        }

        public override IJsonObject SerializeAbsolute(IState state)
        {
            if (!(state is CellState))
                throw new System.Exception("Parameter state must be of type CellState.");
            return SerializeAbsolute((CellState)state);
        }

        public override IJsonObject SerializeRelative(IState state)
        {
            if (!(state is CellState))
                throw new System.Exception("Parameter state must be of type CellState.");
            return SerializeRelative((CellState)state);
        }

        public IJsonObject SerializeAbsolute(CellState state)
        {
            return state.Position.ToJson();
        }

        public IJsonObject SerializeRelative(CellState state)
        {
            return state.Position.ToJson();
        }
    }
}
