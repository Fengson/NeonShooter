using NeonShooter.AppWarp;
using NeonShooter.Players.Cube;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public class RailGun : RayWeapon
    {
        public override int Id { get { return 1; } }
        public override CubelingSpawnEffect DamageEffect { get { return CubelingSpawnEffect.Scatter; } }
        public override FireType FireType { get { return FireType.Single; } }
        public override float CoolDownTime { get { return 1; } }
        public override Color ProjectileColor { get { return Color.green; } }

        public override int LifeRequiredToOwn { get { return Mathf.Max(25, this.AmmoCost); } }
        public override string Name { get { return "Rail Gun"; } }

        public RailGun(BasePlayer player)
            : base(player, 150, 900, 35)
        {
        }

        public override void Shoot(Player shooter, int paidCost)
        {
            shootSound(shooter);
            Vector3 startingPosition = shooter.Position.Value + new Vector3(0, 0.8f, 0);
            Vector3 endingPosition =
                Vector3.MoveTowards(startingPosition, startingPosition + this.Reach * shooter.Direction, (int)Reach);
            RaycastHit hitInfo;
            bool enemyShot = false;
            var projectileStartPosition = startingPosition + 2 * shooter.Direction;

            bool airShot = !shootLine(startingPosition, endingPosition, out hitInfo);
            if (!airShot)
            {
                endingPosition = hitInfo.point;
                foreach (GameObject target in appwarp.enemies.Values)
                {
                    if (target.GetComponent<Collider>() == hitInfo.collider)
                    {
                        enemyShot = true;
                        shooter.enemyShot(this, target, paidCost);
                        break;
                    }
                }
            }

            //show line
            GameObject projectile = createLaserProjectile(shooter, projectileStartPosition, endingPosition);
            shooter.StartCoroutine(destroyLaserProjectile(shooter, projectile));

            //spawn cubelings payed at the end of line (or in front of player if airShot)
            if (airShot)
            {
                shooter.SpawnCubelingsInFrontOfPlayer(paidCost, DamageEffect);
            } else
            {
                shooter.SpawnCubelingsInPosition(endingPosition, paidCost, DamageEffect);
            }
        }

        public override void shootSound(Player player)
        {
            if (player.sounds[2] != null) player.sounds[2].Play();
        }
    }
}