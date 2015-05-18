using NeonShooter.AppWarp.States;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public class Projectile : BaseProjectile
    {
        public int CubeValue { get; set; }

        public Projectile()
            : base(System.DateTime.UtcNow.Ticks)
        {
            Position = NotifyingProperty<Vector3>.PublicGetPrivateSet(Access);
            Rotation = NotifyingProperty<Quaternion>.PublicGetPrivateSet(Access);
        }

        void Update()
        {
            Position[Access] = transform.position;
            Rotation[Access] = transform.rotation;
        }
    }
}
