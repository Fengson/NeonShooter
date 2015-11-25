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
			Collider[] hitColliders = Physics.OverlapSphere(this.Position.Value, explosionReach);
			for(int k = 0; k < hitColliders.Length; ++k)
			{
				foreach (GameObject target in appwarp.enemies.Values)
				{
					if (target.GetComponent<Collider>() == hitColliders[k].GetComponent<Collider>())
					{
						var shooter = this.ParentWeapon.Player as Player;
						var dmg = this.ParentWeapon.Damage;
						if (hitColliders[k] == directHitCollider)
						{
							shooter.enemyShot(this, target, dmg, paidCost);
						}
						else
						{
							float distanceFromExplosion = Vector3.Distance(this.Position, hitColliders[k].ClosestPointOnBounds(this.Position));
							Debug.Log("Explosion distance from " + hitColliders[k].name + " is " + distanceFromExplosion);
							if (distanceFromExplosion < explosionReach)
							{
								shooter.enemyShot(this, target, (int)(dmg * (1.0f - (distanceFromExplosion / explosionReach))), 0);
							}
						}
						break;
					}
				}
			}
			base.OnTriggerEnter(directHitCollider);
		}
	}
}
