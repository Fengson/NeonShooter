using NeonShooter.AppWarp;
using NeonShooter.Players.Cube;
using System.Collections;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public class RocketLauncher : ProjectileWeapon
    {
        public override int Id { get { return 2; } }

        public override CubelingSpawnEffect DamageEffect { get { return CubelingSpawnEffect.Scatter; } }
        public override FireType FireType { get { return FireType.Single; } }
        public override float CoolDownTime { get { return 0.3f; } }
        public override Color ProjectileColor { get { return Color.black; } }

        public float ExplosionReach { get; private set; }

        public override float ProjectileSpeed { get { return 40.0f; } }
        public override float ProjectileForceModifier { get { return 2.5f; } }
        public override int LifeRequiredToOwn { get { return 100; } }
        public override string Name { get { return "Rocket Launcher"; } }

        /**
        make sure to set Reach to whole number, so each flight part length is always 1
        */
        public RocketLauncher(BasePlayer player)
            : base(player, (int)300, 25, 50)
        {
            this.ExplosionReach = 5f;
        }

        public override void Shoot(Player shooter, int paidCost)
        {
            shootSound(shooter);
            Vector3 startingPosition = shooter.Position.Value + new Vector3(0, 0.8f, 0);
            Vector3 finalEndingPosition =
                Vector3.MoveTowards(startingPosition, startingPosition + Reach * shooter.Direction, (int)Reach);
            GameObject projectile = CreateProjectileAndApplyForce(shooter, startingPosition + 2 * shooter.Direction, ProjectileColor, paidCost);
            Debug.Log("Rocket " + projectile + " launched.\nSpeed: " + "point/" + ProjectileSpeed + "sec. Reach point: " + finalEndingPosition);
        }

        public override void shootSound(Player player)
        {
            if (player.sounds[1] != null) player.sounds[1].Play();
        }

    }

}