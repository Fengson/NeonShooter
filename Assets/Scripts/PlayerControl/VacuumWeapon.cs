using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;

public class VacuumWeapon : Weapon
{
	public VacuumWeapon() : base(50, 10, (float)(4.0/18.0*Mathf.PI), 0)	{}

   	public override void shoot(Player shooter, int costPayed)
   	{
		foreach(GameObject target in appwarp.enemies)
		{
			Vector3 heading = (target.transform.position - shooter.Position[null]).normalized;
        	double angle_cos = Vector3.Dot(heading, shooter.Direction[null].normalized);
        	if(angle_cos > this.ConeAngleCos)
           	{
				RaycastHit hit;
				if(Physics.Raycast(shooter.Position[null], heading, out hit, this.Reach))
				{
					shooter.enemyShot(this, target.GetComponent<Collider>(), this.Damage, costPayed);
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
