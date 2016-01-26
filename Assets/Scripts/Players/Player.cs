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

            Position = new NotifyingProperty<Vector3>();
            Rotations = new NotifyingProperty<Vector2>();
            Rotations.ValueChanged += (oldVal, newVal) => RecalculateDirection();

            SelectedWeapon = new NotifyingProperty<Weapon>(DefaultWeapon);
            SelectedWeapon.ValueChanged += (oldVal, newVal) => ContinousFire.Value = false;
            ContinousFire = new NotifyingProperty<bool>();

            LaunchedProjectiles = new NotifyingList<BaseProjectile>();
            SpawnedCubelings = new NotifyingList<BaseCubeling>();

            DamageDealt = new InvokableAction<Damage>();
            CubelingPickedUp = new InvokableAction<PickUp>();
            CubelingPickUpAcknowledged = new InvokableAction<PickUpAcknowledge>();
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

            if (aimRotationSpeed < -90)
                aimRotationSpeed = Mathf.Min(-90, aimRotationSpeed + Time.deltaTime * 500);
            this.aim.transform.Rotate(new Vector3(0, 0, aimRotationSpeed * Time.deltaTime));

            Position.Value = transform.position;

            var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            Rotations.Value = new Vector2(
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
                    else if (Input.GetMouseButton(0))
                        OnShoot();
                    else if (Input.GetMouseButtonUp(0))
                        OnShootEnd();
					else {
//						UnityStandardAssets.ImageEffects.BloomOptimized lvScript = GetComponentsInChildren<UnityStandardAssets.ImageEffects.BloomOptimized>()[0];
//						if (lvScript.threshold > 0.0f) {
//							lvScript.threshold -= 0.05f;
//						}
						
//						if (lvScript.intensity > 0.0f) {
//							lvScript.intensity -= 0.15f;
//						}
					}
					break;
				default:
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
						Cubeling lvCubeling = other.GetComponent<Cubeling> ();
                        Destroy(other.gameObject);
						GainLife(lvCubeling.size);
                        if (sounds[0] != null)
                            GetComponent<AudioSource>().PlayOneShot(sounds[0].clip);
                    }
                    else if (cubeling is EnemyCubeling)
                    {
                        CubelingPickedUp.Invoke(new PickUp(cubeling.Spawner, this, cubeling.Id));
                    }
                }
            }
        }

        void OnShoot()
        {
            if (SelectedWeapon.Value.IsCoolingDown) return;

            SelectedWeapon.Value.RaiseCooldown();

            //include all bonuses to ammo/damage cost (being big = more powerful shot)
            int paidCost = SelectedWeapon.Value.GetCalculatedAmmoCost(Life);

            //pay with players life for shoot
            CubeStructure.RetrieveCells(paidCost);

            //shoot, for example create projectile
            SelectedWeapon.Value.Shoot(this, paidCost);

            //crosshair rotation boost on shoot - for continous fire and otherwise
            if (SelectedWeapon.Value.AmmoCost==0)
            {
                aimRotationSpeed -= Time.deltaTime * 1000;
            } else
            {
                aimRotationSpeed = Mathf.Max(-1500, aimRotationSpeed - Time.deltaTime * 2000 * paidCost);
            }

            //this will switch weapon if there's not enough ammo for current weapon
            if (!CanUseWeapon(SelectedWeapon.Value))
                ChangeWeaponToNext();

			//UnityStandardAssets.ImageEffects.BloomOptimized lvScript = GetComponentsInChildren<UnityStandardAssets.ImageEffects.BloomOptimized>()[0];
			//if (lvScript.threshold <= 0.25f) {
			//	lvScript.threshold += 0.05f;
			//}

			//if (lvScript.intensity <= 0.75f) {
			//	lvScript.intensity += 0.15f;
			//}
        }

        void OnShootStart()
        {
            ContinousFire.Value = true;
            SelectedWeapon.Value.OnShootStart(this);

			//UnityStandardAssets.ImageEffects.ScreenOverlay lvScript = GetComponentsInChildren<UnityStandardAssets.ImageEffects.ScreenOverlay>()[0];
			//lvScript.enabled = true;
			//lvScript.threshold = 0;
			//lvScript.intensity = 0;
        }

        void OnShootEnd()
        {
            ContinousFire.Value = false;
            SelectedWeapon.Value.OnShootEnd();
		}

		public override void GotHit(GameObject shooter, Weapon weapon, int damageValue)
		{
            GetDamaged(new Damage(shooter.GetComponent<BasePlayer>(), this, damageValue, CubelingSpawnEffect.Scatter));
		}

		public void DealDamageTo(EnemyPlayer enemy, Weapon weapon, int damageValue)
        {
            DamageDealt.Invoke(new Damage(this, enemy, damageValue, weapon.DamageEffect));
        }

        public void ChangeWeaponToNext()
        {
            this.OnShootEnd();
            var index = Weapons.IndexOf(SelectedWeapon.Value);
            for (int i = 0; i < Weapons.Count; i++)
            {
                index = (index + 1) % Weapons.Count;
                var candidate = Weapons[index];
                if (CanUseWeapon(candidate))
                {
                    SelectedWeapon.Value = candidate;
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
            SpawnCubelings(cubelingPositions, oldPosition, damage.Effect, (EnemyPlayer)damage.Source);
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
            CubelingPickUpAcknowledged.Invoke(new PickUpAcknowledge(pickUp, accepted));
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

        public void SpawnCubelings(List<IVector3> relativePositions, Vector3 absolutePosition, CubelingSpawnEffect effect, EnemyPlayer shooter = null)
        {
			SpawnCubelingsInFrontOfPlayer(relativePositions.Capacity, effect);
        }

        public void SpawnCubeling(Vector3 position, Vector3 scatterVelocity, CubelingSpawnEffect effect, int pmSize)
        {
			GameObject cubelingObject;

			switch (pmSize) 
			{
				case 5:
					cubelingObject = (GameObject)Instantiate(
					Globals.Instance.playerCubelingPrefabSize5,
					position, transform.rotation);
					break;
				case 15:
				cubelingObject = (GameObject)Instantiate(
					Globals.Instance.playerCubelingPrefabSize15,
					position, transform.rotation);
					break;
				case 25:
					cubelingObject = (GameObject)Instantiate(
					Globals.Instance.playerCubelingPrefabSize25,
					position, transform.rotation);
					break;
				case 1:
				default:
				cubelingObject = (GameObject)Instantiate(
					Globals.Instance.playerCubelingPrefab,
					position, transform.rotation);
				break;
				break;
			}

            
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

        public void SpawnCubelingsInFrontOfPlayer(int cubelingsAmount, CubelingSpawnEffect effect)
        {
            Vector3 cubelingsSpawnPosition = Position.Value+Direction*5;
 
			SpawnChunkCubelings (cubelingsSpawnPosition, Direction, cubelingsAmount, effect);
        }

        public void SpawnCubelingsInPosition(Vector3 cubelingsSpawnPosition, int cubelingsAmount, CubelingSpawnEffect effect)
        {
            //assigning 0 velocity with cubelings spawned all in same position will take effect in random cubelings collisions, giving nice explosion effect
            Vector3 velocity = new Vector3();

			SpawnChunkCubelings (cubelingsSpawnPosition, velocity, cubelingsAmount, effect);
        }

		public void SpawnChunkCubelings(Vector3 pmCubelingsSpawnPosition, Vector3 pmVelocity, int pmCubelingsAmount, CubelingSpawnEffect pmEffect)
		{
			int lvCubelingsAmount = pmCubelingsAmount;
			int lvCubes25Amount = calculateCubelingsAmount (lvCubelingsAmount, 25);

			// pomniejszamy ogolna liczbe kostek o kostki, ktore weszly do puli kostek o wartosci 25
			lvCubelingsAmount -= lvCubes25Amount * 25;

			if (lvCubelingsAmount == 0 && lvCubes25Amount > 0) {
				lvCubes25Amount --;
				lvCubelingsAmount += 25;
			}

			int lvCubes15Amount = calculateCubelingsAmount (lvCubelingsAmount, 15);

			// pomniejszamy ogolna liczbe kostek o kostki, ktore weszly do puli kostek o wartosci 15
			lvCubelingsAmount -= lvCubes15Amount * 15;

			if (lvCubelingsAmount == 0 && lvCubes15Amount > 0) {
				lvCubes15Amount --;
				lvCubelingsAmount += 15;
			}


			int lvCubes5Amount = calculateCubelingsAmount (lvCubelingsAmount, 5);

			// pomniejszamy ogolna liczbe kostek o kostki, ktore weszly do puli kostek o wartosci 15
			lvCubelingsAmount -= lvCubes5Amount * 5;

			if (lvCubelingsAmount == 0 && lvCubes5Amount > 0) {
				lvCubes5Amount --;
				lvCubelingsAmount += 5;
			}

			for (int i = 0; i < lvCubes25Amount; i++) {
				SpawnCubeling (pmCubelingsSpawnPosition, pmVelocity, pmEffect,25);
			}

			for (int i = 0; i < lvCubes15Amount; i++) {
				SpawnCubeling (pmCubelingsSpawnPosition, pmVelocity, pmEffect,15);
			}

			for (int i = 0; i < lvCubes5Amount; i++) {
				SpawnCubeling (pmCubelingsSpawnPosition, pmVelocity, pmEffect,5);
			}

			for (int i = 0; i < lvCubelingsAmount; i++) {
				SpawnCubeling (pmCubelingsSpawnPosition, pmVelocity, pmEffect,1);
			}
		}

		/*
		 * Metoda oblicza ile kostek danej wielkosci mozna stworzyc z danej liczby kostek
		 */
		private int calculateCubelingsAmount(int pmCubelingAmount, int pmCubelingSize)
		{
			return (pmCubelingAmount - (pmCubelingAmount % pmCubelingSize))/pmCubelingSize;
		}

		void OnTriggerEnter(Collider other)
		{
			if(other.CompareTag("SkyBox")){ this.respawn(); }
		}

		public void respawn()
		{
			GameObject[] spawns = GameObject.FindGameObjectsWithTag("PlayerRespawn");
			if (spawns.Length > 0) {
				int spawn_index = Mathf.RoundToInt (Random.Range (0.0f, spawns.Length - 1.0f));
				GameObject spawn = spawns [spawn_index];
				this.transform.position = spawn.transform.position;
			} else {
				throw new System.Exception("Map has to contain at least one Player Spawn Point!");
			}
		}
    }
}