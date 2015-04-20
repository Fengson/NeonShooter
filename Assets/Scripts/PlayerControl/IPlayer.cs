using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.PlayerControl
{
    public interface IPlayer
    {
        NotifyingProperty<Vector3> Position { get; }
        NotifyingProperty<Vector2> Rotations { get; }
        NotifyingProperty<Vector3> Direction { get; }
    }
}
