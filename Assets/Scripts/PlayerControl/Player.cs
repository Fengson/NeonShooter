using NeonShooter.Utils;
using UnityEngine;
using System.Collections;

namespace NeonShooter.PlayerControl
{
    public class Player : MonoBehaviour
    {
        private readonly System.Object access;

        public NotifyingProperty<Vector3> Position { get; private set; }
        public NotifyingProperty<Vector2> Rotations { get; private set; }
        public NotifyingProperty<Vector3> Direction { get; private set; }

        public InvokableAction<object> OnShootStart { get; private set; }
        public InvokableAction<object> OnShootEnd { get; private set; }

        float aimRotationSpeed = -90;
        public GameObject aim;
        public GameObject TEMP_enemy;
        IPlayer TEMP_enemyScript;

        public Player()
        {
            access = new object();

            Position = new NotifyingProperty<Vector3>(access, true, false);
            Rotations = new NotifyingProperty<Vector2>(access, true, false);
            Direction = new NotifyingProperty<Vector3>(access, true, false);

            OnShootStart = new InvokableAction<object>(access);
            OnShootEnd = new InvokableAction<object>(access);
        }

        void Start()
        {
            TEMP_enemyScript = TEMP_enemy.GetComponent<EnemyPlayer>();
            OnShootStart.Action += TEMP_OnShootStart_Action;
            OnShootEnd.Action += TEMP_OnShootEnd_Action;
        }

        void Update()
        {
			this.aim.transform.Rotate (new Vector3 (0,0,aimRotationSpeed * Time.deltaTime));
            if(aimRotationSpeed<-90)
                aimRotationSpeed+=Time.deltaTime*200;
            Position[access] = transform.position;

            var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            Rotations[access] = new Vector2(
                cameraObject.transform.eulerAngles.x,
                cameraObject.transform.eulerAngles.y);
            Direction[access] = Quaternion.Euler(Rotations.Value.x, Rotations.Value.y, 0) * Vector3.forward;

            //TODO talk about this with Sushi & Arek - Grzesiek
            if (Input.GetMouseButtonDown(0))
                if (OnShootStart != null)
                    OnShootStart.Invoke(null, access);
            if (Input.GetMouseButtonUp(0))
                if (OnShootEnd != null)
                    OnShootEnd.Invoke(null, access);

            if (Input.GetKey(KeyCode.X)) {
                StartCoroutine(changeToNextWeapon());
            }

            TEMP_enemyScript.Position.Value = Position.Value + new Vector3(5, 0, 0);
            TEMP_enemyScript.Rotations.Value = new Vector2(Rotations.Value.x, -Rotations.Value.y);
        }

        void TEMP_OnShootStart_Action(object obj)
        {
            StartCoroutine(onShoot());
        }

        void TEMP_OnShootEnd_Action(object obj)
        {
            TEMP_enemyScript.OnShootEnd.Invoke(null);
        }

        bool shooting = false;
        IEnumerator onShoot() {
            if(!shooting) {
                shooting=true;
                int costPayed = (int)(CellsIncorporator.selectedWeapon.AmmoCost*Mathf.Max(1,Mathf.Sqrt(CellsIncorporator.amount/100)));
                CellsIncorporator.amount -= costPayed;

                CellsIncorporator.selectedWeapon.shoot(this, this.TEMP_enemyScript, costPayed);
			    if (aimRotationSpeed > -3000)
				    aimRotationSpeed -= Time.deltaTime*30*CellsIncorporator.selectedWeapon.Damage;
                //this will switch weapon if theres not enough ammo for current weapon
                changeWeapon(CellsIncorporator.selectedWeapon);
                TEMP_enemyScript.OnShootStart.Invoke(null);
                //TODO play sound and cast animations depending on weapon
                yield return new WaitForSeconds(0.1f);
                shooting=false;
            }
        }

        public void enemyShot(Weapon weapon, Collider target, int damage, int costPayed) {
            //damage bonus for being big
            damage += (int)(8*Mathf.Sqrt(costPayed));
            //return lost cost and add what was taken
		    CellsIncorporator.amount += damage + costPayed;

			Debug.Log(target.name+" got shot with "+weapon.getWeaponName() + " for "+damage+" damage");
            GameObject enemy = target.gameObject;
           //TODO take enemy life, play sound and cast animations depending on weapon
        }

        bool changingWeapon = false;
        IEnumerator changeToNextWeapon() {
            if(!changingWeapon) {
                changingWeapon=true;
                changeWeapon(CellsIncorporator.selectedWeapon.nextWeapon());
                yield return new WaitForSeconds(0.1f);
                changingWeapon=false;
            }
        }

        /**
        changes weapon to set, or next if not available
        */
        void changeWeapon(Weapon weapon) {
            if(CellsIncorporator.amount>weapon.lifeRequiredToOwn()) {
                CellsIncorporator.selectedWeapon=weapon;
            } else {
                changeWeapon(weapon.nextWeapon());
            }
        }
    }
}