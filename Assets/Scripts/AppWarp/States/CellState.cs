using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Utils;
using System;
using System.IO;

namespace NeonShooter.AppWarp.States
{
    public class CellState : IState
    {
        public bool Changed { get; private set; }

        public IJsonObject RelativeJson { get { return Position.ToJson(); } }
        public IJsonObject AbsoluteJson { get { return Position.ToJson(); } }

        public int AbsoluteBinarySize { get { return 3 * 4; } }

        public IVector3 Position { get; private set; }

        public CellState(JSONNode jsonNode)
            : this(jsonNode.AsIVector3())
        {
        }

        public CellState(BinaryReader br)
            : this(br.ReadIVector3())
        {
        }

        public CellState(IVector3 cellPosition)
        {
            Position = cellPosition;
            Changed = true;
        }

        public void WriteRelativeBinaryTo(BinaryWriter bw)
        {
            bw.Write(Position);
        }

        public void WriteAbsoluteBinaryTo(BinaryWriter bw)
        {
            bw.Write(Position);
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
