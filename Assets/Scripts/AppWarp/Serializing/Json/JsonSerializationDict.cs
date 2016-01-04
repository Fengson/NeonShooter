using System.Collections.Generic;
using NeonShooter.AppWarp.States;
using NeonShooter.AppWarp.Json;
using UnityEngine;

namespace NeonShooter.AppWarp.Serializing.Json
{
    public class JsonSerializationDict
    {
        Dictionary<System.Type, System.Func<object, IJsonObject>> stateToJsonConversionMethods;
        Dictionary<System.Type, BaseJsonSerializer> stateSerializers;

        public JsonSerializationDict()
        {
            stateToJsonConversionMethods = new Dictionary<System.Type, System.Func<object, IJsonObject>>()
            {
                { typeof(Vector3), o => ((Vector3)o).ToJson() },
                { typeof(Quaternion), o => ((Quaternion)o).ToJson() }
            };
            stateSerializers = new Dictionary<System.Type, BaseJsonSerializer>()
            {
                { typeof(CubelingState), new CubelingStateJsonSerializer(this) },
                { typeof(ProjectileState), new ProjectileStateJsonSerializer(this) }
            };
        }

        public System.Func<TState, IJsonObject> StateToJson<TState>()
        {
            var type = typeof(TState);
            System.Func<object, IJsonObject> func;

            bool success = stateToJsonConversionMethods.TryGetValue(type, out func);
            if (!success)
                throw new System.ArgumentException("No conversion method exists for given type (" + type + ")");

            return new System.Func<TState, IJsonObject>(s => func(s));
        }

        public BaseJsonSerializer GetStateSerializer<TState>()
        {
            var type = typeof(TState);
            BaseJsonSerializer serializer;

            bool success = stateSerializers.TryGetValue(type, out serializer);
            if (!success)
                throw new System.ArgumentException("No serializer exists for given type (" + type + ")");

            return serializer;
        }
    }
}
