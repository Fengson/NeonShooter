﻿using NeonShooter.Players.Cube;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace NeonShooter.Players
{
    public class EnemyPlayer : BasePlayer
    {
        private bool leftGame;

        PropertyInterpolator<Vector3> positionLerp;
        PropertyInterpolator<Vector2> rotationsLerp;

        public string NetworkName { get; set; }

        public Player Player { get; set; }

        public bool DontLerp { get; set; }

        private EnemyPlayer()
        {
            DontLerp = true;

            CellsInStructure = new NotifyingList<IVector3>();

            Position = new NotifyingProperty<Vector3>();
            Rotations = new NotifyingProperty<Vector2>();
            Rotations.ValueChanged += (oldVal, newVal) => RecalculateDirection();

            ContinousFire = new NotifyingProperty<bool>();
            SelectedWeapon = new NotifyingProperty<Weapon>(DefaultWeapon);

            LaunchedProjectiles = new NotifyingList<BaseProjectile>();
            SpawnedCubelings = new NotifyingList<BaseCubeling>();

            DamageDealt = new InvokableAction<Damage>();
            CubelingPickedUp = new InvokableAction<PickUp>();
            CubelingPickUpAcknowledged = new InvokableAction<PickUpAcknowledge>();
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            positionLerp = new PropertyInterpolator<Vector3>(
                () => transform.position,
                v => transform.position = v,
                PropertyInterpolator.Vector3Lerp);
            rotationsLerp = new PropertyInterpolator<Vector2>(
                () => new Vector2(firstPersonCharacter.transform.localEulerAngles.x, transform.localEulerAngles.y),
                v =>
                {
                    var rot = transform.localEulerAngles;
                    transform.localEulerAngles = new Vector3(rot.x, v.y, rot.z);
					var coneRot = firstPersonCharacter.transform.localEulerAngles;
					coneRot.x = v.x;
					firstPersonCharacter.transform.localEulerAngles = coneRot;
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

            DamageDealt.Action += DamageDealt_Action;
            CubelingPickedUp.Action += CubelingPickedUp_Action;
            CubelingPickUpAcknowledged.Action += CubelingPickUpAcknowledged_Action;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (leftGame)
            {
                Object.Destroy(gameObject);
            }

            float dProgress = Time.deltaTime * Globals.LerpFactor;
            positionLerp.Update(dProgress);
            rotationsLerp.Update(dProgress);

            SelectedWeapon.Value.Update();
        }

        protected override void ChangeSizeDetails(float oldRadius, float newRadius)
        {
            var collider = GetComponent<CapsuleCollider>();
            collider.radius = newRadius;
            collider.height = 2 * newRadius;
		}

		public override void GotHit(GameObject shooter, Weapon weapon, int damageValue)
		{
			var player = shooter.GetComponent<Player>();
			player.DealDamageTo(this, weapon, damageValue);
			Debug.Log(GetComponent<Collider>().name + " got hit with " + weapon.Name + " for " + damageValue + " damage");
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

        void DamageDealt_Action(Damage damage)
        {
            Player.GetDamaged(damage);
        }

        void CubelingPickedUp_Action(PickUp pickUp)
        {
            Player.AcknowledgePickUp(pickUp);
        }

        void CubelingPickUpAcknowledged_Action(PickUpAcknowledge pickUpAcknowledge)
        {
            BaseCubeling cubeling = CubelingsById[pickUpAcknowledge.Id];
            SpawnedCubelings.Remove(cubeling);
            CubelingsById.Remove(pickUpAcknowledge.Id);
            Destroy(cubeling.gameObject);
        }

        public void SetLeftGame()
        {
            leftGame = true;
        }

        public EnemyCubeling SpawnCubeling(Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            GameObject cubelingObject = (GameObject)Instantiate(
                Globals.Instance.enemyCubelingPrefab,
                position, rotation);
            cubelingObject.GetComponent<Rigidbody>().velocity = velocity;
            EnemyCubeling cubeling = cubelingObject.GetComponent<EnemyCubeling>();
            cubeling.Spawner = this;
            return cubeling;
        }
    }
}
