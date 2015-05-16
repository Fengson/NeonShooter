using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players
{
    class EnemyPlayer : MonoBehaviour, IPlayer
    {
        public string NetworkName { get; set; }

        public NotifyingProperty<Vector3> Position { get; private set; }
        public NotifyingProperty<Vector2> Rotations { get; private set; }
        public NotifyingProperty<Vector3> Direction { get; private set; }

        public NotifyingProperty<Weapon> SelectedWeapon { get; private set; }

        public NotifyingList<Projectile> LaunchedProjectiles { get; private set; }

        public EnemyPlayer()
        {
            Position = NotifyingProperty<Vector3>.PublicBoth();
            Rotations = NotifyingProperty<Vector2>.PublicBoth();
            Direction = NotifyingProperty<Vector3>.PublicBoth();

            SelectedWeapon = NotifyingProperty<Weapon>.PublicBoth();

            LaunchedProjectiles = new NotifyingList<Projectile>();
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

        private Vector3 previousPosition;
        private float time = 0;
        void Update()
        {
            time += Time.deltaTime * 10;
            transform.position = Vector3.Lerp(previousPosition, Position.Value, time);
        }

        void Position_OnValueChanged(Vector3 oldValue, Vector3 newValue)
        {
            previousPosition = transform.position;
            time = 0;
        }

        void Rotations_OnValueChanged(Vector2 oldValue, Vector2 newValue)
        {
            Vector3 rot = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(rot.x, newValue.y, rot.z);

            //rot = TEMP_nose.transform.localEulerAngles;
            //TEMP_nose.transform.localEulerAngles = new Vector3(newValue.x, rot.y, rot.z);
        }

        public void GainLife(int amount)
        {
        }

        public void DealDamage(int amount, DamageEffect damageEffect)
        {
        }
    }
}
