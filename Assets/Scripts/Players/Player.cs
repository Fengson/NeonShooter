using NeonShooter.Utils;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeonShooter.Cube;
using NeonShooter.Players.Weapons;
using System;

namespace NeonShooter.Players
{
    public class Player : MonoBehaviour, IPlayer
    {
        readonly System.Object access;

        CubeStructure cubeStructure;

        Weapon defaultWeapon;
        List<Weapon> weapons;

        public AudioSource[] sounds;

        public int initialRadius;

        float aimRotationSpeed = -90;
        public GameObject aim;

        public GameObject projectilePrefab;
        public GameObject railGunShotPrefab;
        public GameObject vacuumConePrefab;
        public GameObject cubelingPrefab;

        /// <summary>
        /// Debug value - set to true to use any weapon regardless of the cost.
        /// </summary>
        public bool DEBUGCanUseAnyWeapon;

        public int Life { get { return cubeStructure.Count; } }

        public NotifyingProperty<Vector3> Position { get; private set; }
        public NotifyingProperty<Vector2> Rotations { get; private set; }
        public NotifyingProperty<Vector3> Direction { get; private set; }

        public NotifyingProperty<Weapon> SelectedWeapon { get; private set; }
        public NotifyingProperty<bool> ContinuousFire { get; private set; }

        public NotifyingList<Projectile> LaunchedProjectiles { get; private set; }

        public Player()
        {
            defaultWeapon = new VacuumWeapon();
            weapons = new List<Weapon> { defaultWeapon, new RailGun(), new RocketLauncher() };
            
            access = new object();
            Position = NotifyingProperty<Vector3>.PublicGetPrivateSet(access);
            Rotations = NotifyingProperty<Vector2>.PublicGetPrivateSet(access);
            Direction = NotifyingProperty<Vector3>.PublicGetPrivateSet(access);

            SelectedWeapon = NotifyingProperty<Weapon>.PublicGetPrivateSet(access, defaultWeapon);
            ContinuousFire = NotifyingProperty<bool>.PublicGetPrivateSet(access);

            LaunchedProjectiles = new NotifyingList<Projectile>();
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // TODO: For multiple client network testing, move somewhere else
            //Application.runInBackground = true;

            int radius = Math.Max(1, initialRadius);
            cubeStructure = new CubeStructure(gameObject, radius);
            ChangeSize(1, radius);
            cubeStructure.RadiusChanged += ChangeSize;
        }

        void Update()
        {
            this.aim.transform.Rotate(new Vector3(0, 0, aimRotationSpeed * Time.deltaTime));
            if (aimRotationSpeed < -90)
            {
                aimRotationSpeed = Mathf.Min(-90, aimRotationSpeed + Time.deltaTime * 500);
            }
            Position[access] = transform.position;

            var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            Rotations[access] = new Vector2(
                cameraObject.transform.eulerAngles.x,
                cameraObject.transform.eulerAngles.y);
            Direction[access] = Quaternion.Euler(Rotations.Value.x, Rotations.Value.y, 0) * Vector3.forward;

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
            ContinuousFire[access] = true;
            SelectedWeapon.Value.ShootStart(this);
        }

        void onShootEnd()
        {
            ContinuousFire[access] = false;
            SelectedWeapon.Value.ShootEnd();
        }

        public void enemyShot(Weapon weapon, GameObject enemy, int damage, int paidCost)
        {
            //damage bonus for being big
            damage += (int)(8 * Mathf.Sqrt(paidCost));
            //return lost cost and add what was taken
            //CellsIncorporator.amount += damage + costPayed;
            enemy.GetComponent<IPlayer>().DealDamage(damage, weapon.DamageEffect);
            GainLife(damage); // <--- TEMP

            Debug.Log(enemy.GetComponent<Collider>().name + " got shot with " + weapon.getWeaponName() + " for " + damage + " damage");
            //TODO destroy enemy cubes - available to collect, play sound and cast animations depending on weapon
        }

        void ChangeWeaponToNext()
        {
            var index = weapons.IndexOf(SelectedWeapon.Value);
            for (int i = 0; i < weapons.Count; i++)
            {
                index = (index + 1) % weapons.Count;
                var candidate = weapons[index];
                if (CanUseWeapon(candidate))
                {
                    ContinuousFire[access] = false;
                    SelectedWeapon[access] = candidate;
                    break;
                }
            }
        }

        bool CanUseWeapon(Weapon weapon)
        {
            return DEBUGCanUseAnyWeapon || weapon == defaultWeapon ||
                CellsIncorporator.amount >= weapon.lifeRequiredToOwn();
        }

        public void GainLife(int amount)
        {
            cubeStructure.AppendCells(amount);
        }

        public void DealDamage(int amount, DamageEffect damageEffect)
        {
            List<IVector3> cubelingPositions = cubeStructure.RetrieveCells(amount);
            //foreach (var v in cubelingPositions)
            //{
            //    Instantiate(cubelingPrefab, transform.localPosition + v, transform.rotation);
            //}
        }

        void ChangeSize(int currentRadius, int newRadius)
        {
            var pos = transform.position;
            var currentR = CalculateColliderRadius(currentRadius);
            var newR = CalculateColliderRadius(newRadius);
            var dr = newR - currentR;

            var collider = GetComponent<CharacterController>();
            collider.radius = newR;
            collider.height = 2 * newR;
            transform.position += Vector3.up * dr;

            var character = GameObject.FindGameObjectWithTag("CameraScale");
            character.transform.localScale = Vector3.one * newR * 2;
        }

        float CalculateColliderRadius(int structureRadius)
        {
            return Mathf.Max(0, structureRadius - 0.5f);
        }
    }
}