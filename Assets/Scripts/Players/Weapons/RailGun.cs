using NeonShooter.AppWarp;
using NeonShooter.Players.Cube;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public class RailGun : Weapon
    {
        public override int Id { get { return 1; } }
        public override CubelingSpawnEffect DamageEffect { get { return CubelingSpawnEffect.Scatter; } }
        public override FireType FireType { get { return FireType.Single; } }
        public override float CoolDownTime { get { return 1; } }
        public override Color ProjectileColor { get { return Color.green; } }

        public RailGun()
            : base(150, 900, 0, 35)
        {
        }

        public override void shoot(Player shooter, int paidCost)
        {
            shootSound(shooter);
            Vector3 startingPosition = shooter.Position.Value + new Vector3(0, 0.8f, 0);
            Vector3 endingPosition =
                Vector3.MoveTowards(startingPosition, startingPosition + this.Reach * shooter.Direction, (int)Reach);
            RaycastHit hitInfo;
            bool enemyShot = false;
            var projectileStartPosition = startingPosition + 2 * shooter.Direction;
            if (shootLine(startingPosition, endingPosition, out hitInfo))
            {
                foreach (GameObject target in appwarp.enemies.Values)
                {
                    if (target.GetComponent<Collider>() == hitInfo.collider)
                    {
                        enemyShot = true;
                        endingPosition = hitInfo.point;
                        shooter.enemyShot(this, target, Damage, paidCost);
                        GameObject projectile = CreateProjectileAndApplyForce(shooter, projectileStartPosition, ProjectileColor, paidCost);
                        shooter.StartCoroutine(hitAndDestroyProjectile(shooter, projectile, startingPosition, endingPosition));
                        break;
                    }
                }
            }
            if (!enemyShot)
            {
                GameObject projectile = CreateProjectileAndApplyForce(shooter, projectileStartPosition, Color.red, paidCost);
                shooter.StartCoroutine(destroyProjectile(shooter, projectile));
            }
        }

        public override float projectileSpeed()
        {
            return 100.0f;
        }

        public override float projectileForceModifier()
        {
            return 100.0f;
        }

        public override int lifeRequiredToOwn()
        {
            return -100;
        }

        public override void shootSound(Player player)
        {
            if (player.sounds[2] != null) player.sounds[2].Play();
        }

        public override string getWeaponName()
        {
            return "Rail Gun";
        }
    }
}