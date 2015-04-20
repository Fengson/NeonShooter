using UnityEngine;
using System.Collections;
using NeonShooter.Utils;

namespace NeonShooter.PlayerControl
{
    public class Player : MonoBehaviour
    {
        private readonly System.Object access;

        public NotifyingProperty<Vector3> Position { get; private set; }
        public NotifyingProperty<Vector2> Rotations { get; private set; }
        public NotifyingProperty<Vector3> Direction { get; private set; }

        public GameObject TEMP_enemy;

        public Player()
        {
            access = new System.Object();

            Position = new NotifyingProperty<Vector3>(access, true, false);
            Rotations = new NotifyingProperty<Vector2>(access, true, false);
            Direction = new NotifyingProperty<Vector3>(access, true, false);
        }

        void Update()
        {
            Position[access] = transform.position;

            var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            Rotations[access] = new Vector2(
                cameraObject.transform.eulerAngles.x,
                cameraObject.transform.eulerAngles.y);
            Direction[access] = Quaternion.Euler(Rotations.Value.x, Rotations.Value.y, 0) * Vector3.forward;

            var script = TEMP_enemy.GetComponent<EnemyPlayer>();
            script.Position.Value = Position.Value + new Vector3(5, 0, 0);
            script.Rotations.Value = new Vector2(Rotations.Value.x, -Rotations.Value.y);
        }
    }
}