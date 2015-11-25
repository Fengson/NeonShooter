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

            bool enemyShot = false;

            int baseDamage = ParentWeapon.Damage*CubeValue/ParentWeapon.AmmoCost;

            foreach (GameObject target in appwarp.enemies.Values)
            {
                if (target.GetComponent<Collider>() == other)
                {
                    enemyShot = true;
                    player.enemyShot(ParentWeapon, other.gameObject, baseDamage);
                    break;
                }
            }


            //TODO change this ugly thing ;) move to separate class
            var rocketLauncher = ParentWeapon as RocketLauncher;
            if (rocketLauncher != null)
            {
                explodeMissile(player, hitPoint, other, baseDamage);
            }

            player.SpawnCubelingsInPosition(hitPoint, CubeValue, ParentWeapon.DamageEffect);
        }

        //TODO move to separate class where it will be used
        void explodeMissile(Player shooter, Vector3 hitPoint, Collider directHitCollider, int baseDamage)
        {
            var rocketLauncher = ParentWeapon as RocketLauncher;
            if (rocketLauncher == null)
                throw new System.Exception(string.Format(
                    "Neonshooter.Players.Weapons.RocketLauncher type expected, but got {0} (this should never happen).",
                    ParentWeapon == null ? "NULL" : ParentWeapon.GetType().ToString()));

            Collider[] hitColliders = Physics.OverlapSphere(hitPoint, rocketLauncher.ExplosionReach);
            int k = 0;
            while (k < hitColliders.Length)
            {
                if (hitColliders[k] == directHitCollider)
                {
                    k++;
                    continue;
                }
                foreach (GameObject target in appwarp.enemies.Values)
                {
                    if (target.GetComponent<Collider>() == hitColliders[k].GetComponent<Collider>())
                    {
                        float distanceFromExplosion = Vector3.Distance(hitPoint, hitColliders[k].ClosestPointOnBounds(hitPoint));
                        Debug.Log("Explosion distance from " + hitColliders[k].name + " is " + distanceFromExplosion);
                        if (distanceFromExplosion < rocketLauncher.ExplosionReach)
                        {
                            shooter.enemyShot(ParentWeapon, target, (int)(baseDamage * (1.0f - (distanceFromExplosion / rocketLauncher.ExplosionReach))));
                        }
                        break;
                    }
                }
                k++;
            }
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
