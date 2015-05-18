using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Utils;
using System;

namespace NeonShooter.AppWarp.States
{
    public class CellState : IState
    {
        public static CellState FromJSONNode(JSONNode jsonNode)
        {
            if (jsonNode == null)
                throw new ArgumentException("Argument JSONNode jsonNode cannot be null.", "jsonNode");
            return new CellState(jsonNode.AsIVector3());
        }

        public bool Changed { get; private set; }

        public IJsonObject RelativeJson { get { return Position.ToJson(); } }
        public IJsonObject AbsoluteJson { get { return Position.ToJson(); } }

        public IVector3 Position { get; private set; }

        public CellState(IVector3 cellPosition)
        {
            Position = cellPosition;
            Changed = true;
        }

        public void ClearChanges()
        {
            Changed = false;
        }

        public void ApplyTo(object o)
        {
        }
    }
}
