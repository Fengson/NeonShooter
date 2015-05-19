using NeonShooter.Utils;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeonShooter.Cube;
using NeonShooter.Players.Weapons;
using System;

namespace NeonShooter.Players
{
    public class Player : BasePlayer
    {
        float aimRotationSpeed = -90;
        public GameObject aim;



        /// <summary>
        /// Debug value - set to true to use any weapon regardless of the cost.
        /// </summary>
        public bool DEBUGCanUseAnyWeapon;

        public Player()
        {
            CellsInStructure = new NotifyingList<IVector3>();

            Position = NotifyingProperty<Vector3>.PublicGetPrivateSet(Access);
            Rotations = NotifyingProperty<Vector2>.PublicGetPrivateSet(Access);
            Rotations.ValueChanged += (oldVal, newVal) => RecalculateDirection();

            SelectedWeapon = NotifyingProperty<Weapon>.PublicGetPrivateSet(Access, DefaultWeapon);
            SelectedWeapon.ValueChanged += (oldVal, newVal) => ContinousFire[Access] = false;
            ContinousFire = NotifyingProperty<bool>.PublicGetPrivateSet(Access);

            LaunchedProjectiles = new NotifyingList<BaseProjectile>();

            DamageDealt = InvokableAction<Damage>.Private(Access);
        }

        protected override void OnAwake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            base.OnAwake();

            // TODO: For multiple client network testing, move somewhere else
            //Application.runInBackground = true;

            CubeStructure.CellChanged += CubeStructure_CellChanged;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            this.aim.transform.Rotate(new Vector3(0, 0, aimRotationSpeed * Time.deltaTime));
            if (aimRotationSpeed < -90)
                aimRotationSpeed = Mathf.Min(-90, aimRotationSpeed + Time.deltaTime * 500);

            Position[Access] = transform.position;

            var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            Rotations[Access] = new Vector2(
                cameraObject.transform.eulerAngles.x,
                cameraObject.transform.eulerAngles.y);

            SelectedWeapon.Value.Update();

            //TODO talk about this with Sushi & Arek - Grzesiek
            switch (SelectedWeapon.Value.FireType)
            {
                case FireType.Single:
                    if (Input.GetMouseButtonDown(0))
                        onShoot();
                    break;
                case FireType.Continous:
                    if (Input.GetMouseButtonDown(0))
                        onShootStart();
                    if (Input.GetMouseButton(0))
                        onShoot();
                    if (Input.GetMouseButtonUp(0))
                        onShootEnd();
                    break;
            }

            if (Input.GetKeyDown(KeyCode.X))
                ChangeWeaponToNext();
        }

        protected override void TriggerEnter(Collider other)
        {
            base.TriggerEnter(other);

            if (other.gameObject.CompareTag("Cubeling") && other.gameObject.GetComponent<IsCubelingPickabe>().pickable)
            {
                Destroy(other.gameObject);
                GainLife(1);
                if (sounds[0] != null)
                    GetComponent<AudioSource>().PlayOneShot(sounds[0].clip);
            }
        }

        void onShoot()
        {
            if (SelectedWeapon.Value.IsCoolingDown()) return;

            SelectedWeapon.Value.RaiseCooldown();

            int paidCost = (int)(SelectedWeapon.Value.AmmoCost * Mathf.Max(1, Mathf.Sqrt(CellsIncorporator.amount / 100)));
            CellsIncorporator.amount -= paidCost;
            if (CellsIncorporator.amount < 0) CellsIncorporator.amount = 0;

            SelectedWeapon.Value.shoot(this, paidCost);
            if (aimRotationSpeed > -1500)
                aimRotationSpeed -= Time.deltaTime * 100 * SelectedWeapon.Value.Damage;

            //this will switch weapon if there's not enough ammo for current weapon
            if (!CanUseWeapon(SelectedWeapon.Value))
                ChangeWeaponToNext();
        }

        void onShootStart()
        {
            ContinousFire[Access] = true;
            SelectedWeapon.Value.OnShootStart(this);
        }

        void onShootEnd()
        {
            ContinousFire[Access] = false;
            SelectedWeapon.Value.OnShootEnd();
        }

        public void enemyShot(Weapon weapon, GameObject enemy, int damage, int paidCost)
        {
            //damage bonus for being big
            damage += (int)(8 * Mathf.Sqrt(paidCost));
            //return lost cost and add what was taken
            //CellsIncorporator.amount += damage + costPayed;
            EnemyPlayer enemyScript = enemy.GetComponent<EnemyPlayer>();
            DamageDealt.Invoke(new Damage(this, enemyScript, damage, weapon.DamageEffect), Access);
            //if (weapon.DamageEffect == DamageEffect.Suction)
            //{
            //    Instantiate(cubelingPrefab, transform.localPosition + cell.Value, transform.rotation);
            //}
            //GainLife(damage); // <--- TEMP

            Debug.Log(enemy.GetComponent<Collider>().name + " got shot with " + weapon.getWeaponName() + " for " + damage + " damage");
            //TODO destroy enemy cubes - available to collect, play sound and cast animations depending on weapon
        }

        void ChangeWeaponToNext()
        {
            var index = Weapons.IndexOf(SelectedWeapon.Value);
            for (int i = 0; i < Weapons.Count; i++)
            {
                index = (index + 1) % Weapons.Count;
                var candidate = Weapons[index];
                if (CanUseWeapon(candidate))
                {
                    SelectedWeapon[Access] = candidate;
                    break;
                }
            }
        }

        bool CanUseWeapon(Weapon weapon)
        {
            return DEBUGCanUseAnyWeapon || weapon == DefaultWeapon ||
                CellsIncorporator.amount >= weapon.lifeRequiredToOwn();
        }

        public void GainLife(int amount)
        {
            CubeStructure.AppendCells(amount);
        }

        protected override void ChangeSizeDetails(float oldRadius, float newRadius)
        {
            var collider = GetComponent<CharacterController>();
            collider.radius = newRadius;
            collider.height = 2 * newRadius;

            var character = GameObject.FindGameObjectWithTag("CameraScale");
            character.transform.localScale = Vector3.one * newRadius * 2;
        }

        void CubeStructure_CellChanged(IVector3 position, bool cellValue)
        {
            if (cellValue) CellsInStructure.Add(position);
            else CellsInStructure.Remove(position);
        }
    }
}