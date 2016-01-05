using NeonShooter.AppWarp;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public abstract class Projectile : BaseProjectile
    {
        public int CubeValue { get; set; }

        protected Projectile()
        {
            Id = System.DateTime.UtcNow.Ticks;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
			Destroy(this.gameObject);
        }

        protected override NotifyingProperty<Vector3> CreatePositionProperty()
        {
            return NotifyingProperty<Vector3>.PublicGetPrivateSet(Access);
        }

        protected override NotifyingProperty<Quaternion> CreateRotationProperty()
        {
            return NotifyingProperty<Quaternion>.PublicGetPrivateSet(Access);
        }

        protected override NotifyingProperty<Vector3> CreateVelocityProperty()
        {
            return NotifyingProperty<Vector3>.PublicGetPrivateSet(Access);
        }

		protected override InvokableAction<ProjectileHit> CreateProjectileHitAction()
		{
			return InvokableAction<ProjectileHit>.Private(Access);
		}

        protected override void OnUpdate()
        {
            base.OnUpdate();

            Position[Access] = transform.position;
            Rotation[Access] = transform.rotation;
        }
    }
}
