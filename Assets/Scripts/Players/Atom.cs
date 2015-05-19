using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players
{
    public abstract class Atom : MonoBehaviour
    {
        public const long NullId = -1;

        protected object Access { get; private set; }

        long? id;
        public long Id
        {
            get { return id.HasValue ? id.Value : NullId; }
            set { if (!id.HasValue) id = value; }
        }

        public NotifyingProperty<Vector3> Position { get; private set; }
        public NotifyingProperty<Quaternion> Rotation { get; private set; }

        protected Atom(long? id = null)
        {
            Access = new object();
            this.id = id;

            Position = CreatePositionProperty();
            Rotation = CreateRotationProperty();
        }

        protected abstract NotifyingProperty<Vector3> CreatePositionProperty();
        protected abstract NotifyingProperty<Quaternion> CreateRotationProperty();

        void Awake()
        {
            gameObject.layer = Globals.AtomsLayer;

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
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }
    }
}
