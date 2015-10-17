﻿using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Cube
{
    public class Cubeling : BaseCubeling
    {
        public float SpawnerPickDelay { get; set; }
        public EnemyPlayer TargetPlayer { get; set; }

        public override bool Pickable { get { return SpawnerPickDelay == 0; } }

        private Cubeling()
        {
            // TODO: need better id-generation system?
            Id = System.DateTime.UtcNow.Ticks;
        }

        protected override NotifyingProperty<Vector3> CreatePositionProperty()
        {
            return NotifyingProperty<Vector3>.PublicGetPrivateSet(Access);
        }

        protected override NotifyingProperty<Quaternion> CreateRotationProperty()
        {
            return NotifyingProperty<Quaternion>.PublicGetPrivateSet(Access);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            SpawnerPickDelay = Mathf.Max(0, SpawnerPickDelay - Time.deltaTime);

            if (TargetPlayer != null)
            {
                float step = Globals.CubelingSuckSpeed * Time.deltaTime;

                transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.transform.position, step);
                transform.eulerAngles = new Vector3(1, 1, 1); // TODO: what is this - fixed orientation 1 deg around every axis? what's the purpose?
            }

            Position[Access] = transform.position;
            Rotation[Access] = transform.rotation;
        }

    }
}
