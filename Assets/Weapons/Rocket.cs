using NeonShooter.Utils;
using UnityEngine;
using NeonShooter.AppWarp;

namespace NeonShooter.Players.Weapons
{
	public class Rocket : Projectile
	{
		float explosionReach;
		protected override void OnTriggerEnter(Collider directHitCollider)
		{
            Vector3 hitPoint = Position[Access];
            Destroy(this.gameObject);


            var player = ParentWeapon.Player as Player;
            if (player == null)
                throw new System.Exception(string.Format(
                    "Neonshooter.Players.Player type expected, but got {0} (this should never happen).",
                    ParentWeapon.Player == null ? "NULL" : ParentWeapon.Player.GetType().ToString()));

            var rocketLauncher = ParentWeapon as RocketLauncher;
            if (rocketLauncher != null)
            {
				int dmg = this.ParentWeapon.Damage*this.CubeValue/this.ParentWeapon.AmmoCost;
                explodeRocket(player, hitPoint, directHitCollider, rocketLauncher.ExplosionReach, dmg);
            }

            player.SpawnCubelingsInPosition(hitPoint, CubeValue, ParentWeapon.DamageEffect);
		}

		private void explodeRocket(Player shooter, Vector3 hitPoint, Collider directHitCollider, float explosionReach, int dmg)
		{
			Collider[] hitColliders = Physics.OverlapSphere(hitPoint, explosionReach);
			for(int k = 0; k < hitColliders.Length; ++k)
			{
				foreach (GameObject target in appwarp.enemies.Values)
				{
					if (target.GetComponent<Collider>() == hitColliders[k].GetComponent<Collider>())
					{
						if (hitColliders[k] == directHitCollider)
						{
							shooter.enemyShot(ParentWeapon, target, dmg);
						}
						else
						{
							float distanceFromExplosion = Vector3.Distance(hitPoint, hitColliders[k].ClosestPointOnBounds(hitPoint));
							Debug.Log("Explosion distance from " + hitColliders[k].name + " is " + distanceFromExplosion);
							if (distanceFromExplosion < explosionReach)
							{
								shooter.enemyShot(ParentWeapon, target, (int)(dmg * (1.0f - (distanceFromExplosion / explosionReach))));
							}
						}
						break;
					}
				}
			}
		}
	}
}
