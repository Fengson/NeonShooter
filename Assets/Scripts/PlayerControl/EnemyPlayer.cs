using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.PlayerControl
{
    class EnemyPlayer : MonoBehaviour, IPlayer
    {
        public GameObject TEMP_nose;

        public NotifyingProperty<Vector3> Position { get; private set; }
        public NotifyingProperty<Vector2> Rotations { get; private set; }
        public NotifyingProperty<Vector3> Direction { get; private set; }

        public EnemyPlayer()
        {
            Position = new NotifyingProperty<Vector3>();
            Rotations = new NotifyingProperty<Vector2>();
            Direction = new NotifyingProperty<Vector3>();
        }

        void Start()
        {
            Position.Value = transform.position;

            var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            Rotations.Value = new Vector2(
                cameraObject.transform.eulerAngles.y,
                cameraObject.transform.eulerAngles.x);
            Direction.Value = Quaternion.Euler(Rotations.Value.x, Rotations.Value.y, 0) * Vector3.forward;

            Position.OnValueChanged += Position_OnValueChanged;
            Rotations.OnValueChanged += Rotations_OnValueChanged;
        }

        void Position_OnValueChanged(Vector3 oldValue, Vector3 newValue)
        {
            transform.localPosition = newValue;
        }

        void Rotations_OnValueChanged(Vector2 oldValue, Vector2 newValue)
        {
            Vector3 rot = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(rot.x, newValue.y, rot.z);

            rot = TEMP_nose.transform.localEulerAngles;
            TEMP_nose.transform.localEulerAngles = new Vector3(newValue.x, rot.y, rot.z);
        }
    }
}
