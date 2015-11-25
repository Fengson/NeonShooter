using NeonShooter.AppWarp;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public class Projectile : BaseProjectile
    {
        public int CubeValue { get; set; }

        protected Projectile()
        {
            Id = System.DateTime.UtcNow.Ticks;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            Vector3 hitPoint = Position[Access];
            Destroy(this.gameObject);

            var player = ParentWeapon.Player as Player;
            if (player == null)
                throw new System.Exception(string.Format(
                    "Neonshooter.Players.Player type expected, but got {0} (this should never happen).",
                    ParentWeapon.Player == null ? "NULL" : ParentWeapon.Player.GetType().ToString()));

            int baseDamage = ParentWeapon.Damage*CubeValue/ParentWeapon.AmmoCost;

            foreach (GameObject target in appwarp.enemies.Values)
            {
                if (target.GetComponent<Collider>() == other)
                {
                    player.enemyShot(ParentWeapon, other.gameObject, baseDamage);
                    break;
                }
            }

            player.SpawnCubelingsInPosition(hitPoint, CubeValue, ParentWeapon.DamageEffect);
        }

        protected override NotifyingProperty<Vector3> CreatePositionProperty()
        {
            return NotifyingProperty<Vector3>.PublicGetPrivateSet(Access);
        }

        protected override NotifyingProperty<Quaternion> CreateRotationProperty()
        {
            return NotifyingProperty<Quaternion>.PublicGetPrivateSet(Access);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            Position[Access] = transform.position;
            Rotation[Access] = transform.rotation;
        }
    }
}
