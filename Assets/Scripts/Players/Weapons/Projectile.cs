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

        protected override NotifyingProperty<Vector3> CreatePositionProperty()
        {
            return NotifyingProperty<Vector3>.PublicGetPrivateSet(Access);
        }

        protected override NotifyingProperty<Quaternion> CreateRotationProperty()
        {
            return NotifyingProperty<Quaternion>.PublicGetPrivateSet(Access);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            Position[Access] = transform.position;
            Rotation[Access] = transform.rotation;
        }
    }
}
