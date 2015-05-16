using System.Collections;
using UnityEngine;

namespace NeonShooter.PlayerControl
{
    public abstract class Weapon
    {
        float currentCoolDownTime;

        public int Damage { get; private set; }
        public float Reach { get; private set; }
        public int AmmoCost { get; private set; }
        protected float ConeAngleRadians { get; private set; }
        public double ConeAngleCos { get; private set; }

        public abstract DamageEffect DamageEffect { get; }
        public abstract FireType FireType { get; }
        public abstract float CoolDownTime { get; }

        public Weapon(int dmg, float reach, float cone_angle_radians, int ammo_cost)
        {
            this.Damage = dmg;
            this.Reach = reach;
            this.AmmoCost = ammo_cost;
            ConeAngleRadians = cone_angle_radians;
            this.ConeAngleCos = Mathf.Cos(cone_angle_radians);
        }

        public virtual void Update()
        {
            currentCoolDownTime -= Time.deltaTime;
            if (currentCoolDownTime < 0) currentCoolDownTime = 0;
        }

        public void RaiseCooldown()
        {
            currentCoolDownTime = CoolDownTime;
        }

        public bool IsCoolingDown()
        {
            return currentCoolDownTime > 0;
        }

        public virtual void ShootStart(Player shooter)
        {
        }

        public abstract void shoot(Player shooter, int costPayed);

        public virtual void ShootEnd()
        {
        }

        public abstract string getWeaponName();

        protected bool shootLine(Vector3 rayStart, Vector3 rayEnd, out RaycastHit hitInfo)
        {
            return Physics.Linecast(rayStart, rayEnd, out hitInfo);
        }

        public abstract float projectileSpeed();

        public abstract float projectileForceModifier();

        public abstract int lifeRequiredToOwn();

        public abstract void shootSound(Player player);

        public GameObject createProjectile(Player shooter, Vector3 startingPosition, Color color, int costPaid)
        {
            var projectile = Object.Instantiate(shooter.projectilePrefab);
            projectile.transform.position = startingPosition + 2 * shooter.Direction.Value;
            projectile.GetComponent<Renderer>().material.color = color;
            var script = projectile.AddComponent<Projectile>();
            script.ParentWeapon = this;
            script.CubeValue = costPaid;
            shooter.LaunchedProjectiles.Add(script);
            return projectile;
        }

        public GameObject createProjectileAndApplyForce(Player shooter, Vector3 startingPosition, Color color, int costPaid)
        {
            var projectile = createProjectile(shooter, startingPosition, color, costPaid);
            projectile.GetComponent<ConstantForce>().force = shooter.Direction.Value.normalized * (projectileSpeed() * projectileForceModifier());
            projectile.GetComponent<ConstantForce>().torque = shooter.Direction.Value * 10;
            return projectile;
        }

        public GameObject createLaserProjectile(Player shooter, Vector3 startingPosition, Vector3 endingPosition)
        {
            GameObject projectile;

            projectile = Object.Instantiate(shooter.railGunShotPrefab);
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
            Object.Destroy(projectile);
        }

        public IEnumerator destroyProjectile(Player shooter, GameObject projectile)
        {
            yield return new WaitForSeconds(Reach / projectileSpeed());
            DestroyProjectileNow(shooter, projectile);
        }

        public IEnumerator hitAndDestroyProjectile(Player shooter, GameObject projectile, Vector3 startingPosition, Vector3 endingPosition)
        {
            yield return new WaitForSeconds((endingPosition - startingPosition).magnitude / projectileSpeed());
            DestroyProjectileNow(shooter, projectile);
        }

        void DestroyProjectileNow(Player shooter, GameObject projectile)
        {
            var script = projectile.GetComponent<Projectile>();
            shooter.LaunchedProjectiles.Remove(script);
            Object.Destroy(projectile);
        }
    }
}
