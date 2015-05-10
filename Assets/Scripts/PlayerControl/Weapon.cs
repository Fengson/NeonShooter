using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;

namespace NeonShooter.PlayerControl
{
	abstract public class Weapon
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

		abstract public void shoot(Player shooter, int costPayed);

		abstract public string getWeaponName();

		protected bool shootLine(Vector3 rayStart, Vector3 rayEnd, out RaycastHit hitInfo)
		{
			return Physics.Linecast(rayStart, rayEnd, out hitInfo);
		}

		abstract public float missileFlightDuration();

		abstract public int lifeRequiredToOwn();

		abstract public Weapon nextWeapon();
	}
}
