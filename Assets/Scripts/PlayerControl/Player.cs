using NeonShooter.Utils;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NeonShooter.PlayerControl
{
    public class Player : MonoBehaviour, IPlayer
    {
        readonly System.Object access;

        Weapon defaultWeapon;
        List<Weapon> weapons;

        public AudioSource[] sounds;

        float aimRotationSpeed = -90;
        public GameObject aim;

        public GameObject projectilePrefab;
        public GameObject railGunShotPrefab;

        public NotifyingProperty<Vector3> Position { get; private set; }
        public NotifyingProperty<Vector2> Rotations { get; private set; }
        public NotifyingProperty<Vector3> Direction { get; private set; }

        public NotifyingProperty<Weapon> SelectedWeapon { get; private set; }

        public NotifyingList<Projectile> LaunchedProjectiles { get; private set; }

        public Player()
        {
            defaultWeapon = new VacuumWeapon();
            weapons = new List<Weapon> { defaultWeapon, new RailGun(), new RocketLauncher() };

            // TODO: For multiple client network testing, move somewhere else
            //Application.runInBackground = true;

            access = new object();
            Position = NotifyingProperty<Vector3>.PublicGetPrivateSet(access);
            Rotations = NotifyingProperty<Vector2>.PublicGetPrivateSet(access);
            Direction = NotifyingProperty<Vector3>.PublicGetPrivateSet(access);

            SelectedWeapon = NotifyingProperty<Weapon>.PublicGetPrivateSet(access, defaultWeapon);

            LaunchedProjectiles = new NotifyingList<Projectile>();
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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

            //TODO talk about this with Sushi & Arek - Grzesiek
            if (Input.GetMouseButtonDown(0))
                StartCoroutine(onShoot());

            if (Input.GetKeyDown(KeyCode.X))
                ChangeWeaponToNext();
        }

        bool shooting = false;
        IEnumerator onShoot()
        {
            if (!shooting)
            {
                shooting = true;
                int costPayed = (int)(SelectedWeapon.Value.AmmoCost * Mathf.Max(1, Mathf.Sqrt(CellsIncorporator.amount / 100)));
                CellsIncorporator.amount -= costPayed;

                SelectedWeapon.Value.shoot(this, costPayed);
                if (aimRotationSpeed > -1500)
                    aimRotationSpeed -= Time.deltaTime * 100 * SelectedWeapon.Value.Damage;

                //this will switch weapon if theres not enough ammo for current weapon
                if (!CanUseWeapon(SelectedWeapon.Value))
                    ChangeWeaponToNext();

                yield return new WaitForSeconds(0.1f);
                shooting = false;
            }
        }

        public void enemyShot(Weapon weapon, GameObject enemy, int damage, int costPayed)
        {
            //damage bonus for being big
            damage += (int)(8 * Mathf.Sqrt(costPayed));
            //return lost cost and add what was taken
            //CellsIncorporator.amount += damage + costPayed;

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
                    SelectedWeapon[access] = candidate;
                    break;
                }
            }
        }

        bool CanUseWeapon(Weapon weapon)
        {
            return weapon == defaultWeapon ||
                CellsIncorporator.amount >= weapon.lifeRequiredToOwn();
        }
    }
}