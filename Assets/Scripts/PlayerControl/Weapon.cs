using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;

namespace NeonShooter.PlayerControl
{
	public class Weapon
	{
		public int Damage { get; private set; }
		public float Reach { get; private set; }
		public int AmmoCost { get; private set; }
		public double ConeAngleCos { get; private set; }

		public Weapon(int dmg, float reach, float cone_angle_radians, int ammo_cost)
		{
			this.Damage = dmg;
			this.Reach = reach;
			this.AmmoCost = ammo_cost;
			this.ConeAngleCos = Mathf.Cos(cone_angle_radians);
		}

		public bool shoot(Player shooter, IPlayer target)
		{
			Vector3 heading = (target.Position[null] - shooter.Position[null]).normalized;
			double angle_cos = Vector3.Dot(heading, shooter.Direction[null].normalized);
			if(angle_cos > this.ConeAngleCos)
			{
				RaycastHit hit = new RaycastHit();
				if(Physics.Raycast(shooter.Position[null], heading, out hit))
				{
					if(hit.distance <= this.Reach){ return true; }
				}
			}
			return false;
		}
	}
}
