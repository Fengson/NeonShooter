using NeonShooter;
using NeonShooter.Utils;
using NeonShooter.PlayerControl;
using UnityEngine;
using System.Collections;

public class RocketLauncher : Weapon {

		public float ExplosionReach { get; private set; }
		public float FlySpeed { get; private set; }

		/**
		make sure to set Reach to whole number, so each flight part length is always 1
		*/
		public RocketLauncher() : base((int)300, 25, 0, 50)
		{
			this.ExplosionReach = 5f;
			this.FlySpeed = 10.0f;
		}

		public override void shoot(Player shooter, int costPayed) {
		    shooter.StartCoroutine(rocketLauncherShoot(shooter, costPayed));
		}

		/**
		this method send [parts_number] scaled-length rays to simulate long lasting rocket flight
		when it hits something explode method invokes
		*/
		public IEnumerator rocketLauncherShoot(Player shooter, int costPayed) {
    		Vector3 startingPosition = shooter.Position[null];
			Vector3 finalEndingPosition =
				Vector3.MoveTowards(startingPosition, startingPosition+Reach*shooter.Direction[null], (int)Reach);
			Vector3 endingPosition = startingPosition;
			//this makes length=1 for every part of rocket projectile, so rocket speed = FlySpeed
		    int partsNumber = (int)Reach;
			float step = Reach/partsNumber;

			int rocketId = (int)(1000*Random.value);
			Debug.Log("Rocket "+rocketId+" launched.\nSpeed: "+step + "point/" + FlySpeed +"sec. Reach point: "+finalEndingPosition);
            for(int i=0; i<partsNumber; i++) {
				startingPosition = endingPosition;
			    endingPosition = Vector3.MoveTowards(endingPosition, finalEndingPosition, step);

                Debug.Log("Rocket "+rocketId+" position " + endingPosition);

                RaycastHit hitInfo;
			    if(shootLine(startingPosition, endingPosition, out hitInfo)) {
			        //explosion effect
			        explodeMissile(shooter, hitInfo.point, hitInfo.collider, costPayed);
			        break;
			    }
                yield return new WaitForSeconds(1.0f/FlySpeed);
            }
        }

        void explodeMissile(Player shooter, Vector3 hitPoint, Collider directHitCollider, int costPayed) {
        	Collider[] hitColliders = Physics.OverlapSphere(hitPoint, ExplosionReach);
            int k = 0;
            while (k < hitColliders.Length) {
            	//TODO compare to other player collider list
            	if(hitColliders[k].name=="Plane" || hitColliders[k].name=="FPSController") {
            		k++;
            		continue;
            	}
            	if(hitColliders[k]==directHitCollider) {
            		shooter.enemyShot(this, hitColliders[k], this.Damage, costPayed);
            	} else {
            		float distanceFromExplosion = Vector3.Distance(hitPoint, hitColliders[k].ClosestPointOnBounds(hitPoint));
                	Debug.Log("Explosion distance from " + hitColliders[k].name + " is " + distanceFromExplosion);
            		if(distanceFromExplosion<ExplosionReach) {
            			//only on clear shot we return shot cost
            			shooter.enemyShot(this, hitColliders[k], (int)(Damage*(1.0f-(distanceFromExplosion/ExplosionReach))), 0);
            		}
            	}
                k++;
            }
        }

		public override float missileFlightDuration() {
			return Reach/FlySpeed;
		}

		public override int lifeRequiredToOwn() {
			return 300;
		}

		public override Weapon nextWeapon() {
			return new VacuumWeapon();
		}

		public override string getWeaponName() {
		    return "Rocket Launcher";
		}
}
