using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Cube;
using NeonShooter.Players.Weapons;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class CubelingState : IState
    {
        public const string IdKey = "Id";
        public const string DontLerpKey = "DontLerp";
        public const string PositionKey = "Position";
        public const string RotationKey = "Rotation";

        public static CubelingState FromJSONNode(JSONNode jsonNode, EnemyPlayer enemy)
        {
            var cubelingState = new CubelingState();

            cubelingState.parentEnemy = enemy;

            if (jsonNode != null)
            {
                cubelingState.Id = jsonNode[IdKey].AsLong();

                var jsonDontLerp = jsonNode[DontLerpKey];
                cubelingState.DontLerp = jsonDontLerp == null ? false : jsonDontLerp.AsBool;

                var jsonPosition = jsonNode[PositionKey];
                cubelingState.Position = PropertyState<Vector3, Vector3>.FromJSONNode(
                    jsonPosition, js => js.AsVector3(), (p, s) => p.Value = s);

                var jsonRotations = jsonNode[RotationKey];
                cubelingState.Rotation = PropertyState<Quaternion, Quaternion>.FromJSONNode(
                    jsonRotations, js => js.AsQuaternion(), (p, s) => p.Value = s);
            }

            return cubelingState;
        }

        public bool Changed
        {
            get
            {
                return
                    DontLerp.HasValue ||
                    Position.Changed ||
                    Rotation.Changed;
            }
        }

        public IJsonObject RelativeJson
        {
            get
            {
                var json = new JsonObject();
                json.Append(new JsonPair(IdKey, Id));
                if (DontLerp.HasValue) json.Append(new JsonPair(DontLerpKey, DontLerp.Value));
                if (Position.Changed) json.Append(new JsonPair(PositionKey, Position.RelativeJson));
                if (Rotation.Changed) json.Append(new JsonPair(RotationKey, Rotation.RelativeJson));
                return json;
            }
        }

        public IJsonObject AbsoluteJson
        {
            get
            {
                var json = new JsonObject();
                json.Append(new JsonPair(IdKey, Id));
                json.Append(new JsonPair(DontLerpKey, true));
                json.Append(new JsonPair(PositionKey, Position.AbsoluteJson));
                json.Append(new JsonPair(RotationKey, Rotation.AbsoluteJson));
                return json;
            }
        }

        EnemyPlayer parentEnemy;

        public long Id { get; private set; }

        public bool? DontLerp { get; private set; }

        public PropertyState<Vector3, Vector3> Position { get; private set; }
        public PropertyState<Quaternion, Quaternion> Rotation { get; private set; }
        
        private CubelingState()
        {
        }

        public CubelingState(Cubeling cubeling)
        {
            Id = cubeling.Id;

            DontLerp = true;

            Position = new PropertyState<Vector3, Vector3>(cubeling.Position, p => p, s => s.ToJson());
            Rotation = new PropertyState<Quaternion, Quaternion>(cubeling.Rotation, p => p, s => s.ToJson());
        }

        public void ClearChanges()
        {
            DontLerp = null;

            Position.ClearChanges();
            Rotation.ClearChanges();
        }

        public void ApplyTo(object o)
        {
            var cubeling = o as EnemyCubeling;
            if (cubeling != null) ApplyTo(cubeling);
        }

        public void ApplyTo(EnemyCubeling cubeling)
        {
            cubeling.DontLerp = DontLerp.HasValue && DontLerp.Value;
            if (Position.Changed) Position.ApplyTo(cubeling.Position);
            if (Rotation.Changed) Rotation.ApplyTo(cubeling.Rotation);
        }

        public EnemyCubeling CreateCubeling()
        {
            if (parentEnemy == null || !DontLerp.HasValue || DontLerp.Value == false) return null;

            var cubelingObject = parentEnemy.SpawnCubeling(Position.Value, Rotation.Value);
            var cubeling = cubelingObject.GetComponent<EnemyCubeling>();
            cubeling.Id = Id;
            parentEnemy.CubelingsById[Id] = cubeling;
            return cubeling;
        }
    }
}
