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
    		Vector3 startingPosition = shooter.Position[null]+new Vector3(0,0.8f,0);
    	 	Vector3 endingPosition =
               	Vector3.MoveTowards(startingPosition, startingPosition+this.Reach*shooter.Direction[null], (int)Reach);
            RaycastHit hitInfo;
		    if(shootLine(startingPosition, endingPosition, out hitInfo)) {
        	    endingPosition = hitInfo.point;
        		shooter.enemyShot(this, hitInfo.collider, Damage, costPayed);
            	GameObject projectile = createProjectile(shooter, startingPosition, endingPosition,
				(hitInfo.collider.name=="Plane" || hitInfo.collider.name=="Projectile(Clone)")?Color.red:
				Color.green, false);
				shooter.StartCoroutine(hitAndDestroyProjectile(shooter, projectile, startingPosition, endingPosition));
			} else {
            	GameObject projectile = createProjectile(shooter, startingPosition, endingPosition, Color.red, false);
				shooter.StartCoroutine(destroyProjectile(shooter, projectile));
			}
    	}

		public override float projectileSpeed() {
			return 100.0f;
		}

		public override int lifeRequiredToOwn() {
			return -100;
		}

		public override Weapon nextWeapon() {
			return new RocketLauncher();
		}

    	public override string getWeaponName() {
    	    return "Rail Gun";
    	}
}
