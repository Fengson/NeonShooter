using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Weapons;
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

        public static ProjectileState FromJSONNode(JSONNode jsonNode, EnemyPlayer enemy)
        {
            var projectileState = new ProjectileState();
            
            projectileState.parentEnemy = enemy;

            if (jsonNode != null)
            {
                projectileState.Id = jsonNode[IdKey].AsLong();

                var jsonBrandNew = jsonNode[DontLerpKey];
                projectileState.DontLerp = jsonBrandNew == null ? false : jsonBrandNew.AsBool;

                var jsonParentWeaponId = jsonNode[ParentWeaponIdKey];
                projectileState.ParentWeaponId = ReadOnlyState<Projectile, int>.FromJSONNode(
                    jsonParentWeaponId, js => js.AsInt, (po, s) => po.ParentWeapon = enemy.WeaponsById[s]);

                var jsonPosition = jsonNode[PositionKey];
                projectileState.Position = PropertyState<Vector3, Vector3>.FromJSONNode(
                    jsonPosition, js => js.AsVector3(), (p, s) => p.Value = s);

                var jsonRotations = jsonNode[RotationKey];
                projectileState.Rotation = PropertyState<Quaternion, Quaternion>.FromJSONNode(
                    jsonRotations, js => js.AsQuaternion(), (p, s) => p.Value = s);
            }

            return projectileState;
        }

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

        EnemyPlayer parentEnemy;

        public long Id { get; private set; }

        public bool? DontLerp { get; private set; }
        public ReadOnlyState<Projectile, int> ParentWeaponId { get; private set; }

        public PropertyState<Vector3, Vector3> Position { get; private set; }
        public PropertyState<Quaternion, Quaternion> Rotation { get; private set; }
        
        private ProjectileState()
        {
        }

        public ProjectileState(Projectile projectile)
        {
            Id = projectile.Id;

            DontLerp = true;
            ParentWeaponId = new ReadOnlyState<Projectile,int>(projectile.ParentWeapon.Id);

            Position = new PropertyState<Vector3, Vector3>(projectile.Position, p => p, s => s.ToJson());
            Rotation = new PropertyState<Quaternion, Quaternion>(projectile.Rotation, p => p, s => s.ToJson());
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
