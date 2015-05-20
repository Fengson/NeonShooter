using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;

namespace NeonShooter.AppWarp.States
{
    public class ReadOnlyState<TProperty, TState> : BaseReadOnlyState<TProperty, TState>
        where TState : BinaryConvert.IBinaryWritable
    {
        public ReadOnlyState(
            JSONNode jsonNode, Func<JSONNode, TState> toStateConverter,
            Action<TProperty, TState> stateApplier)
            : base(jsonNode, toStateConverter, stateApplier)
        {
        }

        public ReadOnlyState(bool valueInReader,
            BinaryReader br, Func<BinaryReader, TState> toStateReader,
            Action<TProperty, TState> stateApplier)
            : base(valueInReader, br, toStateReader, stateApplier)
        {
        }

        public ReadOnlyState(TState state)
            : base(state)
        {
        }

        public override void WriteRelativeBinaryTo(BinaryWriter bw)
        {
            if (Changed) WriteAbsoluteBinaryTo(bw);
        }

        public override void WriteAbsoluteBinaryTo(BinaryWriter bw)
        {
            bw.Write(Value);
        }
    }

}
