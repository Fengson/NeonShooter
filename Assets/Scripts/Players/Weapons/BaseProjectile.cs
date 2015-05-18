using NeonShooter.AppWarp.States;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public abstract class BaseProjectile : MonoBehaviour
    {
        public const long NullId = -1;

        protected object Access { get; private set; }

        long? id;
        public long Id
        {
            get { return id.HasValue ? id.Value : NullId; }
            set { if (!id.HasValue) id = value; }
        }
        public Weapon ParentWeapon { get; set; }

        public NotifyingProperty<Vector3> Position { get; protected set; }
        public NotifyingProperty<Quaternion> Rotation { get; protected set; }

        protected BaseProjectile(long? id = null)
        {
            Access = new object();
            this.id = id;
        }
    }
}
