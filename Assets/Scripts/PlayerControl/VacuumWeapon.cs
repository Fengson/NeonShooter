using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;

public class VacuumWeapon : Weapon {


	public VacuumWeapon() : base(50, 10, (float)(4.0/18.0*Mathf.PI), 0)
	{
	}

   	public override void shoot(Player shooter, int costPayed)
   	{
   	   //TODO change this Arek when we get to know how to get IPlayer array
    	Collider[] hitColliders = Physics.OverlapSphere(shooter.Position[null], Reach);
		int k = 0;
        while (k < hitColliders.Length) {
            if(hitColliders[k].name=="Plane" || hitColliders[k].name=="FPSController") {
            	k++;
            	continue;
            }
        	Vector3 heading = (hitColliders[k].bounds.center - shooter.Position[null]).normalized;
        	double angle_cos = Vector3.Dot(heading, shooter.Direction[null].normalized);
        	if(angle_cos > this.ConeAngleCos)
           		{
				shooter.enemyShot(this, hitColliders[k], this.Damage, costPayed);
            	}
            k++;
   		}
   	}

/*
OLD METHOD
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
 */
   	/**
   	method redundant for this specific weapon, suction speed would be more useful, as the distance will vary
   	*/
   	public override float missileFlightDuration() {
     	return 0;
     }

   	public override int lifeRequiredToOwn() {
     	return int.MinValue;
    }

   	public override Weapon nextWeapon() {
     	return new RailGun();
    }

	public override string getWeaponName() {
		return "Vacuum";
	}
}
