using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class ReadOnlyVector2State<TProperty> : ReadOnlyCustomBinaryState<TProperty, Vector2>
    {
        public ReadOnlyVector2State(
            JSONNode jsonNode, Func<JSONNode, Vector2> toStateConverter,
            Action<TProperty, Vector2> stateApplier)
            : base(jsonNode, toStateConverter, stateApplier)
        {
        }

        public ReadOnlyVector2State(bool valueInReader,
            BinaryReader br, Func<BinaryReader, Vector2> toStateReader,
            Action<TProperty, Vector2> stateApplier)
            : base(valueInReader, br, toStateReader, stateApplier)
        {
        }


        public ReadOnlyVector2State(Vector2 state)
            : base(state, BinaryConvert.Write)
        {
        }
    }

}
