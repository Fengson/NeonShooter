using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Cube;
using NeonShooter.Players;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class PlayerState : IState
    {
        public const string DontLerpKey = "DontLerp";
        public const string CellsInStructureKey = "CellsInStructure";
        public const string PositionKey = "Position";
        public const string RotationsKey = "Rotations";
        public const string SelectedWeaponIdKey = "SelectedWeaponId";
        public const string ContinousFireKey = "ContinousFire";
        public const string LaunchedProjectilesKey = "LaunchedProjectiles";

        public static PlayerState FromJSONNode(JSONNode jsonNode, EnemyPlayer enemy)
        {
            var playerState = new PlayerState();

            if (jsonNode != null)
            {
                var jsonBrandNew = jsonNode[DontLerpKey];
                playerState.DontLerp = jsonBrandNew == null ? false : jsonBrandNew.AsBool;

                var jsonCellsInStructure = jsonNode[PlayerState.CellsInStructureKey];
                playerState.CellsInStructure = ListState<IVector3, CellState>.FromJSONNode(
                    jsonCellsInStructure, js => new CellState(js.AsIVector3()),
                    cs => cs.Position, cs => cs.Position);

                var jsonPosition = jsonNode[PositionKey];
                playerState.Position = PropertyState<Vector3, Vector3>.FromJSONNode(
                    jsonPosition, js => js.AsVector3(), (p, s) => p.Value = s);

                var jsonRotations = jsonNode[RotationsKey];
                playerState.Rotations = PropertyState<Vector2, Vector2>.FromJSONNode(
                    jsonRotations, js => js.AsVector2(), (p, s) => p.Value = s);

                var jsonSelectedWeaponId = jsonNode[SelectedWeaponIdKey];
                playerState.SelectedWeaponId = PropertyState<Weapon, int>.FromJSONNode(
                    jsonSelectedWeaponId, js => js.AsInt, (p, s) => p.Value = enemy.WeaponsById[s]);

                var jsonContinousFire = jsonNode[ContinousFireKey];
                playerState.ContinousFire = PropertyState<bool, bool>.FromJSONNode(
                    jsonContinousFire, js => js.AsBool, (p, s) => p.Value = s);

                var jsonLaunchedProjectiles = jsonNode[LaunchedProjectilesKey];
                playerState.LaunchedProjectiles = ListState<BaseProjectile, ProjectileState>.FromJSONNode(
                    jsonLaunchedProjectiles, js => ProjectileState.FromJSONNode(js, enemy),
                    ps => ps.CreateProjectile(), ps => enemy.ProjectilesById[ps.Id]);
            }

            return playerState;
        }

        public bool Changed
        {
            get
            {
                return
                    DontLerp.HasValue ||
                    CellsInStructure.Changed ||
                    Position.Changed ||
                    Rotations.Changed ||
                    SelectedWeaponId.Changed ||
                    ContinousFire.Changed ||
                    LaunchedProjectiles.Changed;
            }
        }

        public IJsonObject RelativeJson
        {
            get
            {
                var json = new JsonObject();
                if (DontLerp.HasValue) json.Append(new JsonPair(DontLerpKey, DontLerp.Value));
                if (CellsInStructure.Changed) json.Append(new JsonPair(CellsInStructureKey, CellsInStructure.RelativeJson));
                if (Position.Changed) json.Append(new JsonPair(PositionKey, Position.RelativeJson));
                if (Rotations.Changed) json.Append(new JsonPair(RotationsKey, Rotations.RelativeJson));
                if (ContinousFire.Changed) json.Append(new JsonPair(ContinousFireKey, ContinousFire.RelativeJson));
                if (SelectedWeaponId.Changed) json.Append(new JsonPair(SelectedWeaponIdKey, SelectedWeaponId.RelativeJson));
                if (LaunchedProjectiles.Changed) json.Append(new JsonPair(LaunchedProjectilesKey, LaunchedProjectiles.RelativeJson));
                return json;
            }
        }

        public IJsonObject AbsoluteJson
        {
            get
            {
                var json = new JsonObject();
                json.Append(new JsonPair(DontLerpKey, true));
                json.Append(new JsonPair(CellsInStructureKey, CellsInStructure.AbsoluteJson));
                json.Append(new JsonPair(PositionKey, Position.AbsoluteJson));
                json.Append(new JsonPair(RotationsKey, Rotations.AbsoluteJson));
                json.Append(new JsonPair(ContinousFireKey, ContinousFire.AbsoluteJson));
                json.Append(new JsonPair(SelectedWeaponIdKey, SelectedWeaponId.AbsoluteJson));
                json.Append(new JsonPair(LaunchedProjectilesKey, LaunchedProjectiles.AbsoluteJson));
                return json;
            }
        }

        public bool? DontLerp { get; private set; }

        public ListState<IVector3, CellState> CellsInStructure { get; private set; }

        public PropertyState<Vector3, Vector3> Position { get; private set; }
        public PropertyState<Vector2, Vector2> Rotations { get; private set; }

        public PropertyState<Weapon, int> SelectedWeaponId { get; private set; }
        public PropertyState<bool, bool> ContinousFire { get; private set; }

        public ListState<BaseProjectile, ProjectileState> LaunchedProjectiles { get; private set; }

        private PlayerState()
        {
        }

        public PlayerState(Player player)
        {
            DontLerp = true;

            CellsInStructure = new ListState<IVector3, CellState>(player.CellsInStructure, v => new CellState(v));
            Position = new PropertyState<Vector3, Vector3>(player.Position, p => p, s => s.ToJson());
            Rotations = new PropertyState<Vector2, Vector2>(player.Rotations, p => p, s => s.ToJson());
            SelectedWeaponId = new PropertyState<Weapon, int>(player.SelectedWeapon, p => p.Id, s => new JsonValue(s));
            ContinousFire = new PropertyState<bool, bool>(player.ContinousFire, p => p, s => new JsonValue(s));
            LaunchedProjectiles = new ListState<BaseProjectile, ProjectileState>(
                player.LaunchedProjectiles, p => new ProjectileState((Projectile)p));
        }

        public void ClearChanges()
        {
            DontLerp = null;

            CellsInStructure.ClearChanges();
            Position.ClearChanges();
            Rotations.ClearChanges();
            SelectedWeaponId.ClearChanges();
            ContinousFire.ClearChanges();
            LaunchedProjectiles.ClearChanges();
        }

        public void ApplyTo(object o)
        {
            var player = o as EnemyPlayer;
            if (player != null) ApplyTo(player);
        }

        public void ApplyTo(EnemyPlayer enemy)
        {
            enemy.DontLerp = DontLerp.HasValue && DontLerp.Value;
            if (CellsInStructure.Changed) CellsInStructure.ApplyTo(enemy.CellsInStructure);
            if (Position.Changed) Position.ApplyTo(enemy.Position);
            if (Rotations.Changed) Rotations.ApplyTo(enemy.Rotations);
            if (SelectedWeaponId.Changed) SelectedWeaponId.ApplyTo(enemy.SelectedWeapon);
            if (ContinousFire.Changed) ContinousFire.ApplyTo(enemy.ContinousFire);
            if (LaunchedProjectiles.Changed) LaunchedProjectiles.ApplyTo(enemy.LaunchedProjectiles);
        }
    }
}
