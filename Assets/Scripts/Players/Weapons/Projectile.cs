﻿using NeonShooter.AppWarp;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public abstract class Projectile : BaseProjectile
    {
        public int CubeValue { get; set; }

        protected Projectile()
        {
            Id = System.DateTime.UtcNow.Ticks;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            Position.Value = transform.position;
            Rotation.Value = transform.rotation;
        }
    }
}
