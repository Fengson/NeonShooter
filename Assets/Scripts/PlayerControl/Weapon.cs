using System.Collections;
using UnityEngine;

namespace NeonShooter.PlayerControl
{
    public abstract class Weapon
    {
        public int Damage { get; private set; }
        public float Reach { get; private set; }
        public int AmmoCost { get; private set; }
        public double ConeAngleCos { get; private set; }

        public abstract DamageEffect DamageEffect { get; }

        public Weapon(int dmg, float reach, float cone_angle_radians, int ammo_cost)
        {
            this.Damage = dmg;
            this.Reach = reach;
            this.AmmoCost = ammo_cost;
            this.ConeAngleCos = Mathf.Cos(cone_angle_radians);
        }

        public abstract void shoot(Player shooter, int costPayed);

        public abstract string getWeaponName();

        protected bool shootLine(Vector3 rayStart, Vector3 rayEnd, out RaycastHit hitInfo)
        {
            return Physics.Linecast(rayStart, rayEnd, out hitInfo);
        }

        public abstract float projectileSpeed();

        public abstract float projectileForceModifier();

        public abstract int lifeRequiredToOwn();

        public abstract Weapon nextWeapon();

        public abstract void shootSound(Player player);

        public GameObject createProjectile(Player shooter, Vector3 startingPosition, Color color, int costPaid)
        {
            var projectile = shooter.instantiateProjectile(shooter.projectilePrefab);
            projectile.transform.position = startingPosition + 2 * shooter.Direction[null];
            projectile.GetComponent<Renderer>().material.color = color;
            var script = projectile.AddComponent<Projectile>();
            script.ParentWeapon = this;
            script.CubeValue = costPaid;
            return projectile;
        }

        public GameObject createProjectileAndApplyForce(Player shooter, Vector3 startingPosition, Color color, int costPaid)
        {
            var projectile = createProjectile(shooter, startingPosition, color, costPaid);
            projectile.GetComponent<ConstantForce>().force = shooter.Direction.Value.normalized * (projectileSpeed() * projectileForceModifier());
            projectile.GetComponent<ConstantForce>().torque = shooter.Direction[null] * 10;
            return projectile;
        }

        public GameObject createLaserProjectile(Player shooter, Vector3 startingPosition, Vector3 endingPosition)
        {
            GameObject projectile;

            projectile = shooter.instantiateProjectile(shooter.railGunShotPrefab);
            LineRenderer lineRenderer = projectile.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, startingPosition);
            lineRenderer.SetPosition(1, endingPosition);
            lineRenderer.SetColors(Color.red, Color.yellow);
            lineRenderer.SetWidth(0.2F, 0.2F);

            return projectile;
        }

        public IEnumerator destroyLaserProjectile(Player shooter, GameObject projectile)
        {
            yield return new WaitForSeconds(0.1f);

            shooter.destroyProjectile(projectile);
        }

        public IEnumerator destroyProjectile(Player shooter, GameObject projectile)
        {
            yield return new WaitForSeconds(Reach / projectileSpeed());

            shooter.destroyProjectile(projectile);
        }

        public IEnumerator hitAndDestroyProjectile(Player shooter, GameObject projectile, Vector3 startingPosition, Vector3 endingPosition)
        {
            yield return new WaitForSeconds((endingPosition - startingPosition).magnitude / projectileSpeed());
            shooter.destroyProjectile(projectile);
        }
    }
}
