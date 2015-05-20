using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;

namespace NeonShooter.AppWarp.States
{
    public class ReadOnlyCustomBinaryState<TProperty, TState> : BaseReadOnlyState<TProperty, TState>
    {
        Action<BinaryWriter, TState> customBinaryWriter;

        public ReadOnlyCustomBinaryState(
            JSONNode jsonNode, Func<JSONNode, TState> toStateConverter,
            Action<TProperty, TState> stateApplier)
            : base(jsonNode, toStateConverter, stateApplier)
        {
        }

        public ReadOnlyCustomBinaryState(bool valueInReader,
            BinaryReader br, Func<BinaryReader, TState> toStateReader,
            Action<TProperty, TState> stateApplier)
            : base(valueInReader, br, toStateReader, stateApplier)
        {
        }

        public ReadOnlyCustomBinaryState(TState state,
            Action<BinaryWriter, TState> customBinaryWriter)
            : base(state)
        {
            this.customBinaryWriter = customBinaryWriter;
        }

        public override void WriteRelativeBinaryTo(BinaryWriter bw)
        {
            if (Changed) WriteAbsoluteBinaryTo(bw);
        }

        public override void WriteAbsoluteBinaryTo(BinaryWriter bw)
        {
            customBinaryWriter(bw, Value);
        }
    }

}
