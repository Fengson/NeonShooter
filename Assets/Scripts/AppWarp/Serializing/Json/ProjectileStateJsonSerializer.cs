using UnityEngine;
using NeonShooter.AppWarp.States;
using NeonShooter.AppWarp.Json;

namespace NeonShooter.AppWarp.Serializing.Json
{
    public class ProjectileStateJsonSerializer : BaseJsonSerializer
    {
        public const string IdKey = "Id";
        public const string DontLerpKey = "DontLerp";
        public const string ParentWeaponIdKey = "ParentWeaponId";
        public const string PositionKey = "Position";
        public const string RotationKey = "Rotation";

        public ProjectileStateJsonSerializer(JsonSerializationDict jsonSerializationDict)
            : base(jsonSerializationDict)
        {
        }

        public override IJsonObject SerializeAbsolute(IState state)
        {
            if (!(state is ProjectileState))
                throw new System.Exception("Parameter state must be of type ProjectileState.");
            return SerializeAbsolute((ProjectileState)state);
        }

        public override IJsonObject SerializeRelative(IState state)
        {
            if (!(state is ProjectileState))
                throw new System.Exception("Parameter state must be of type ProjectileState.");
            return SerializeRelative((ProjectileState)state);
        }

        public IJsonObject SerializeAbsolute(ProjectileState state)
        {
            var vector3Serializer = new PropertyStateJsonSerializer<Vector3, Vector3>(JsonSerializationDict);
            var quaternionSerializer = new PropertyStateJsonSerializer<Quaternion, Quaternion>(JsonSerializationDict);

            var json = new JsonObject();
            json.Append(new JsonPair(IdKey, state.Id));
            json.Append(new JsonPair(DontLerpKey, true));
            json.Append(new JsonPair(ParentWeaponIdKey, state.ParentWeaponId.AbsoluteJson));
            json.Append(new JsonPair(PositionKey, vector3Serializer.SerializeAbsolute(state.Position)));
            json.Append(new JsonPair(RotationKey, quaternionSerializer.SerializeAbsolute(state.Rotation)));
            return json;
        }

        public IJsonObject SerializeRelative(ProjectileState state)
        {
            var vector3Serializer = new PropertyStateJsonSerializer<Vector3, Vector3>(JsonSerializationDict);
            var quaternionSerializer = new PropertyStateJsonSerializer<Quaternion, Quaternion>(JsonSerializationDict);

            var json = new JsonObject();
            json.Append(new JsonPair(IdKey, state.Id));
            if (state.DontLerp.HasValue) json.Append(new JsonPair(DontLerpKey, state.DontLerp.Value));
            if (state.ParentWeaponId.Changed) json.Append(new JsonPair(ParentWeaponIdKey, state.ParentWeaponId.RelativeJson));
            if (state.Position.Changed) json.Append(new JsonPair(PositionKey, vector3Serializer.SerializeRelative(state.Position)));
            if (state.Rotation.Changed) json.Append(new JsonPair(RotationKey, quaternionSerializer.SerializeRelative(state.Rotation)));
            return json;
        }
    }
}
