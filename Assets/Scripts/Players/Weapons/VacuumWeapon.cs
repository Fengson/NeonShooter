using NeonShooter.AppWarp;
using NeonShooter.Players.Cube;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public class VacuumWeapon : Weapon
    {
        float damageLeftover;
        GameObject vacuumCone;

        public override int Id { get { return 0; } }
        public override CubelingSpawnEffect DamageEffect { get { return CubelingSpawnEffect.FlyToPlayer; } }
        public override FireType FireType { get { return FireType.Continous; } }
        public override float CoolDownTime { get { return 0; } }

        protected float ConeAngleRadians { get; private set; }
        public double ConeAngleCos { get; private set; }

        public System.Func<float> ConeXRotation { get; set; }
        
        /**
        suction speed in this case
        */
        //public override float ProjectileSpeed { get { return 1.0f; } }
        //public override float ProjectileForceModifier { get { return 100.0f; } }
        public override int LifeRequiredToOwn { get { return int.MinValue; } }
        public override string Name { get { return "Vacuum"; } }

        public VacuumWeapon()
            : base(50, 10, 0)
        {
            ConeAngleRadians = 10 * Mathf.Deg2Rad;
            this.ConeAngleCos = Mathf.Cos(ConeAngleRadians);
        }

        public override void OnShootStart(BasePlayer shooter)
        {
            damageLeftover = 0;
            vacuumCone = Object.Instantiate(Globals.Instance.vacuumConePrefab);
            GameObjectHelper.SetParentDontMessUpCoords(vacuumCone, shooter.firstPersonCharacter);
            var xyScale = Reach * Mathf.Tan(ConeAngleRadians);
            vacuumCone.transform.localScale = new Vector3(xyScale, xyScale, Reach);
            vacuumCone.transform.localRotation = Quaternion.Euler(shooter.Rotations.Value.x, 0, 0);
        }

        public override void Shoot(Player shooter, int paidCost)
        {
            float fractionalDamage = Damage * Time.deltaTime + damageLeftover;
            int integerDamage = (int)fractionalDamage;
            damageLeftover = fractionalDamage - integerDamage;
            if (integerDamage == 0) return;

            foreach (GameObject target in appwarp.enemies.Values)
            {
                Vector3 heading = (target.transform.position - shooter.Position.Value).normalized;
                double angle_cos = Vector3.Dot(heading, shooter.Direction.normalized);
                if (angle_cos > this.ConeAngleCos)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(shooter.Position.Value, heading, out hit, this.Reach) && hit.collider.gameObject == target)
                    {
                        Debug.Log(hit.collider.name);
                        shooter.enemyShot(this, target, integerDamage, paidCost);
                    }
                }
            }
        }

        public override void OnShootEnd()
        {
            Object.Destroy(vacuumCone);
        }

        public override void shootSound(Player player)
        {
            if (player.sounds[1] != null) player.sounds[1].Play();
        }
    }
}