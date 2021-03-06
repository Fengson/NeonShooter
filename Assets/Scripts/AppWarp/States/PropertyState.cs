﻿using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;

namespace NeonShooter.AppWarp.States
{
    public class PropertyState<TProperty, TState> : BasePropertyState<TProperty, TState>
        where TState : BinaryConvert.IBinaryWritable
    {
        public PropertyState(
            JSONNode jsonNode, Func<JSONNode, TState> toStateConverter,
            Action<NotifyingProperty<TProperty>, TState> stateApplier)
            : base(jsonNode, toStateConverter, stateApplier)
        {
        }

        public PropertyState(bool valueInReader,
            BinaryReader br, Func<BinaryReader, TState> toStateReader,
            Action<NotifyingProperty<TProperty>, TState> stateApplier)
            : base(valueInReader, br, toStateReader, stateApplier)
        {
        }

        public PropertyState(NotifyingProperty<TProperty> property,
            Func<TProperty, TState> stateSelector,
            Func<TState, IJsonObject> toJsonConverter)
            : base(property, stateSelector, toJsonConverter)
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
