using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class PropertyVector3State<TProperty> : PropertyCustomBinaryState<TProperty, Vector3>
    {
        public PropertyVector3State(bool valueInReader,
            BinaryReader br, Func<BinaryReader, Vector3> toStateReader,
            Action<NotifyingProperty<TProperty>, Vector3> stateApplier)
            : base(valueInReader, br, toStateReader, stateApplier)
        {
        }

        public PropertyVector3State(
            JSONNode jsonNode, Func<JSONNode, Vector3> toStateConverter,
            Action<NotifyingProperty<TProperty>, Vector3> stateApplier)
            : base(jsonNode, toStateConverter, stateApplier)
        {
        }

        public PropertyVector3State(NotifyingProperty<TProperty> property,
            Func<TProperty, Vector3> stateSelector,
            Func<Vector3, IJsonObject> toJsonConverter)
            : base(property, stateSelector, toJsonConverter, BinaryConvert.Write)
        {
        }
    }
}
