using NeonShooter.AppWarp;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public class Projectile : BaseProjectile
    {
        public int CubeValue { get; set; }

        protected Projectile()
        {
            Id = System.DateTime.UtcNow.Ticks;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
			this.ProjectileHit.Invoke(new ProjectileHit(this.ParentWeapon.Player, this.Id));
			Destroy(this.gameObject);
        }
		
        protected override void OnUpdate()
        {
            base.OnUpdate();

            Position.Value = transform.position;
            Rotation.Value = transform.rotation;
        }

    }
}
