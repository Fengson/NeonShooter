using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class PropertyQuaternionState<TProperty> : PropertyCustomBinaryState<TProperty, Quaternion>
    {
        public PropertyQuaternionState(
            JSONNode jsonNode, Func<JSONNode, Quaternion> toStateConverter,
            Action<NotifyingProperty<TProperty>, Quaternion> stateApplier)
            : base(jsonNode, toStateConverter, stateApplier)
        {
        }

        public PropertyQuaternionState(bool valueInReader,
            BinaryReader br, Func<BinaryReader, Quaternion> toStateReader,
            Action<NotifyingProperty<TProperty>, Quaternion> stateApplier)
            : base(valueInReader, br, toStateReader, stateApplier)
        {
        }

        public PropertyQuaternionState(NotifyingProperty<TProperty> property,
            Func<TProperty, Quaternion> stateSelector,
            Func<Quaternion, IJsonObject> toJsonConverter)
            : base(property, stateSelector, toJsonConverter, BinaryConvert.Write)
        {
        }
    }
}
