using NeonShooter;
using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;
using System.Collections;

public class RailGun : Weapon
{
    public override DamageEffect DamageEffect { get { return DamageEffect.Destruction; } }

    public RailGun()
        : base(150, 900, 0, 35)
    {
    }

    public override void shoot(Player shooter, int paidCost)
    {
        shootSound(shooter);
        Vector3 startingPosition = shooter.Position[null] + new Vector3(0, 0.8f, 0);
        Vector3 endingPosition =
            Vector3.MoveTowards(startingPosition, startingPosition + this.Reach * shooter.Direction[null], (int)Reach);
        RaycastHit hitInfo;
        bool enemyShot = false;
        if (shootLine(startingPosition, endingPosition, out hitInfo))
        {
            foreach (GameObject target in appwarp.enemies)
            {
                if (target.GetComponent<Collider>() == hitInfo.collider)
                {
                    enemyShot = true;
                    endingPosition = hitInfo.point;
                    shooter.enemyShot(this, target, Damage, paidCost);
                    GameObject projectile = createProjectileAndApplyForce(shooter, startingPosition, Color.green, paidCost);
                    shooter.StartCoroutine(hitAndDestroyProjectile(shooter, projectile, startingPosition, endingPosition));
                    break;
                }
            }
        }
        if (!enemyShot)
        {
            GameObject projectile = createProjectileAndApplyForce(shooter, startingPosition, Color.red, paidCost);
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

    public override Weapon nextWeapon()
    {
        return new VacuumWeapon();
    }

    public override void shootSound(Player player)
    {
        player.sounds[1].Play();
    }

    public override string getWeaponName()
    {
        return "Rail Gun";
    }
}
