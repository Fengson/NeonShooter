using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
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
            Position = NotifyingProperty<Vector3>.PublicGetPrivateSet(access);
            Rotation = NotifyingProperty<Quaternion>.PublicGetPrivateSet(access);
        }

        void Update()
        {
            Position[access] = transform.position;
            Rotation[access] = transform.rotation;
        }
    }
}
