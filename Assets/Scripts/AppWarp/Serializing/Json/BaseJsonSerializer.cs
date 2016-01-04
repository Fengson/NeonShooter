using NeonShooter.AppWarp.Json;
using NeonShooter.AppWarp.States;

namespace NeonShooter.AppWarp.Serializing.Json
{
    public abstract class BaseJsonSerializer
    {
        protected JsonSerializationDict JsonSerializationDict { get; private set; }

        protected BaseJsonSerializer(JsonSerializationDict jsonSerializationDict)
        {
            JsonSerializationDict = jsonSerializationDict;
        }

        public abstract IJsonObject SerializeAbsolute(IState state);
        public abstract IJsonObject SerializeRelative(IState state);
    }
}
