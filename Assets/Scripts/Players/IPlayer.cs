using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players
{
    public interface IPlayer
    {
        NotifyingProperty<Vector3> Position { get; }
        NotifyingProperty<Vector2> Rotations { get; }
        NotifyingProperty<Vector3> Direction { get; }

        NotifyingProperty<Weapon> SelectedWeapon { get; }

        NotifyingList<Projectile> LaunchedProjectiles { get; }
    }
}
