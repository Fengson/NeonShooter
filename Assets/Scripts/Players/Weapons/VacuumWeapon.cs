using NeonShooter.AppWarp;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public class VacuumWeapon : Weapon
    {
        private float damageLeftover;
        private GameObject vacuumCone;
        private BasePlayer shooter;

        public override int Id { get { return 0; } }
        public override DamageEffect DamageEffect { get { return DamageEffect.Suction; } }
        public override FireType FireType { get { return FireType.Continous; } }
        public override float CoolDownTime { get { return 0; } }

        public System.Func<float> ConeXRotation { get; set; }

        public VacuumWeapon() : base(50, 10, 10 * Mathf.Deg2Rad, 0)
        {
            ConeXRotation = () => shooter.Rotations.Value.x;
        }

        public override void Update()
        {
            base.Update();

            if (shooter != null && vacuumCone != null)
                vacuumCone.transform.localRotation = Quaternion.Euler(ConeXRotation(), 0, 0);
        }

        public override void OnShootStart(BasePlayer shooter)
        {
            this.shooter = shooter;

            damageLeftover = 0;
            vacuumCone = Object.Instantiate(shooter.vacuumConePrefab);
            GameObjectHelper.SetParentDontMessUpCoords(vacuumCone, shooter.gameObject);
            var xyScale = Reach * Mathf.Tan(ConeAngleRadians);
            vacuumCone.transform.localScale = new Vector3(xyScale, xyScale, Reach);
            vacuumCone.transform.localRotation = Quaternion.Euler(shooter.Rotations.Value.x, 0, 0);
        }

        public override void shoot(Player shooter, int paidCost)
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

        /**
        suction speed in this case
        */
        public override float projectileSpeed()
        {
            return 1.0f;
        }

        public override float projectileForceModifier()
        {
            return 100.0f;
        }

        public override int lifeRequiredToOwn()
        {
            return int.MinValue;
        }

        public override void shootSound(Player player)
        {
            if (player.sounds[1] != null) player.sounds[1].Play();
        }

        public override string getWeaponName()
        {
            return "Vacuum";
        }
    }
}