using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.Utils;
using System;
using System.IO;
using System.Linq;

namespace NeonShooter.AppWarp.States
{
    public class ListState<TList, TState> : BaseListState<TList, TState>
        where TState : class, IState
    {
        //public override int AbsoluteBinarySize
        //{
        //    get
        //    {
        //        return
        //            4 + // int (items count)
        //            // TODO: it works only if this is list of the same-sized objects!
        //            AllItems.Count > 0 ? AllItems.Count * AllItems.First().Value.AbsoluteBinarySize : 0 +
        //            4 + // int (changed items count = 0)
        //            4; // int (removed items count = 0)
        //    }
        //}

        public ListState(JSONNode jsonNode, Func<JSONNode, TState> toStateConverter,
            Func<TState, TList> objectCreator, Func<TState, TList> objectSelector)
            : base(jsonNode, toStateConverter, objectCreator, objectSelector)
        //public ListState(JSONNode jsonNode, Func<BinaryReader, TState> toStateReader,
        //    Func<TState, TList> objectCreator, Func<TState, TList> objectSelector)
        //    : base(jsonNode, toStateReader, objectCreator, objectSelector)
        {
        }

        //public ListState(bool valueInReader, BinaryReader br, Func<BinaryReader, TState> toStateConverter,
        //    Func<TState, TList> objectCreator, Func<TState, TList> objectSelector)
        //    : base(valueInReader, br, toStateConverter, objectCreator, objectSelector)
        //{
        //}

        public ListState(INotifyingList<TList> list, Func<TList, TState> stateFactory)
            : base(list, stateFactory)
        {
        }

        //public override void WriteRelativeBinaryTo(BinaryWriter bw)
        //{
        //    bw.Write(AddedItems.Count);
        //    bw.WriteRelative(AddedItems.Values);
        //    var changedObservedItems = from i in ObservedItems where i.Value.Changed select i.Value;
        //    bw.Write(changedObservedItems.Count());
        //    bw.WriteRelative(changedObservedItems);
        //    bw.Write(RemovedItems.Count);
        //    bw.WriteRelative(RemovedItems.Values);
        //}

        //public override void WriteAbsoluteBinaryTo(BinaryWriter bw)
        //{
        //    bw.Write(AllItems.Count);
        //    bw.WriteAbsolute(AllItems.Values);
        //    bw.Write(0);
        //    bw.Write(0);
        //}
    }

}
