using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class PropertyVector2State<TProperty> : PropertyCustomBinaryState<TProperty, Vector2>
    {
        public PropertyVector2State(bool valueInReader,
            BinaryReader br, Func<BinaryReader, Vector2> toStateReader,
            Action<NotifyingProperty<TProperty>, Vector2> stateApplier)
            : base(valueInReader, br, toStateReader, stateApplier)
        {
        }

        public PropertyVector2State(
            JSONNode jsonNode, Func<JSONNode, Vector2> toStateConverter,
            Action<NotifyingProperty<TProperty>, Vector2> stateApplier)
            : base(jsonNode, toStateConverter, stateApplier)
        {
        }

        public PropertyVector2State(NotifyingProperty<TProperty> property,
            Func<TProperty, Vector2> stateSelector,
            Func<Vector2, IJsonObject> toJsonConverter)
            : base(property, stateSelector, toJsonConverter, BinaryConvert.Write)
        {
        }
    }
}
