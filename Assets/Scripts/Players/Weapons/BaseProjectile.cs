using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public abstract class BaseProjectile : Atom
    {
        public Weapon ParentWeapon { get; set; }
		public InvokableAction<ProjectileHit> ProjectileHit { get; protected set; }

		public BaseProjectile()
		{
			ProjectileHit = new InvokableAction<ProjectileHit>();
		}
    }
}
