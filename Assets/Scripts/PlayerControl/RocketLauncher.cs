﻿using NeonShooter;
using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon {

		public float ExplosionReach { get; private set; }

		/**
		make sure to set Reach to whole number, so each flight part length is always 1
		*/
		public RocketLauncher() : base((int)300, 25, 0, 50)
		{
			this.ExplosionReach = 5f;
		}

		public override void shoot(Player shooter, int costPayed) {
    		shootSound(shooter);
		    shooter.StartCoroutine(rocketLauncherShoot(shooter, costPayed));
		}

		/**
		this method send [parts_number] scaled-length rays to simulate long lasting rocket flight
		when it hits something explode method invokes
		*/
		public IEnumerator rocketLauncherShoot(Player shooter, int costPayed) {
    		Vector3 startingPosition = shooter.Position[null]+new Vector3(0,0.8f,0);
			Vector3 finalEndingPosition =
				Vector3.MoveTowards(startingPosition, startingPosition+Reach*shooter.Direction[null], (int)Reach);
            GameObject projectile = createProjectile(shooter, startingPosition, Color.black);
			Vector3 endingPosition = startingPosition;
			//this makes length=1 for every part of rocket projectile, so rocket speed = projectileSpeed()
		    int partsNumber = (int)(Reach/2);
			float step = Reach/partsNumber;

			int rocketId = (int)(1000*Random.value);
			Debug.Log("Rocket "+rocketId+" launched.\nSpeed: "+step + "point/" + projectileSpeed() +"sec. Reach point: "+finalEndingPosition);
            for(int i=0; i<partsNumber; i++) {
            	float beforeCalc = Time.time;
				startingPosition = endingPosition;
			    endingPosition = Vector3.MoveTowards(endingPosition, finalEndingPosition, step);

                Debug.Log("Rocket "+rocketId+" position " + endingPosition);

                RaycastHit hitInfo;
			    if(shootLine(startingPosition, endingPosition, out hitInfo)) {
			        //explosion effect
			        shooter.StartCoroutine(destroyProjectile(shooter, projectile));
			        explodeMissile(shooter, hitInfo.point, hitInfo.collider, costPayed);
			        break;
			    }
            	float minusSeconds = Time.time-beforeCalc;
                yield return new WaitForSeconds(Mathf.Max(0.1f,step/projectileSpeed()-minusSeconds));
            }
        }

        void explodeMissile(Player shooter, Vector3 hitPoint, Collider directHitCollider, int costPayed) {
        	Collider[] hitColliders = Physics.OverlapSphere(hitPoint, ExplosionReach);
            int k = 0;
            while (k < hitColliders.Length) {
            	if(hitColliders[k].name=="Plane" || hitColliders[k].name=="FPSController" || hitColliders[k].name=="Projectile(Clone)") {
            		k++;
            		continue;
            	}
            	foreach (GameObject target in appwarp.enemies)
                {
					if(target.GetComponent<Collider>()==hitColliders[k].GetComponent<Collider>()) {
						if(hitColliders[k]==directHitCollider) {
							shooter.enemyShot(this, target, this.Damage, costPayed);
						} else {
							float distanceFromExplosion = Vector3.Distance(hitPoint, hitColliders[k].ClosestPointOnBounds(hitPoint));
							Debug.Log("Explosion distance from " + hitColliders[k].name + " is " + distanceFromExplosion);
							if(distanceFromExplosion<ExplosionReach) {
								//only on clear shot we return shot cost
								shooter.enemyShot(this, target, (int)(Damage*(1.0f-(distanceFromExplosion/ExplosionReach))), 0);
							}
						}
						break;
					}
               	}
				k++;
            }
        }

		public override float projectileSpeed() {
			return 40.0f;
		}

		public override float projectileForceModifier() {
			return 2.5f;
		}

		public override int lifeRequiredToOwn() {
			return 300;
		}

		public override Weapon nextWeapon() {
			return new VacuumWeapon();
		}

		public override void shootSound(Player player) {
			player.sounds[2].Play();
		}

		public override string getWeaponName() {
		    return "Rocket Launcher";
		}
}
