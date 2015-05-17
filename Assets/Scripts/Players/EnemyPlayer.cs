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
                var globalsObject = GameObject.FindGameObjectWithTag("Globals");
                if (globalsObject == null) return DefaultLerpFactor;
                var globals = globalsObject.GetComponent<Globals>();
                if (globals == null) return DefaultLerpFactor;
                return globals.enemyLerpFactor;
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

            Position.OnValueChanged += Position_OnValueChanged;
            Rotations.OnValueChanged += Rotations_OnValueChanged;
        }

        void Update()
        {
            float dProgress = Time.deltaTime * LerpFactor;
            Debug.Log(string.Format("{0}, {1}", LerpFactor, dProgress));
            positionLerp.UpdateForward(dProgress);
            rotationsLerp.UpdateForward(dProgress);
        }

        void Position_OnValueChanged(Vector3 oldValue, Vector3 newValue)
        {
            positionLerp.TargetValue = newValue;
        }

        void Rotations_OnValueChanged(Vector2 oldValue, Vector2 newValue)
        {
            //Vector3 rot = transform.localEulerAngles;
            //transform.localEulerAngles = new Vector3(rot.x, newValue.y, rot.z);

            //rot = TEMP_nose.transform.localEulerAngles;
            //TEMP_nose.transform.localEulerAngles = new Vector3(newValue.x, rot.y, rot.z);
            rotationsLerp.TargetValue = newValue;
        }

        public void GainLife(int amount)
        {
        }

        public void DealDamage(int amount, DamageEffect damageEffect)
        {
        }
    }
}
