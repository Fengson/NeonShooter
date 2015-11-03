using System.Collections;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public abstract class ProjectileWeapon : Weapon
    {
        public abstract float ProjectileSpeed { get; }
        public abstract float ProjectileForceModifier { get; }

        public ProjectileWeapon(int damage, float reach, int ammoCost)
            : base (damage, reach, ammoCost)
        {
        }

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
            projectile.GetComponent<ConstantForce>().force = shooter.Direction.normalized * (ProjectileSpeed * ProjectileForceModifier);
            projectile.GetComponent<ConstantForce>().torque = shooter.Direction * 10;
            return projectile;
        }

        public IEnumerator destroyProjectile(Player shooter, GameObject projectile)
        {
            yield return new WaitForSeconds(Reach / ProjectileSpeed);
            DestroyProjectileNow(shooter, projectile);
        }

        public IEnumerator hitAndDestroyProjectile(Player shooter, GameObject projectile, Vector3 startingPosition, Vector3 endingPosition)
        {
            yield return new WaitForSeconds((endingPosition - startingPosition).magnitude / ProjectileSpeed);
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
