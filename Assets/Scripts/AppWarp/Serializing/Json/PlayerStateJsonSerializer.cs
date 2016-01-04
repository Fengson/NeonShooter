using NeonShooter.AppWarp.Json;
using NeonShooter.AppWarp.States;
using NeonShooter.Players.Cube;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.AppWarp.Serializing.Json
{
    public class PlayerStateJsonSerializer : BaseJsonSerializer
    {
        public const string DontLerpKey = "DontLerp";
        public const string CellsInStructureKey = "CellsInStructure";
        public const string PositionKey = "Position";
        public const string RotationsKey = "Rotations";
        public const string SelectedWeaponIdKey = "SelectedWeaponId";
        public const string ContinousFireKey = "ContinousFire";
        public const string LaunchedProjectilesKey = "LaunchedProjectiles";
        public const string SpawnedCubelingsKey = "SpawnedCubelings";

        public PlayerStateJsonSerializer(JsonSerializationDict jsonSerializationDict)
            : base(jsonSerializationDict)
        {
        }

        public override IJsonObject SerializeAbsolute(IState state)
        {
            if (!(state is PlayerState))
                throw new System.Exception("Parameter state must be of type PlayerState.");
            return SerializeAbsolute((PlayerState)state);
        }

        public override IJsonObject SerializeRelative(IState state)
        {
            if (!(state is PlayerState))
                throw new System.Exception("Parameter state must be of type PlayerState.");
            return SerializeRelative((PlayerState)state);
        }

        public IJsonObject SerializeAbsolute(PlayerState state)
        {
            var cellListSerializer = new ListStateJsonSerializer<IVector3, CellState>(JsonSerializationDict);
            var vector3Serializer = new PropertyStateJsonSerializer<Vector3, Vector3>(JsonSerializationDict);
            var vector2Serializer = new PropertyStateJsonSerializer<Vector2, Vector2>(JsonSerializationDict);
            var weaponIdSerializer = new PropertyStateJsonSerializer<Weapon, int>(JsonSerializationDict);
            var boolSerializer = new PropertyStateJsonSerializer<bool, bool>(JsonSerializationDict);
            var projectileListSerializer = new ListStateJsonSerializer<BaseProjectile, ProjectileState>(JsonSerializationDict);
            var cubelingListSerializer = new ListStateJsonSerializer<BaseCubeling, CubelingState>(JsonSerializationDict);
            
            var json = new JsonObject();
            json.Append(new JsonPair(DontLerpKey, true));
            json.Append(new JsonPair(CellsInStructureKey, cellListSerializer.SerializeAbsolute(state.CellsInStructure)));
            json.Append(new JsonPair(PositionKey, vector3Serializer.SerializeAbsolute(state.Position)));
            json.Append(new JsonPair(RotationsKey, vector2Serializer.SerializeAbsolute(state.Rotations)));
            json.Append(new JsonPair(SelectedWeaponIdKey, weaponIdSerializer.SerializeAbsolute(state.SelectedWeaponId)));
            json.Append(new JsonPair(ContinousFireKey, boolSerializer.SerializeAbsolute(state.ContinousFire)));
            json.Append(new JsonPair(LaunchedProjectilesKey, projectileListSerializer.SerializeAbsolute(state.LaunchedProjectiles)));
            json.Append(new JsonPair(SpawnedCubelingsKey, cubelingListSerializer.SerializeAbsolute(state.SpawnedCubelings)));
            return json;
        }

        public IJsonObject SerializeRelative(PlayerState state)
        {
            var cellListSerializer = new ListStateJsonSerializer<IVector3, CellState>(JsonSerializationDict);
            var vector3Serializer = new PropertyStateJsonSerializer<Vector3, Vector3>(JsonSerializationDict);
            var vector2Serializer = new PropertyStateJsonSerializer<Vector2, Vector2>(JsonSerializationDict);
            var weaponIdSerializer = new PropertyStateJsonSerializer<Weapon, int>(JsonSerializationDict);
            var boolSerializer = new PropertyStateJsonSerializer<bool, bool>(JsonSerializationDict);
            var projectileListSerializer = new ListStateJsonSerializer<BaseProjectile, ProjectileState>(JsonSerializationDict);
            var cubelingListSerializer = new ListStateJsonSerializer<BaseCubeling, CubelingState>(JsonSerializationDict);

            var json = new JsonObject();
            if (state.DontLerp.HasValue) json.Append(new JsonPair(DontLerpKey, state.DontLerp.Value));
            if (state.CellsInStructure.Changed) json.Append(new JsonPair(CellsInStructureKey, cellListSerializer.SerializeRelative(state.CellsInStructure)));
            if (state.Position.Changed) json.Append(new JsonPair(PositionKey, vector3Serializer.SerializeRelative(state.Position)));
            if (state.Rotations.Changed) json.Append(new JsonPair(RotationsKey, vector2Serializer.SerializeRelative(state.Rotations)));
            if (state.SelectedWeaponId.Changed) json.Append(new JsonPair(SelectedWeaponIdKey, weaponIdSerializer.SerializeRelative(state.SelectedWeaponId)));
            if (state.ContinousFire.Changed) json.Append(new JsonPair(ContinousFireKey, boolSerializer.SerializeRelative(state.ContinousFire)));
            if (state.LaunchedProjectiles.Changed) json.Append(new JsonPair(LaunchedProjectilesKey, projectileListSerializer.SerializeRelative(state.LaunchedProjectiles)));
            if (state.SpawnedCubelings.Changed) json.Append(new JsonPair(SpawnedCubelingsKey, cubelingListSerializer.SerializeRelative(state.SpawnedCubelings)));
            return json;
        }
    }
}
