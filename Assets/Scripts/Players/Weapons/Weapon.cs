using NeonShooter.Players.Cube;
using System.Collections;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public abstract class Weapon
    {
        float currentCoolDownTime;

        public abstract int Id { get; }
        public abstract string Name { get; }

        public abstract CubelingSpawnEffect DamageEffect { get; }
        public abstract FireType FireType { get; }

        public abstract float CoolDownTime { get; }
        public bool IsCoolingDown { get { return currentCoolDownTime > 0; } }

        public virtual Color ProjectileColor { get { return Color.white; } }

        public BasePlayer Player { get; private set;  }
        public int Damage { get; private set; }
        public float Reach { get; private set; }
        public int AmmoCost { get; private set; }

        public abstract int LifeRequiredToOwn { get; }

        public int GetCalculatedAmmoCost(int life)
        {
            return AmmoCost == 0 ? 0 : (int)(AmmoCost * Mathf.Max(1, 1.0f*life / (6*AmmoCost)));
        }

        public Weapon(BasePlayer player, int damage, float reach, int ammoCost)
        {
            this.Damage = damage;
            this.Reach = reach;
            this.AmmoCost = ammoCost;
            this.Player = player;
        }

        public virtual void Update()
        {
            currentCoolDownTime -= Time.deltaTime;
            if (currentCoolDownTime < 0) currentCoolDownTime = 0;
        }

        public void RaiseCooldown()
        {
            currentCoolDownTime = CoolDownTime;
        }

        public virtual void OnShootStart(BasePlayer shooter)
        {
        }

        public abstract void Shoot(Player shooter, int costPayed);

        public virtual void OnShootEnd()
        {
        }


        protected bool shootLine(Vector3 rayStart, Vector3 rayEnd, out RaycastHit hitInfo)
        {
            return Physics.Linecast(rayStart, rayEnd, out hitInfo);
        }
        
        public abstract void shootSound(Player player);
    }
}
