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

		private readonly double coneAngleCos;
		private readonly double coneAngleTan;
        
        public override int LifeRequiredToOwn { get { return int.MinValue; } }
        public override string Name { get { return "Vacuum"; } }

        public VacuumWeapon(BasePlayer player, double coneAngle = 10)
			: base(player, 50, 10, 0)
		{
			this.coneAngleCos = System.Math.Cos(MathHelper.Deg2Rad(coneAngle));
			this.coneAngleTan = System.Math.Tan(MathHelper.Deg2Rad(coneAngle));
		}

        public override void OnShootStart(BasePlayer shooter)
        {
            damageLeftover = 0;
            vacuumCone = Object.Instantiate(Globals.Instance.vacuumConePrefab);
			vacuumCone.transform.position = new Vector3 (0, -0.05f, 0);
            GameObjectHelper.SetParentDontMessUpCoords(vacuumCone, shooter.firstPersonCharacter);
			float xyScale = (float)(Reach * coneAngleTan);
            vacuumCone.transform.localScale = new Vector3(xyScale, xyScale, Reach);
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
                if (angle_cos > this.coneAngleCos)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(shooter.Position.Value, heading, out hit, this.Reach) && hit.collider.gameObject == target)
                    {
                        Debug.Log(hit.collider.name);
                        target.GetComponent<BasePlayer>().GotHit(shooter.gameObject, this, integerDamage);
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