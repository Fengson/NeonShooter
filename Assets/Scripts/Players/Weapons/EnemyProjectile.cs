using NeonShooter.AppWarp.States;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public class EnemyProjectile : BaseProjectile
    {
        PropertyInterpolator<Vector3> positionLerp;
        PropertyInterpolator<Quaternion> rotationLerp;

        public bool DontLerp { get; set; }

        private EnemyProjectile()
            : base()
        {
            DontLerp = true;
            Position = NotifyingProperty<Vector3>.PublicBoth();
            Rotation = NotifyingProperty<Quaternion>.PublicBoth();
        }

        void Awake()
        {
            positionLerp = new PropertyInterpolator<Vector3>(
                () => transform.position,
                v => transform.position = v,
                PropertyInterpolator.Vector3Lerp);
            rotationLerp = new PropertyInterpolator<Quaternion>(
                () => transform.rotation,
                q => transform.rotation = q,
                PropertyInterpolator.QuaternionLerp);
        }

        void Start()
        {
            Position.ValueChanged += Position_ValueChanged;
            Rotation.ValueChanged += Rotation_ValueChanged;
        }

        void Update()
        {
            float dProgress = Time.deltaTime * Globals.LerpFactor;
            positionLerp.Update(dProgress);
            rotationLerp.Update(dProgress);
        }

        void Position_ValueChanged(Vector3 oldValue, Vector3 newValue)
        {
            positionLerp.TargetValue = newValue;
            if (DontLerp) positionLerp.Progress = 1;
        }

        void Rotation_ValueChanged(Quaternion oldValue, Quaternion newValue)
        {
            rotationLerp.TargetValue = newValue;
            if (DontLerp) rotationLerp.Progress = 1;
        }
    }
}
