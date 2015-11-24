using NeonShooter.Utils;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeonShooter.Players.Weapons;
using NeonShooter.Players.Cube;

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

        private Player()
        {
            CellsInStructure = new NotifyingList<IVector3>();

            Position = NotifyingProperty<Vector3>.PublicGetPrivateSet(Access);
            Rotations = NotifyingProperty<Vector2>.PublicGetPrivateSet(Access);
            Rotations.ValueChanged += (oldVal, newVal) => RecalculateDirection();

            SelectedWeapon = NotifyingProperty<Weapon>.PublicGetPrivateSet(Access, DefaultWeapon);
            SelectedWeapon.ValueChanged += (oldVal, newVal) => ContinousFire[Access] = false;
            ContinousFire = NotifyingProperty<bool>.PublicGetPrivateSet(Access);

            LaunchedProjectiles = new NotifyingList<BaseProjectile>();
            SpawnedCubelings = new NotifyingList<BaseCubeling>();

            DamageDealt = InvokableAction<Damage>.Private(Access);
            CubelingPickedUp = InvokableAction<PickUp>.Private(Access);
            CubelingPickUpAcknowledged = InvokableAction<PickUpAcknowledge>.Private(Access);
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
                        OnShoot();
                    break;
                case FireType.Continous:
                    if (Input.GetMouseButtonDown(0))
                        OnShootStart();
                    if (Input.GetMouseButton(0))
                        OnShoot();
                    if (Input.GetMouseButtonUp(0))
                        OnShootEnd();
                    break;
            }

            if (Input.GetKeyDown(KeyCode.X))
                ChangeWeaponToNext();
        }

        protected override void TriggerEnter(Collider other)
        {
            base.TriggerEnter(other);

            if (other.CompareTag("Cubeling"))
            {
                var cubeling = other.GetComponent<BaseCubeling>();
                if (cubeling != null && cubeling.Pickable)
                {
                    // TODO: polymorphic
                    if (cubeling is Cubeling)
                    {
                        Destroy(other.gameObject);
                        GainLife(1);
                        if (sounds[0] != null)
                            GetComponent<AudioSource>().PlayOneShot(sounds[0].clip);
                    }
                    else if (cubeling is EnemyCubeling)
                    {
                        CubelingPickedUp.Invoke(new PickUp(cubeling.Spawner, this, cubeling.Id), Access);
                    }
                }
            }
        }

        void OnShoot()
        {
            if (SelectedWeapon.Value.IsCoolingDown) return;

            SelectedWeapon.Value.RaiseCooldown();

            //int paidCost = (int)(SelectedWeapon.Value.AmmoCost * Mathf.Max(1, Mathf.Sqrt(CellsIncorporator.amount / 100)));
            //CellsIncorporator.amount -= paidCost;
            //if (CellsIncorporator.amount < 0) CellsIncorporator.amount = 0;

            SelectedWeapon.Value.Shoot(this, 1);//paidCost);
            if (aimRotationSpeed > -1500)
                aimRotationSpeed -= Time.deltaTime * 100 * SelectedWeapon.Value.Damage;

            //this will switch weapon if there's not enough ammo for current weapon
            if (!CanUseWeapon(SelectedWeapon.Value))
                ChangeWeaponToNext();
        }

        void OnShootStart()
        {
            ContinousFire[Access] = true;
            SelectedWeapon.Value.OnShootStart(this);
        }

        void OnShootEnd()
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

            Debug.Log(enemy.GetComponent<Collider>().name + " got shot with " + weapon.Name + " for " + damage + " damage");
            //TODO destroy enemy cubes - available to collect, play sound and cast animations depending on weapon
        }

        void ChangeWeaponToNext()
        {
            this.OnShootEnd();
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
                Life >= weapon.LifeRequiredToOwn;
        }

        public void GainLife(int amount)
        {
			int oldRadius = this.CubeStructure.Radius;
			CubeStructure.AppendCells(amount);
			
			int newRadius = this.CubeStructure.Radius;
			
			if (oldRadius != newRadius) 
			{
				gameObject.GetComponent<CharacterController>().height = newRadius;
				gameObject.GetComponent<CharacterController>().radius = newRadius;
			}

        }

        public void GetDamaged(Damage damage)
        {
            Vector3 oldPosition = transform.position;
            List<IVector3> cubelingPositions = CubeStructure.RetrieveCells(damage.Amount);
            SpawnCubelings(cubelingPositions, oldPosition, damage.Effect);
        }

        public void AcknowledgePickUp(PickUp pickUp)
        {
            bool accepted = false;
            lock (CubelingsById)
            {
                BaseCubeling cubeling = CubelingsById.TryGet(pickUp.Id);
                if (cubeling != null)
                {
                    SpawnedCubelings.Remove(cubeling);
                    CubelingsById.Remove(pickUp.Id);
                    Destroy(cubeling.gameObject);
                    accepted = true;
                }
            }
            CubelingPickUpAcknowledged.Invoke(new PickUpAcknowledge(pickUp, accepted), Access);
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

        private void SpawnCubelings(List<IVector3> relativePositions, Vector3 absolutePosition, CubelingSpawnEffect effect)
        {
            foreach (IVector3 p in relativePositions)
                SpawnCubeling(p + absolutePosition, p * Globals.CubelingScatterVelocityFactor, effect);
        }

        public void SpawnCubeling(Vector3 position, Vector3 scatterVelocity, CubelingSpawnEffect effect)
        {
            GameObject cubelingObject = (GameObject)Instantiate(
                Globals.Instance.playerCubelingPrefab,
                position, transform.rotation);
            Cubeling cubeling = cubelingObject.GetComponent<Cubeling>();
            CubelingsById[cubeling.Id] = cubeling;
            cubeling.Spawner = this;
            SpawnedCubelings.Add(cubeling);
            switch (effect)
            {
                case CubelingSpawnEffect.Scatter:
                    cubelingObject.GetComponent<Rigidbody>().velocity = scatterVelocity;
                    break;
                case CubelingSpawnEffect.FlyToPlayer:
                    cubeling.SpawnerPickDelay = Globals.CubelingSpawnerPickDelay;
                    break;
            }
        }
    }
}