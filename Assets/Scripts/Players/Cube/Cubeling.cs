using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Cube
{
    public class Cubeling : BaseCubeling
    {
        public float SpawnerPickDelay { get; set; }
        public EnemyPlayer TargetPlayer { get; set; }

        public override bool Pickable { get { return SpawnerPickDelay == 0; } }

        private Cubeling()
        {
            Id = System.DateTime.UtcNow.Ticks;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            SpawnerPickDelay = Mathf.Max(0, SpawnerPickDelay - Time.deltaTime);

            if (TargetPlayer != null)
            {
                float step = Globals.CubelingSuckSpeed * Time.deltaTime;

                transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.transform.position, step);
                transform.eulerAngles = new Vector3(1, 1, 1); // TODO: what is this - fixed orientation 1 deg around every axis? what's the purpose?
            }
			else if(this.transform.position.y < -150){ this.respawn(); }

            Position.Value = transform.position;
            Rotation.Value = transform.rotation;
            Velocity.Value = GetComponent<Rigidbody>().velocity;
        }

		protected void respawn()
		{
			GameObject[] spawns = GameObject.FindGameObjectsWithTag("CubelingRespawn");
			if(spawns.Length > 0)
			{
				int spawn_index = Mathf.RoundToInt(Random.Range(0.0f, spawns.Length-1.0f));
				GameObject spawn = spawns[spawn_index];
				this.transform.position = spawn.transform.position;
			}
			GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
		}
    }
}
