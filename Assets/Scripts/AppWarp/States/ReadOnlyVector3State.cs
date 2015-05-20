using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class ReadOnlyVector3State<TProperty> : ReadOnlyCustomBinaryState<TProperty, Vector3>
    {
        public ReadOnlyVector3State(
            JSONNode jsonNode, Func<JSONNode, Vector3> toStateConverter,
            Action<TProperty, Vector3> stateApplier)
            : base(jsonNode, toStateConverter, stateApplier)
        {
        }

        public ReadOnlyVector3State(bool valueInReader,
            BinaryReader br, Func<BinaryReader, Vector3> toStateReader,
            Action<TProperty, Vector3> stateApplier)
            : base(valueInReader, br, toStateReader, stateApplier)
        {
        }


        public ReadOnlyVector3State(Vector3 state)
            : base(state, BinaryConvert.Write)
        {
        }
    }

}
