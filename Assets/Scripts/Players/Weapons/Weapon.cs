using System.Collections;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public abstract class Weapon
    {
        float currentCoolDownTime;

        public abstract int Id { get; }
        public abstract DamageEffect DamageEffect { get; }
        public abstract FireType FireType { get; }
        public abstract float CoolDownTime { get; }
        public virtual Color ProjectileColor { get { return Color.white; } }

        public int Damage { get; private set; }
        public float Reach { get; private set; }
        public int AmmoCost { get; private set; }
        protected float ConeAngleRadians { get; private set; }
        public double ConeAngleCos { get; private set; }

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

        public virtual void OnShootStart(BasePlayer shooter)
        {
        }

        public abstract void shoot(Player shooter, int costPayed);

        public virtual void OnShootEnd()
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

        public GameObject CreateProjectile<T>(BasePlayer shooter, Vector3 startingPosition, Color? color = null)
            where T : BaseProjectile
        {
            if (!color.HasValue) color = ProjectileColor;

            var projectileObject = Object.Instantiate(Globals.Instance.projectilePrefab);
            projectileObject.transform.position = startingPosition;
            projectileObject.GetComponent<Renderer>().material.color = color.Value;
            var projectile = projectileObject.AddComponent<T>();
            projectile.ParentWeapon = this;
            shooter.LaunchedProjectiles.Add(projectile);
            if (projectile.Id != BaseProjectile.NullId)
                shooter.ProjectilesById[projectile.Id] = projectile;
            return projectileObject;
        }

        public GameObject CreateProjectileAndApplyForce(BasePlayer shooter, Vector3 startingPosition, Color color, int costPaid)
        {
            var projectile = CreateProjectile<Projectile>(shooter, startingPosition, color);
            var script = projectile.GetComponent<Projectile>();
            script.CubeValue = costPaid;
            projectile.GetComponent<ConstantForce>().force = shooter.Direction.normalized * (projectileSpeed() * projectileForceModifier());
            projectile.GetComponent<ConstantForce>().torque = shooter.Direction * 10;
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
