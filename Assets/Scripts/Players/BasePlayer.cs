using NeonShooter.Players.Cube;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeonShooter.Players
{
    public abstract class BasePlayer : MonoBehaviour, IPlayer
    {
        public int initialRadius;

        public bool cubeStructureVisible = false;

        public AudioSource[] sounds;
        public GameObject railGunShotPrefab;

        protected object Access { get; private set; }

        protected CubeStructure CubeStructure { get; private set; }
        public int Life { get { return CubeStructure.Count; } }
        public int InitialRadius { get { return Mathf.Max(1, initialRadius); } }

        public Weapon DefaultWeapon { get; private set; }
        public List<Weapon> Weapons { get; private set; }
        public Dictionary<int, Weapon> WeaponsById { get; private set; }

        public Dictionary<long, BaseProjectile> ProjectilesById { get; private set; }
        public Dictionary<long, BaseCubeling> CubelingsById { get; private set; }

        public Vector3 Direction { get; private set; }

        public INotifyingList<IVector3> CellsInStructure { get; protected set; }

        public NotifyingProperty<Vector3> Position { get; protected set; }
        public NotifyingProperty<Vector2> Rotations { get; protected set; }

        public NotifyingProperty<Weapon> SelectedWeapon { get; protected set; }
        public NotifyingProperty<bool> ContinousFire { get; protected set; }

        public INotifyingList<BaseProjectile> LaunchedProjectiles { get; protected set; }
        public INotifyingList<BaseCubeling> SpawnedCubelings { get; protected set; }

        public InvokableAction<Damage> DamageDealt { get; protected set; }
        public InvokableAction<PickUp> CubelingPickedUp { get; protected set; }
        public InvokableAction<PickUpAcknowledge> CubelingPickUpAcknowledged { get; protected set; }

		public GameObject firstPersonCharacter;

        protected BasePlayer()
        {
            Access = new object();

            DefaultWeapon = new VacuumWeapon();
            Weapons = new List<Weapon> { DefaultWeapon, new RailGun(), new RocketLauncher() };
            WeaponsById = new Dictionary<int, Weapon>();
            foreach (var w in Weapons)
                WeaponsById[w.Id] = w;
            ProjectilesById = new Dictionary<long, BaseProjectile>();
            CubelingsById = new Dictionary<long, BaseCubeling>();
        }

        void Awake()
        {
            gameObject.layer = Globals.PlayersLayer;

            CubeStructure = new CubeStructure(gameObject, InitialRadius);
            CubeStructure.CellAppender = new RandomOuterLayerCellAppender();
            CubeStructure.CellRetriever = new RandomOuterLayerCellRetriever();
            CubeStructure.Visible = cubeStructureVisible;
            ChangeSize(1, InitialRadius);
            CubeStructure.RadiusChanged += CubeStructure_RadiusChanged;
            CellsInStructure.AddMany(from cell in CubeStructure select cell.Position);

            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        void Start()
        {
            OnStart();
        }
        
        protected virtual void OnStart()
        {
        }

        void Update()
        {
            CubeStructure.Visible = cubeStructureVisible;

            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }

        void OnTriggerEnter(Collider other)
        {
            TriggerEnter(other);
        }

        protected virtual void TriggerEnter(Collider other)
        {
        }

        protected void ChangeSize(int currentRadius, int newRadius)
        {
            var currentR = CalculateColliderRadius(currentRadius);
            var newR = CalculateColliderRadius(newRadius);
            var dr = newR - currentR;

            transform.position += Vector3.up * dr;

            ChangeSizeDetails(currentR, newR);
        }

        protected virtual void ChangeSizeDetails(float oldRadius, float newRadius)
        {
        }

        protected float CalculateColliderRadius(int structureRadius)
        {
            return Mathf.Max(0, structureRadius - 0.5f);
        }

        protected void RecalculateDirection()
        {
            if (Rotations == null) return;
            Direction = Quaternion.Euler(Rotations.Value.x, Rotations.Value.y, 0) * Vector3.forward;
        }

        void CubeStructure_RadiusChanged(int oldValue, int newValue)
        {
            ChangeSize(oldValue, newValue);
        }
    }
}
