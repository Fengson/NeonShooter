using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;

namespace NeonShooter.AppWarp.States
{
    public class PropertyCustomBinaryState<TProperty, TState> : BasePropertyState<TProperty, TState>
    {
        Action<BinaryWriter, TState> customBinaryWriter;

        public PropertyCustomBinaryState(
            JSONNode jsonNode, Func<JSONNode, TState> toStateConverter,
            Action<NotifyingProperty<TProperty>, TState> stateApplier)
            : base(jsonNode, toStateConverter, stateApplier)
        {
        }

        public PropertyCustomBinaryState(bool valueInReader,
            BinaryReader br, Func<BinaryReader, TState> toStateReader,
            Action<NotifyingProperty<TProperty>, TState> stateApplier)
            : base(valueInReader, br, toStateReader, stateApplier)
        {
        }

        public PropertyCustomBinaryState(NotifyingProperty<TProperty> property,
            Func<TProperty, TState> stateSelector,
            Func<TState, IJsonObject> toJsonConverter,
            Action<BinaryWriter, TState> customBinaryWriter)
            : base(property, stateSelector, toJsonConverter)
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
