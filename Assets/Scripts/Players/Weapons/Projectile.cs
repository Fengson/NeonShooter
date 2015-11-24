using NeonShooter.AppWarp;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public class Projectile : BaseProjectile
    {
        public int CubeValue { get; set; }

        private Projectile()
        {
            Id = System.DateTime.UtcNow.Ticks;
        }

        void OnTriggerEnter(Collider other)
        {
            Vector3 hitPoint = Position[Access];
            Destroy(this.gameObject);

            var player = ParentWeapon.Player as Player;
            if (player == null)
                throw new System.Exception(string.Format(
                    "Neonshooter.Players.Player type expected, but got {0} (this should never happen).",
                    ParentWeapon.Player == null ? "NULL" : ParentWeapon.Player.GetType().ToString()));

            bool enemyShot = false;

            foreach (GameObject target in appwarp.enemies.Values)
            {
                if (target.GetComponent<Collider>() == other)
                {
                    enemyShot = true;
                    player.enemyShot(ParentWeapon, other.gameObject, CubeValue);
                    break;
                }
            }

            if (!enemyShot)
            {
                player.SpawnCubelingsInPosition(hitPoint, CubeValue, ParentWeapon.DamageEffect);
            }
        }

        //TODO fix this method and move to separate class where it will be used
        void explodeMissile(Player shooter, Vector3 hitPoint, Collider directHitCollider, int paidCost)
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
                foreach (GameObject target in appwarp.enemies.Values)
                {
                    if (target.GetComponent<Collider>() == hitColliders[k].GetComponent<Collider>())
                    {
                        if (hitColliders[k] == directHitCollider)
                        {
                            shooter.enemyShot(ParentWeapon, target, paidCost);
                        }
                        else
                        {
                            float distanceFromExplosion = Vector3.Distance(hitPoint, hitColliders[k].ClosestPointOnBounds(hitPoint));
                            Debug.Log("Explosion distance from " + hitColliders[k].name + " is " + distanceFromExplosion);
                            if (distanceFromExplosion < rocketLauncher.ExplosionReach)
                            {
                                //only on clear shot we return shot cost
                                shooter.enemyShot(ParentWeapon, target, (int)(paidCost * (1.0f - (distanceFromExplosion / rocketLauncher.ExplosionReach))));
                            }
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
