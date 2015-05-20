using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class ReadOnlyQuaternionState<TProperty> : ReadOnlyCustomBinaryState<TProperty, Quaternion>
    {
        public ReadOnlyQuaternionState(
            JSONNode jsonNode, Func<JSONNode, Quaternion> toStateConverter,
            Action<TProperty, Quaternion> stateApplier)
            : base(jsonNode, toStateConverter, stateApplier)
        {
        }

        public ReadOnlyQuaternionState(bool valueInReader,
            BinaryReader br, Func<BinaryReader, Quaternion> toStateReader,
            Action<TProperty, Quaternion> stateApplier)
            : base(valueInReader, br, toStateReader, stateApplier)
        {
        }


        public ReadOnlyQuaternionState(Quaternion state)
            : base(state, BinaryConvert.Write)
        {
        }
    }

}
