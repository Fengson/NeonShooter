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
        public NotifyingProperty<Vector3> Velocity { get; private set; }

        protected Atom(long? id = null)
        {
            Access = new object();
            this.id = id;

            Position = new NotifyingProperty<Vector3>();
            Rotation = new NotifyingProperty<Quaternion>();
            Velocity = new NotifyingProperty<Vector3>();
        }

        void Awake()
        {
 	       OnAwake();
        }

        protected virtual void OnAwake()
        {
			gameObject.layer = Globals.AtomsLayer;
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
