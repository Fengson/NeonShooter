using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;

public class RailGun : Weapon {

			public RailGun() : base(150, 100, 0, 35)
    		{
    		}

    		public override void shoot(Player shooter, IPlayer target)
    		{
    			Vector3 startingPosition = shooter.Position[null];
    		 	Vector3 endingPosition =
                	Vector3.MoveTowards(startingPosition, startingPosition+this.Reach*shooter.Direction[null], (int)this.Reach);

                RaycastHit hitInfo;
			    if(shootLine(startingPosition, endingPosition, out hitInfo)) {
                    shooter.enemyShot(this, hitInfo.collider, this.Damage);
				}
    		}

    		public override string getWeaponName() {
    		    return "Rail Gun";
    		}
}
