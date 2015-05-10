using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;

public class RailGun : Weapon {

	public RailGun() : base(150, 100, 0, 35)
    	{
    	}

    	public override void shoot(Player shooter, int costPayed)
    	{
    		Vector3 startingPosition = shooter.Position[null];
    	 	Vector3 endingPosition =
               	Vector3.MoveTowards(startingPosition, startingPosition+this.Reach*shooter.Direction[null], (int)this.Reach);
            RaycastHit hitInfo;
		    if(shootLine(startingPosition, endingPosition, out hitInfo)) {
        	    shooter.enemyShot(this, hitInfo.collider, this.Damage, costPayed);
			}
    	}

		public override float missileFlightDuration() {
			return 0.01f;
		}

		public override int lifeRequiredToOwn() {
			return 100;
		}

		public override Weapon nextWeapon() {
			return new RocketLauncher();
		}

    	public override string getWeaponName() {
    	    return "Rail Gun";
    	}
}
