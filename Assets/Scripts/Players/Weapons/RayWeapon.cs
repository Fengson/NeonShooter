using System.Collections;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public abstract class RayWeapon : Weapon
    {
        public RayWeapon(int damage, float reach, int ammoCost)
            : base (damage, reach, ammoCost)
        {
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
            yield return new WaitForSeconds(0.05f);
            Object.Destroy(projectile);
        }
    }
}
