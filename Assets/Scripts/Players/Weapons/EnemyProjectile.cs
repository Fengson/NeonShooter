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
        {
            DontLerp = true;
        }

        protected override NotifyingProperty<Vector3> CreatePositionProperty()
        {
            return NotifyingProperty<Vector3>.PublicBoth();
        }

        protected override NotifyingProperty<Quaternion> CreateRotationProperty()
        {
            return NotifyingProperty<Quaternion>.PublicBoth();
        }

        protected override NotifyingProperty<Vector3> CreateVelocityProperty()
        {
            return NotifyingProperty<Vector3>.PublicBoth();
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            
            positionLerp = new PropertyInterpolator<Vector3>(
                () => transform.position,
                v => transform.position = v,
                PropertyInterpolator.Vector3Lerp);
            rotationLerp = new PropertyInterpolator<Quaternion>(
                () => transform.rotation,
                q => transform.rotation = q,
                PropertyInterpolator.QuaternionLerp);
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            Position.ValueChanged += Position_ValueChanged;
            Rotation.ValueChanged += Rotation_ValueChanged;
        }
        
        protected override void OnUpdate()
        {
            base.OnUpdate();
            
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
