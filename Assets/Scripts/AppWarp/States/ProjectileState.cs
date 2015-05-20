using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using System.IO;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class ProjectileState : IState
    {
        public const string IdKey = "Id";
        public const string DontLerpKey = "DontLerp";
        public const string ParentWeaponIdKey = "ParentWeaponId";
        public const string PositionKey = "Position";
        public const string RotationKey = "Rotation";

        public bool Changed
        {
            get
            {
                return
                    DontLerp.HasValue ||
                    ParentWeaponId.Changed ||
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
                if (ParentWeaponId.Changed) json.Append(new JsonPair(ParentWeaponIdKey, ParentWeaponId.RelativeJson));
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
                json.Append(new JsonPair(ParentWeaponIdKey, ParentWeaponId.AbsoluteJson));
                json.Append(new JsonPair(PositionKey, Position.AbsoluteJson));
                json.Append(new JsonPair(RotationKey, Rotation.AbsoluteJson));
                return json;
            }
        }

        public int AbsoluteBinarySize { get { return 3 + 8 + 1 + 4 + 3 * 4 + 4 * 4; } }

        EnemyPlayer parentEnemy;

        public long Id { get; private set; }

        public bool? DontLerp { get; private set; }
        public ReadOnlyCustomBinaryState<Projectile, int> ParentWeaponId { get; private set; }

        public PropertyVector3State<Vector3> Position { get; private set; }
        public PropertyQuaternionState<Quaternion> Rotation { get; private set; }

        public ProjectileState(JSONNode jsonNode, EnemyPlayer enemy)
        {
            parentEnemy = enemy;

            if (jsonNode != null)
            {
                Id = jsonNode[IdKey].AsLong();

                var jsonDontLerp = jsonNode[DontLerpKey];
                DontLerp = jsonDontLerp == null ? false : jsonDontLerp.AsBool;

                var jsonParentWeaponId = jsonNode[ParentWeaponIdKey];
                ParentWeaponId = new ReadOnlyCustomBinaryState<Projectile, int>(jsonParentWeaponId,
                    js => js.AsInt, (p, s) => p.ParentWeapon = enemy.WeaponsById[s]);

                var jsonPosition = jsonNode[PositionKey];
                Position = new PropertyVector3State<Vector3>(jsonPosition,
                    js => js.AsVector3(), (p, s) => p.Value = s);

                var jsonRotations = jsonNode[RotationKey];
                Rotation = new PropertyQuaternionState<Quaternion>(jsonRotations,
                    js => js.AsQuaternion(), (p, s) => p.Value = s);
            }
        }

        public ProjectileState(BinaryReader br, EnemyPlayer enemy)
        {
            parentEnemy = enemy;

            bool hasParentWeaponIdKey = br.ReadBoolean();
            bool hasPosition = br.ReadBoolean();
            bool hasRotation = br.ReadBoolean();

            Id = br.ReadInt64();
            DontLerp = br.ReadBoolean();

            ParentWeaponId = new ReadOnlyCustomBinaryState<Projectile, int>(hasParentWeaponIdKey,
                br, _br => _br.ReadInt32(), (p, s) => p.ParentWeapon = enemy.WeaponsById[s]);
            Position = new PropertyVector3State<Vector3>(hasPosition,
                br, _br => _br.ReadVector3(), (p, s) => p.Value = s);
            Rotation = new PropertyQuaternionState<Quaternion>(hasRotation,
                br, _br => _br.ReadQuaternion(), (p, s) => p.Value = s);
        }

        public ProjectileState(Projectile projectile)
        {
            Id = projectile.Id;

            DontLerp = true;
            ParentWeaponId = new ReadOnlyCustomBinaryState<Projectile, int>(
                projectile.ParentWeapon.Id, (bw, i) => bw.Write(i));

            Position = new PropertyVector3State<Vector3>(projectile.Position, p => p, s => s.ToJson());
            Rotation = new PropertyQuaternionState<Quaternion>(projectile.Rotation, p => p, s => s.ToJson());
        }

        public void WriteRelativeBinaryTo(BinaryWriter bw)
        {
            bool hasParentWeaponIdKey = ParentWeaponId.Changed;
            bool hasPosition = Position.Changed;
            bool hasRotation = Rotation.Changed;

            bw.Write(hasParentWeaponIdKey);
            bw.Write(hasPosition);
            bw.Write(hasRotation);

            bw.Write(Id);
            bw.Write(DontLerp.HasValue && DontLerp.Value);
            if (hasParentWeaponIdKey) bw.WriteRelative(ParentWeaponId);
            if (hasPosition) bw.WriteRelative(Position);
            if (hasRotation) bw.WriteRelative(Rotation);
        }

        public void WriteAbsoluteBinaryTo(BinaryWriter bw)
        {
            bw.Write(true);
            bw.Write(true);
            bw.Write(true);

            bw.Write(Id);
            bw.Write(true);
            bw.WriteAbsolute(ParentWeaponId);
            bw.WriteAbsolute(Position);
            bw.WriteAbsolute(Rotation);
        }

        public void ClearChanges()
        {
            DontLerp = null;
            ParentWeaponId.ClearChanges();

            Position.ClearChanges();
            Rotation.ClearChanges();
        }

        public void ApplyTo(object o)
        {
            var projectile = o as EnemyProjectile;
            if (projectile != null) ApplyTo(projectile);
        }

        public void ApplyTo(EnemyProjectile projectile)
        {
            projectile.DontLerp = DontLerp.HasValue && DontLerp.Value;
            if (ParentWeaponId.Changed) ParentWeaponId.ApplyTo(projectile);
            if (Position.Changed) Position.ApplyTo(projectile.Position);
            if (Rotation.Changed) Rotation.ApplyTo(projectile.Rotation);
        }

        public EnemyProjectile CreateProjectile()
        {
            if (parentEnemy == null || !DontLerp.HasValue || DontLerp.Value == false) return null;

            var weapon = parentEnemy.WeaponsById[ParentWeaponId.Value];
            var projectileObject = weapon.CreateProjectile<EnemyProjectile>(parentEnemy, Position.Value);
            projectileObject.transform.rotation = Rotation.Value;
            var projectile = projectileObject.GetComponent<EnemyProjectile>();
            projectile.Id = Id;
            parentEnemy.ProjectilesById[Id] = projectile;
            return projectile;
        }
    }
}
