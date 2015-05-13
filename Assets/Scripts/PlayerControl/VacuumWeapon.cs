using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;

public class VacuumWeapon : Weapon
{
	public VacuumWeapon() : base(50, 10, (float)(4.0/18.0*Mathf.PI), 0)	{}

   	public override void shoot(Player shooter, int costPayed)
   	{
		foreach (GameObject target in appwarp.enemies)
		{
			Vector3 heading = (target.transform.position - shooter.Position [null]).normalized;
			double angle_cos = Vector3.Dot (heading, shooter.Direction [null].normalized);
			if (angle_cos > this.ConeAngleCos)
			{
				RaycastHit hit;
				if (Physics.Raycast (shooter.Position [null], heading, out hit, this.Reach))
				{
					shooter.enemyShot (this, target.GetComponent<Collider> (), this.Damage, costPayed);
				}
			}
		}
   	}

   	/**
   	suction speed in this case
   	*/
	public override float projectileSpeed() {
		return 1.0f;
	}

	public override float projectileForceModifier() {
		return 100.0f;
	}

   	public override int lifeRequiredToOwn() {
     	return int.MinValue;
    }

   	public override Weapon nextWeapon() {
     	return new RailGun();
    }

	public override void shootSound(Player player) {
		player.sounds[0].Play();
	}

	public override string getWeaponName() {
		return "Vacuum";
	}
}
