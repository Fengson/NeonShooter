using NeonShooter.Utils;
using UnityEngine;

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

        public GameObject TEMP_enemy;
        IPlayer TEMP_enemyScript;

		protected Weapon selectedWeapon;

        public Player()
        {
            access = new object();

            Position = new NotifyingProperty<Vector3>(access, true, false);
            Rotations = new NotifyingProperty<Vector2>(access, true, false);
            Direction = new NotifyingProperty<Vector3>(access, true, false);

            OnShootStart = new InvokableAction<object>(access);
            OnShootEnd = new InvokableAction<object>(access);

			selectedWeapon = new Weapon (100, 0, (float)(4.0/18.0*Mathf.PI), 10);
        }

        void Start()
        {
            TEMP_enemyScript = TEMP_enemy.GetComponent<EnemyPlayer>();
            OnShootStart.Action += TEMP_OnShootStart_Action;
            OnShootEnd.Action += TEMP_OnShootEnd_Action;
        }

        void Update()
        {
            Position[access] = transform.position;

            var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            Rotations[access] = new Vector2(
                cameraObject.transform.eulerAngles.x,
                cameraObject.transform.eulerAngles.y);
            Direction[access] = Quaternion.Euler(Rotations.Value.x, Rotations.Value.y, 0) * Vector3.forward;

            if (Input.GetMouseButtonDown(0))
                if (OnShootStart != null)
                    OnShootStart.Invoke(null, access);
            if (Input.GetMouseButtonUp(0))
                if (OnShootEnd != null)
                    OnShootEnd.Invoke(null, access);

            TEMP_enemyScript.Position.Value = Position.Value + new Vector3(5, 0, 0);
            TEMP_enemyScript.Rotations.Value = new Vector2(Rotations.Value.x, -Rotations.Value.y);
        }

        void TEMP_OnShootStart_Action(object obj)
        {
			CellsIncorporator.amount -= this.selectedWeapon.AmmoCost;
			if(this.selectedWeapon.shoot(this, this.TEMP_enemyScript)){ CellsIncorporator.amount += this.selectedWeapon.Damage; }
            TEMP_enemyScript.OnShootStart.Invoke(null);
        }

        void TEMP_OnShootEnd_Action(object obj)
        {
            TEMP_enemyScript.OnShootEnd.Invoke(null);
        }
    }
}