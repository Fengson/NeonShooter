using NeonShooter;
using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;
using System.Collections;

namespace NeonShooter.PlayerControl
{
	abstract public class Weapon
	{
		public int Damage { get; private set; }
		public float Reach { get; private set; }
		public int AmmoCost { get; private set; }
		public double ConeAngleCos { get; private set; }
        public GameObject projectilePrefab;

		public Weapon(int dmg, float reach, float cone_angle_radians, int ammo_cost)
		{
			this.Damage = dmg;
			this.Reach = reach;
			this.AmmoCost = ammo_cost;
			this.ConeAngleCos = Mathf.Cos(cone_angle_radians);
		}

		abstract public void shoot(Player shooter, int costPayed);

		abstract public string getWeaponName();

		protected bool shootLine(Vector3 rayStart, Vector3 rayEnd, out RaycastHit hitInfo)
		{
			return Physics.Linecast(rayStart, rayEnd, out hitInfo);
		}

		abstract public float projectileSpeed();

		abstract public int lifeRequiredToOwn();

		abstract public Weapon nextWeapon();

		abstract public void shootSound(Player player);

    	public GameObject createProjectile(Player shooter, Vector3 startingPosition, Vector3 endingPosition, Color color, bool laser) {
    		GameObject projectile;
    		if(laser) {
				//LASER
				projectile = shooter.instantiateProjectile(shooter.railGunShotPrefab);
				LineRenderer lineRenderer = projectile.GetComponent<LineRenderer>();
				lineRenderer.SetPosition(0, startingPosition);
				lineRenderer.SetPosition(1, endingPosition);
				lineRenderer.SetColors(Color.red, Color.yellow);
				lineRenderer.SetWidth(0.2F, 0.2F);
			} else {
				projectile = shooter.instantiateProjectile(shooter.projectilePrefab);
                projectile.transform.position=startingPosition+2*shooter.Direction[null];
                projectile.GetComponent<ConstantForce>().force=shooter.Direction[null].normalized*projectileSpeed()*10;
                projectile.GetComponent<ConstantForce>().torque=shooter.Direction[null]*10;
                projectile.GetComponent<Renderer>().material.color = color;
			}

        	return projectile;
    	}

    	public IEnumerator destroyLaserProjectile(Player shooter, GameObject projectile) {
        	yield return new WaitForSeconds(0.1f);

        	shooter.destroyProjectile(projectile);
    	}

    	public IEnumerator destroyProjectile(Player shooter, GameObject projectile) {
        	yield return new WaitForSeconds(Reach/projectileSpeed());

        	shooter.destroyProjectile(projectile);
    	}

    	public IEnumerator hitAndDestroyProjectile(Player shooter, GameObject projectile, Vector3 startingPosition, Vector3 endingPosition) {
        	yield return new WaitForSeconds((endingPosition-startingPosition).magnitude/projectileSpeed());
        	shooter.destroyProjectile(projectile);
    	}
	}
}
