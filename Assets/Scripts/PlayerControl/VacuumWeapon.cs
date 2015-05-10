using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;

public class VacuumWeapon : Weapon {


	public VacuumWeapon() : base(50, 10, (float)(4.0/18.0*Mathf.PI), 0)
	{
	}

   	public override void shoot(Player shooter, IPlayer target, int costPayed)
   	{
    	RaycastHit hitInfo;
   		Vector3 heading = (target.Position[null] - shooter.Position[null]).normalized;
   		double angle_cos = Vector3.Dot(heading, shooter.Direction[null].normalized);
   		if(angle_cos > this.ConeAngleCos)
   		{
   			if(Physics.Raycast(shooter.Position[null], heading, out hitInfo))
   			{
   				if(hitInfo.distance <= this.Reach){
					shooter.enemyShot(this, hitInfo.collider, this.Damage, costPayed);
   				}
   			}
   		}
   	}

   	/**
   	method redundant for this specific weapon, suction speed would be more useful, as the distance will vary
   	*/
   	public override float missileFlightDuration() {
     	return 0;
     }

	public override string getWeaponName() {
		return "Vacuum";
	}
}
