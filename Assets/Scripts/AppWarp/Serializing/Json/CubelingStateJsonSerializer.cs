using NeonShooter.AppWarp.States;
using NeonShooter.AppWarp.Json;
using UnityEngine;

namespace NeonShooter.AppWarp.Serializing.Json
{
    public class CubelingStateJsonSerializer : BaseJsonSerializer
    {
        public const string IdKey = "Id";
        public const string DontLerpKey = "DontLerp";
        public const string PositionKey = "Position";
        public const string RotationKey = "Rotation";
        public const string VelocityKey = "Velocity";

        public CubelingStateJsonSerializer(JsonSerializationDict jsonSerializationDict)
            : base(jsonSerializationDict)
        {
        }

        public override IJsonObject SerializeAbsolute(IState state)
        {
            if (!(state is CubelingState))
                throw new System.Exception("Parameter state must be of type CubelingState.");
            return SerializeAbsolute((CubelingState)state);
        }

        public override IJsonObject SerializeRelative(IState state)
        {
            if (!(state is CubelingState))
                throw new System.Exception("Parameter state must be of type CubelingState.");
            return SerializeRelative((CubelingState)state);
        }

        public IJsonObject SerializeAbsolute(CubelingState state)
        {
            var vector3Serializer = new PropertyStateJsonSerializer<Vector3, Vector3>(JsonSerializationDict);
            var quaternionSerializer = new PropertyStateJsonSerializer<Quaternion, Quaternion>(JsonSerializationDict);

            var json = new JsonObject();
            json.Append(new JsonPair(IdKey, state.Id));
            json.Append(new JsonPair(DontLerpKey, true));
            json.Append(new JsonPair(PositionKey, vector3Serializer.SerializeAbsolute(state.Position)));
            json.Append(new JsonPair(RotationKey, quaternionSerializer.SerializeAbsolute(state.Rotation)));
            json.Append(new JsonPair(VelocityKey, vector3Serializer.SerializeAbsolute(state.Velocity)));
            return json;
        }

        public IJsonObject SerializeRelative(CubelingState state)
        {
            var vector3Serializer = new PropertyStateJsonSerializer<Vector3, Vector3>(JsonSerializationDict);
            var quaternionSerializer = new PropertyStateJsonSerializer<Quaternion, Quaternion>(JsonSerializationDict);

            var json = new JsonObject();
            json.Append(new JsonPair(IdKey, state.Id));
            if (state.DontLerp.HasValue) json.Append(new JsonPair(DontLerpKey, true));
            if (state.Position.Changed) json.Append(new JsonPair(PositionKey, vector3Serializer.SerializeRelative(state.Position)));
            if (state.Rotation.Changed) json.Append(new JsonPair(RotationKey, quaternionSerializer.SerializeRelative(state.Rotation)));
            if (state.Velocity.Changed) json.Append(new JsonPair(VelocityKey, vector3Serializer.SerializeRelative(state.Velocity)));
            return json;
        }
    }
}
