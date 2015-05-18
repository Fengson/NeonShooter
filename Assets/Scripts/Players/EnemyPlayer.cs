﻿using NeonShooter.Cube;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace NeonShooter.Players
{
    public class EnemyPlayer : BasePlayer
    {
        PropertyInterpolator<Vector3> positionLerp;
        PropertyInterpolator<Vector2> rotationsLerp;

        float vacuumConeXRotation;

        public string NetworkName { get; set; }

        public bool DontLerp { get; set; }

        public EnemyPlayer()
        {
            ((VacuumWeapon)DefaultWeapon).ConeXRotation = () => vacuumConeXRotation;

            DontLerp = true;

            CellsInStructure = new NotifyingList<IVector3>();

            Position = NotifyingProperty<Vector3>.PublicBoth();
            Rotations = NotifyingProperty<Vector2>.PublicBoth();
            Rotations.ValueChanged += (oldVal, newVal) => RecalculateDirection();

            ContinousFire = NotifyingProperty<bool>.PublicBoth();
            SelectedWeapon = NotifyingProperty<Weapon>.PublicBoth();

            LaunchedProjectiles = new NotifyingList<BaseProjectile>();

            DamageDealt = InvokableAction<Damage>.Private(Access);
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            positionLerp = new PropertyInterpolator<Vector3>(
                () => transform.position,
                v => transform.position = v,
                PropertyInterpolator.Vector3Lerp);
            rotationsLerp = new PropertyInterpolator<Vector2>(
                () => new Vector2(vacuumConeXRotation, transform.localEulerAngles.y),
                v =>
                {
                    var rot = transform.localEulerAngles;
                    transform.localEulerAngles = new Vector3(rot.x, v.y, rot.z);
                    vacuumConeXRotation = v.x;
                },
                PropertyInterpolator.Vector2LerpAngle);
        }

        protected override void OnStart()
        {
            base.OnStart();

            Position.Value = transform.position;
            Rotations.Value = new Vector2(transform.eulerAngles.x, transform.eulerAngles.y);

            CellsInStructure.ListChanged += CellsInStructure_ListChanged;

            Position.ValueChanged += Position_ValueChanged;
            Rotations.ValueChanged += Rotations_ValueChanged;

            ContinousFire.ValueChanged += ContinousFire_ValueChanged;

        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            float dProgress = Time.deltaTime * Globals.LerpFactor;
            positionLerp.Update(dProgress);
            rotationsLerp.Update(dProgress);

            SelectedWeapon.Value.Update();
        }

        public void DealDamage(Damage damage)
        {
            DamageDealt.Invoke(damage, Access);
        }

        protected override void ChangeSizeDetails(float oldRadius, float newRadius)
        {
            var collider = GetComponent<CapsuleCollider>();
            collider.radius = newRadius;
            collider.height = 2 * newRadius;
        }

        void CellsInStructure_ListChanged(NotifyingListEventArgs<IVector3> e)
        {
            switch (e.Change)
            {
                case NotifyingListEventArgs.ListChange.Add:
                    CubeStructure.AddCellAt(e.Item);
                    break;
                case NotifyingListEventArgs.ListChange.AddMany:
                    CubeStructure.AddCellsAt(e.Items);
                    break;
                case NotifyingListEventArgs.ListChange.Remove:
                    CubeStructure.RemoveCellAt(e.Item);
                    break;
                case NotifyingListEventArgs.ListChange.RemoveMany:
                    CubeStructure.RemoveCellsAt(e.Items);
                    break;
                case NotifyingListEventArgs.ListChange.Set:
                    CubeStructure.RemoveCellAt(e.OldItem);
                    CubeStructure.AddCellAt(e.NewItem);
                    break;
                case NotifyingListEventArgs.ListChange.Clear:
                    CubeStructure.RetrieveCells(CubeStructure.Count);
                    break;
            }
        }

        void Position_ValueChanged(Vector3 oldValue, Vector3 newValue)
        {
            positionLerp.TargetValue = newValue;
            if (DontLerp) positionLerp.Progress = 1;
        }

        void Rotations_ValueChanged(Vector2 oldValue, Vector2 newValue)
        {
            rotationsLerp.TargetValue = newValue;
            if (DontLerp) positionLerp.Progress = 1;
        }

        void ContinousFire_ValueChanged(bool oldValue, bool newValue)
        {
            if (newValue) SelectedWeapon.Value.OnShootStart(this);
            else SelectedWeapon.Value.OnShootEnd();
        }
    }
}
