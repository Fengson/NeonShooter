using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Weapons
{
    public abstract class BaseProjectile : Atom
    {
        public Weapon ParentWeapon { get; set; }
    }
}
