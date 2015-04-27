using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.PlayerControl
{
	public class Weapon
	{
		public int Damage { get; private set; }
		public double Reach { get; private set; }
		public int AmmoCost { get; private set; }

		public Weapon(int dmg, double reach, int ammo_cost)
		{
			this.Damage = dmg;
			this.Reach = reach;
			this.AmmoCost = ammo_cost;
		}

		public bool shoot()
		{
			return false;
		}
	}
}

