using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.PlayerControl
{
    public class Projectile : MonoBehaviour
    {
        private readonly System.Object access;

        public Weapon ParentWeapon { get; set; }
        public int CubeValue { get; set; }

        public NotifyingProperty<Vector3> Position { get; private set; }
        public NotifyingProperty<Quaternion> Rotation { get; private set; }

        public Projectile()
        {
            access = new System.Object();
            Position = new NotifyingProperty<Vector3>(access, true, false);
            Rotation = new NotifyingProperty<Quaternion>(access, true, false);
        }

        void Update()
        {
            Position[access] = transform.position;
            Rotation[access] = transform.rotation;
        }
    }
}
