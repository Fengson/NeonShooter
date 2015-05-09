using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;

public class VacuumWeapon : Weapon {


	public VacuumWeapon() : base(50, 0, (float)(4.0/18.0*Mathf.PI), 10)
	{
	}

	public override void shoot(Player shooter, IPlayer target)
	{
    	RaycastHit hitInfo;
		Vector3 heading = (target.Position[null] - shooter.Position[null]).normalized;
		double angle_cos = Vector3.Dot(heading, shooter.Direction[null].normalized);
		if(angle_cos > this.ConeAngleCos)
		{
			if(Physics.Raycast(shooter.Position[null], heading, out hitInfo, this.Reach))
			{
				shooter.enemyShot(this, hitInfo.collider, this.Damage);
			}
		}
	}

	public override string getWeaponName() {
		return "Vacuum";
	}
}
