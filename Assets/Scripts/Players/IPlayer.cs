using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using System.Collections.Generic;
using UnityEngine;
using NeonShooter.Players.Cube;

namespace NeonShooter.Players
{
    public interface IPlayer
    {
        INotifyingList<IVector3> CellsInStructure { get; }

        NotifyingProperty<Vector3> Position { get; }
        NotifyingProperty<Vector2> Rotations { get; }

        NotifyingProperty<Weapon> SelectedWeapon { get; }
        NotifyingProperty<bool> ContinousFire { get; }

        INotifyingList<BaseProjectile> LaunchedProjectiles { get; }

        InvokableAction<Damage> DamageDealt { get; }
        InvokableAction<PickUp> CubelingPickedUp { get; }
        InvokableAction<PickUpAcknowledge> CubelingPickUpAcknowledged { get; }
    }
}
