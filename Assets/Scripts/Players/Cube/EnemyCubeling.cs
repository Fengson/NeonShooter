using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Cube
{
    public class EnemyCubeling : BaseCubeling
    {
        PropertyInterpolator<Vector3> positionLerp;
        PropertyInterpolator<Quaternion> rotationLerp;
        PropertyInterpolator<Vector3> velocityLerp;

        public override bool Pickable { get { return true; } }

        public bool DontLerp { get; set; }

        private EnemyCubeling()
        {
            DontLerp = true;
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
            velocityLerp = new PropertyInterpolator<Vector3>(
                () => GetComponent<Rigidbody>().velocity,
                v => GetComponent<Rigidbody>().velocity = v,
                PropertyInterpolator.Vector3Lerp);
        }

        protected override void OnStart()
        {
            base.OnStart();

            Position.ValueChanged += Position_ValueChanged;
            Rotation.ValueChanged += Rotation_ValueChanged;
            Velocity.ValueChanged += Velocity_ValueChanged;
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
        
        void Velocity_ValueChanged(Vector3 oldValue, Vector3 newValue)
        {
            velocityLerp.TargetValue = newValue;
            if (DontLerp) velocityLerp.Progress = 1;
        }
    }
}
