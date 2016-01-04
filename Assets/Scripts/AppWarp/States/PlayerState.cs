using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Cube;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using System.IO;
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
        public const string SpawnedCubelingsKey = "SpawnedCubelings";

        public bool IsNewcomer { get; private set; }

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
                    LaunchedProjectiles.Changed ||
                    SpawnedCubelings.Changed;
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
                if (SelectedWeaponId.Changed) json.Append(new JsonPair(SelectedWeaponIdKey, SelectedWeaponId.RelativeJson));
                if (ContinousFire.Changed) json.Append(new JsonPair(ContinousFireKey, ContinousFire.RelativeJson));
                if (LaunchedProjectiles.Changed) json.Append(new JsonPair(LaunchedProjectilesKey, LaunchedProjectiles.RelativeJson));
                if (SpawnedCubelings.Changed) json.Append(new JsonPair(SpawnedCubelingsKey, SpawnedCubelings.RelativeJson));
                return json;
            }
        }

        //public int AbsoluteBinarySize
        //{
        //    get
        //    {
        //        return 6 +
        //            1 +
        //            CellsInStructure.AbsoluteBinarySize +
        //            3 * 4 +
        //            2 * 4 +
        //            4 +
        //            1 +
        //            LaunchedProjectiles.AbsoluteBinarySize +
        //            SpawnedCubelings.AbsoluteBinarySize;
        //    }
        //}

        public IJsonObject AbsoluteJson
        {
            get
            {
                var json = new JsonObject();
                json.Append(new JsonPair(DontLerpKey, true));
                json.Append(new JsonPair(CellsInStructureKey, CellsInStructure.AbsoluteJson));
                json.Append(new JsonPair(PositionKey, Position.AbsoluteJson));
                json.Append(new JsonPair(RotationsKey, Rotations.AbsoluteJson));
                json.Append(new JsonPair(SelectedWeaponIdKey, SelectedWeaponId.AbsoluteJson));
                json.Append(new JsonPair(ContinousFireKey, ContinousFire.AbsoluteJson));
                json.Append(new JsonPair(LaunchedProjectilesKey, LaunchedProjectiles.AbsoluteJson));
                json.Append(new JsonPair(SpawnedCubelingsKey, SpawnedCubelings.AbsoluteJson));
                return json;
            }
        }

        public bool? DontLerp { get; private set; }

        public BaseListState<IVector3, CellState> CellsInStructure { get; private set; }

        public BasePropertyState<Vector3, Vector3> Position { get; private set; }
        public BasePropertyState<Vector2, Vector2> Rotations { get; private set; }

        public BasePropertyState<Weapon, int> SelectedWeaponId { get; private set; }
        public BasePropertyState<bool, bool> ContinousFire { get; private set; }

        public BaseListState<BaseProjectile, ProjectileState> LaunchedProjectiles { get; private set; }
        public BaseListState<BaseCubeling, CubelingState> SpawnedCubelings { get; private set; }

        public PlayerState(JSONNode jsonNode, EnemyPlayer enemy)
        {
            if (jsonNode != null)
            {
                var jsonBrandNew = jsonNode[DontLerpKey];
                DontLerp = jsonBrandNew == null ? false : jsonBrandNew.AsBool;

                //var jsonCellsInStructure = jsonNode[PlayerState.CellsInStructureKey];
                //CellsInStructure = new ListState<IVector3, CellState>(
                //    jsonCellsInStructure, js => new CellState(js.AsIVector3()),
                //    cs => cs.Position, cs => cs.Position);

                var jsonCellsInStructure = jsonNode[PlayerState.CellsInStructureKey];
                CellsInStructure = new BaseListState<IVector3, CellState>(
                    jsonCellsInStructure, js => new CellState(js),
                    cs => cs.Position, cs => cs.Position);

                var jsonPosition = jsonNode[PositionKey];
                Position = new BasePropertyState<Vector3, Vector3>(
                    jsonPosition, js => js.AsVector3(), (p, s) => p.Value = s);

                var jsonRotations = jsonNode[RotationsKey];
                Rotations = new BasePropertyState<Vector2, Vector2>(
                    jsonRotations, js => js.AsVector2(), (p, s) => p.Value = s);

                var jsonSelectedWeaponId = jsonNode[SelectedWeaponIdKey];
                SelectedWeaponId = new BasePropertyState<Weapon, int>(
                    jsonSelectedWeaponId, js => js.AsInt, (p, s) => p.Value = enemy.WeaponsById[s]);

                var jsonContinousFire = jsonNode[ContinousFireKey];
                ContinousFire = new BasePropertyState<bool, bool>(
                    jsonContinousFire, js => js.AsBool, (p, s) => p.Value = s);

                var jsonLaunchedProjectiles = jsonNode[LaunchedProjectilesKey];
                LaunchedProjectiles = new BaseListState<BaseProjectile, ProjectileState>(
                    jsonLaunchedProjectiles, js => new ProjectileState(js, enemy),
                    ps => ps.CreateProjectile(), ps => enemy.ProjectilesById[ps.Id]);

                var jsonSpawnedCubelings = jsonNode[SpawnedCubelingsKey];
                SpawnedCubelings = new BaseListState<BaseCubeling, CubelingState>(
                    jsonSpawnedCubelings, js => new CubelingState(js, enemy),
                    cs => cs.CreateCubeling(), cs => enemy.CubelingsById[cs.Id]);
            }
        }

        //public PlayerState(BinaryReader br, EnemyPlayer enemy)
        //{
        //    bool hasCellsInStructure = br.ReadBoolean();
        //    bool hasPosition = br.ReadBoolean();
        //    bool hasRotations = br.ReadBoolean();
        //    bool hasSelectedWeaponIdKey = br.ReadBoolean();
        //    bool hasLaunchedProjectiles = br.ReadBoolean();
        //    bool hasSpawnedCubelings = br.ReadBoolean();

        //    DontLerp = br.ReadBoolean();
        //    Debug.Log("DontLerp = " + DontLerp);

        //    Debug.Log("hasCellsInStructure = " + hasCellsInStructure);
        //    CellsInStructure = new ListState<IVector3, CellState>(hasCellsInStructure,
        //        br, _br => new CellState(_br), cs => cs.Position, cs => cs.Position);
        //    if (hasCellsInStructure) Debug.Log(CellsInStructure.RelativeJson.BuildString());

        //    Debug.Log("hasPosition = " + hasPosition);
        //    Position = new PropertyVector3State<Vector3>(hasPosition,
        //        br, _br => _br.ReadVector3(), (p, s) => p.Value = s);
        //    if (hasPosition) Debug.Log(Position.Value);

        //    Debug.Log("hasRotations = " + hasRotations);
        //    Rotations = new PropertyVector2State<Vector2>(hasRotations,
        //        br, _br => _br.ReadVector2(), (p, s) => p.Value = s);
        //    if (hasRotations) Debug.Log(Rotations.Value);

        //    Debug.Log("hasSelectedWeaponIdKey = " + hasSelectedWeaponIdKey);
        //    SelectedWeaponId = new PropertyCustomBinaryState<Weapon, int>(hasSelectedWeaponIdKey,
        //        br, _br => _br.ReadInt32(), (p, s) => p.Value = enemy.WeaponsById[s]);
        //    if (hasSelectedWeaponIdKey) Debug.Log(SelectedWeaponId.Value);

        //    //Debug.Log(br.BaseStream.Position);

        //    ContinousFire = new PropertyCustomBinaryState<bool, bool>(true,
        //        br, _br => _br.ReadBoolean(), (p, s) => p.Value = s);
        //    Debug.Log("ContinousFire = " + ContinousFire.Value);

        //    Debug.Log("hasLaunchedProjectiles = " + hasLaunchedProjectiles);
        //    LaunchedProjectiles = new ListState<BaseProjectile, ProjectileState>(hasLaunchedProjectiles,
        //        br, _br => new ProjectileState(br, enemy),
        //        ps => ps.CreateProjectile(), ps => enemy.ProjectilesById[ps.Id]);
        //    //if (hasLaunchedProjectiles) Debug.Log(LaunchedProjectiles.RelativeJson.BuildString());

        //    Debug.Log("hasSpawnedCubelings = " + hasSpawnedCubelings);
        //    SpawnedCubelings = new ListState<BaseCubeling, CubelingState>(hasSpawnedCubelings,
        //        br, _br => new CubelingState(br, enemy),
        //        cs => cs.CreateCubeling(), ps => enemy.CubelingsById[ps.Id]);
        //    //if (hasSpawnedCubelings) Debug.Log(SpawnedCubelings.RelativeJson.BuildString());
        //}

        public PlayerState(Player player)
        {
            IsNewcomer = true;
            DontLerp = true;

            CellsInStructure = new BaseListState<IVector3, CellState>(player.CellsInStructure, v => new CellState(v));
            Position = new BasePropertyState<Vector3, Vector3>(player.Position, p => p, s => s.ToJson());
            Rotations = new BasePropertyState<Vector2, Vector2>(player.Rotations, p => p, s => s.ToJson());
            SelectedWeaponId = new BasePropertyState<Weapon, int>(
                player.SelectedWeapon, p => p.Id, s => new JsonValue(s));//, (bw, i) => bw.Write(i));
            ContinousFire = new BasePropertyState<bool, bool>(
                player.ContinousFire, p => p, s => new JsonValue(s));//, (bw, b) => bw.Write(b));
            LaunchedProjectiles = new BaseListState<BaseProjectile, ProjectileState>(
                player.LaunchedProjectiles, p => new ProjectileState((Projectile)p));
            SpawnedCubelings = new BaseListState<BaseCubeling, CubelingState>(
                player.SpawnedCubelings, p => new CubelingState((Cubeling)p));
        }

        //public void WriteRelativeBinaryTo(BinaryWriter bw)
        //{
        //    bool hasCellsInStructure = CellsInStructure.Changed;
        //    bool hasPosition = Position.Changed;
        //    bool hasRotations = Rotations.Changed;
        //    bool hasSelectedWeaponIdKey = SelectedWeaponId.Changed;
        //    bool hasLaunchedProjectiles = LaunchedProjectiles.Changed;
        //    bool hasSpawnedCubelings = SpawnedCubelings.Changed;

        //    bw.Write(hasCellsInStructure);
        //    bw.Write(hasPosition);
        //    bw.Write(hasRotations);
        //    bw.Write(hasSelectedWeaponIdKey);
        //    bw.Write(hasLaunchedProjectiles);
        //    bw.Write(hasSpawnedCubelings);

        //    bw.Write(DontLerp.HasValue && DontLerp.Value);

        //    if (hasCellsInStructure) bw.WriteRelative(CellsInStructure);
        //    if (hasPosition) bw.WriteRelative(Position);
        //    if (hasRotations) bw.WriteRelative(Rotations);
        //    if (hasSelectedWeaponIdKey) bw.WriteRelative(SelectedWeaponId);

        //    bw.WriteAbsolute(ContinousFire);

        //    if (hasLaunchedProjectiles) bw.WriteRelative(LaunchedProjectiles);
        //    if (hasSpawnedCubelings)  bw.WriteRelative(SpawnedCubelings);
        //}

        //public void WriteAbsoluteBinaryTo(BinaryWriter bw)
        //{
        //    bw.Write(true);
        //    bw.Write(true);
        //    bw.Write(true);
        //    bw.Write(true);
        //    bw.Write(true);
        //    bw.Write(true);

        //    bw.Write(DontLerp.HasValue && DontLerp.Value);
        //    bw.WriteAbsolute(CellsInStructure);
        //    bw.WriteAbsolute(Position);
        //    bw.WriteAbsolute(Rotations);
        //    bw.WriteAbsolute(SelectedWeaponId);
        //    bw.WriteAbsolute(ContinousFire);
        //    bw.WriteAbsolute(LaunchedProjectiles);
        //    bw.WriteAbsolute(SpawnedCubelings);
        //}

        public void ClearChanges()
        {
            DontLerp = null;
            IsNewcomer = false;

            CellsInStructure.ClearChanges();
            Position.ClearChanges();
            Rotations.ClearChanges();
            SelectedWeaponId.ClearChanges();
            ContinousFire.ClearChanges();
            LaunchedProjectiles.ClearChanges();
            SpawnedCubelings.ClearChanges();
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
            if (SpawnedCubelings.Changed) SpawnedCubelings.ApplyTo(enemy.SpawnedCubelings);
        }
    }
}
