using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players
{
    public class EnemyPlayer : MonoBehaviour, IPlayer
    {
        public const float DefaultLerpFactor = 10;

        public string NetworkName { get; set; }

        public float LerpFactor
        {
            get
            {
                if (Globals.Instance == null) return DefaultLerpFactor;
                return Globals.Instance.enemyLerpFactor;
            }
        }

        public NotifyingProperty<Vector3> Position { get; private set; }
        public NotifyingProperty<Vector2> Rotations { get; private set; }
        public NotifyingProperty<Vector3> Direction { get; private set; }

        public NotifyingProperty<Weapon> SelectedWeapon { get; private set; }

        public NotifyingList<Projectile> LaunchedProjectiles { get; private set; }

        PropertyInterpolator<Vector3> positionLerp;
        PropertyInterpolator<Vector2> rotationsLerp;

        public EnemyPlayer()
        {
            Position = NotifyingProperty<Vector3>.PublicBoth();
            Rotations = NotifyingProperty<Vector2>.PublicBoth();
            Direction = NotifyingProperty<Vector3>.PublicBoth();

            SelectedWeapon = NotifyingProperty<Weapon>.PublicBoth();

            LaunchedProjectiles = new NotifyingList<Projectile>();
        }

        void Awake()
        {
            positionLerp = new PropertyInterpolator<Vector3>(
                () => transform.position,
                v => transform.position = v,
                (v1, v2, p) => Vector3.Lerp(v1, v2, p));
            rotationsLerp = new PropertyInterpolator<Vector2>(
                () => transform.localEulerAngles,
                v =>
                {
                    var rot = transform.localEulerAngles;
                    transform.localEulerAngles = new Vector3(rot.x, v.y, rot.z);
                },
                (v1, v2, p) => new Vector2(
                    Mathf.LerpAngle(v1.x, v2.x, p),
                    Mathf.LerpAngle(v1.y, v2.y, p)));
        }

        void Start()
        {
            Position.Value = transform.position;

            var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            Rotations.Value = new Vector2(
                cameraObject.transform.eulerAngles.y,
                cameraObject.transform.eulerAngles.x);
            Direction.Value = Quaternion.Euler(Rotations.Value.x, Rotations.Value.y, 0) * Vector3.forward;

            Position.ValueChanged += (oldVal, newVal) => positionLerp.TargetValue = newVal;
            Rotations.ValueChanged += (oldVal, newVal) => rotationsLerp.TargetValue = newVal;
        }

        void Update()
        {
            float dProgress = Time.deltaTime * LerpFactor;
            positionLerp.Update(dProgress);
            rotationsLerp.Update(dProgress);
        }

        public void GainLife(int amount)
        {
        }

        public void DealDamage(int amount, DamageEffect damageEffect)
        {
        }
    }
}
