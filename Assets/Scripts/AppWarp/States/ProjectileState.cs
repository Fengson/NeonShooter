using NeonShooter.AppWarp.Json;
using NeonShooter.Players.Weapons;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class ProjectileState : IState
    {
        public bool Changed
        {
            get
            {
                return
                    Position != null &&
                    Rotation != null;
            }
        }

        public Vector3? Position { get; private set; }
        public Quaternion? Rotation { get; private set; }

        public ProjectileState(Projectile projectile)
        {
            projectile.Position.ValueChanged += Position_ValueChanged;
            projectile.Rotation.ValueChanged += Rotation_ValueChanged;
        }

        public void Clear()
        {
            Position = null;
            Rotation = null;
        }

        public JsonObject ToJson()
        {
            var json = new JsonObject();
            if (Position != null) json.Append(new JsonPair("Position", Position.Value.ToJson()));
            if (Position != null) json.Append(new JsonPair("Rotation", Rotation.Value.ToJson()));
            return json;
        }

        void Position_ValueChanged(Vector3 oldValue, Vector3 newValue)
        {
            Position = newValue;
        }

        void Rotation_ValueChanged(Quaternion oldValue, Quaternion newValue)
        {
            Rotation = newValue;
        }
    }
}
