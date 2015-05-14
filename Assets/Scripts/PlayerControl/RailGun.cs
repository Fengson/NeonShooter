using NeonShooter;
using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;
using System.Collections;

public class RailGun : Weapon {

	public RailGun() : base(150, 900, 0, 35)
    	{
    	}

    	public override void shoot(Player shooter, int costPayed)
    	{
    		shootSound(shooter);
    		Vector3 startingPosition = shooter.Position[null]+new Vector3(0,0.8f,0);
    	 	Vector3 endingPosition =
               	Vector3.MoveTowards(startingPosition, startingPosition+this.Reach*shooter.Direction[null], (int)Reach);
            RaycastHit hitInfo;
            bool enemyShot = false;
		    if(shootLine(startingPosition, endingPosition, out hitInfo) && !(hitInfo.collider.name=="Plane" || hitInfo.collider.name=="Projectile(Clone)")) {
				foreach (GameObject target in appwarp.enemies)
				{
					if(target.GetComponent<Collider> ()==hitInfo.collider) {
						enemyShot = true;
						endingPosition = hitInfo.point;
						shooter.enemyShot(this, target, Damage, costPayed);
						GameObject projectile = createProjectile(shooter, startingPosition, Color.green);
						shooter.StartCoroutine(hitAndDestroyProjectile(shooter, projectile, startingPosition, endingPosition));
						break;
					}
				}
			}
			if(!enemyShot) {
            	GameObject projectile = createProjectile(shooter, startingPosition, Color.red);
				shooter.StartCoroutine(destroyProjectile(shooter, projectile));
			}
    	}

		public override float projectileSpeed() {
			return 100.0f;
		}

		public override float projectileForceModifier() {
			return 100.0f;
		}

		public override int lifeRequiredToOwn() {
			return -100;
		}

		public override Weapon nextWeapon() {
			return new RocketLauncher();
		}

		public override void shootSound(Player player) {
			player.sounds[1].Play();
		}

    	public override string getWeaponName() {
    	    return "Rail Gun";
    	}
}
