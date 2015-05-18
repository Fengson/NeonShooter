using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter.AppWarp.States
{
    public class ListState<TList, TState> : IState
        where TState : class, IState
    {
        public const string AddedKey = "Added";
        public const string ObservedKey = "Observed";
        public const string RemovedKey = "Removed";

        public static ListState<TList, TState> FromJSONNode(JSONNode jsonNode,
            Func<JSONNode, TState> toStateConverter, Func<TState, TList> objectCreator,
            Func<TState, TList> objectSelector)
        {
            var listState = new ListState<TList, TState>();

            if (jsonNode != null)
            {
                var jsonAdded = jsonNode[AddedKey];
                if (jsonAdded != null)
                    foreach (var item in jsonAdded.AsList<TState>(toStateConverter))
                        listState.AddedItems[objectCreator(item)] = item;

                var jsonObserved = jsonNode[ObservedKey];
                if (jsonObserved != null)
                    foreach (var item in jsonObserved.AsList<TState>(toStateConverter))
                        listState.ObservedItems[objectSelector(item)] = item;

                var jsonRemoved = jsonNode[RemovedKey];
                if (jsonRemoved != null)
                    foreach (var item in jsonRemoved.AsList<TState>(toStateConverter))
                        listState.RemovedItems[objectSelector(item)] = item;
            }

            return listState;
        }

        public bool Changed
        {
            get
            {
                return
                    AddedItems.Count > 0 ||
                    RemovedItems.Count > 0 ||
                    ObservedItems.Any(i => i.Value.Changed);
            }
        }

        public IJsonObject RelativeJson
        {
            get
            {
                var json = new JsonObject();
                if (AddedItems.Count > 0)
                    json.Append(new JsonPair(AddedKey, new JsonArray(
                        from i in AddedItems select i.Value.RelativeJson)));
                if (ObservedItems.Count > 0)
                    json.Append(new JsonPair(ObservedKey, new JsonArray(
                        from i in ObservedItems where i.Value.Changed select i.Value.RelativeJson)));
                if (RemovedItems.Count > 0)
                    json.Append(new JsonPair(RemovedKey, new JsonArray(
                        from i in RemovedItems select i.Value.RelativeJson)));
                return json;
            }
        }

        public IJsonObject AbsoluteJson
        {
            get
            {
                var json = new JsonObject();
                json.Append(new JsonPair(AddedKey, new JsonArray(
                    from i in AllItems select (IJsonObject)i.Value.RelativeJson)));
                return json;
            }
        }

        private Func<TList, TState> stateFactory;

        public Dictionary<TList, TState> AllItems { get; private set; }
        public Dictionary<TList, TState> AddedItems { get; private set; }
        public Dictionary<TList, TState> ObservedItems { get; private set; }
        public Dictionary<TList, TState> RemovedItems { get; private set; }

        private ListState()
        {
            AllItems = new Dictionary<TList, TState>();
            AddedItems = new Dictionary<TList, TState>();
            ObservedItems = new Dictionary<TList, TState>();
            RemovedItems = new Dictionary<TList, TState>();
        }

        public ListState(INotifyingList<TList> list, Func<TList, TState> stateFactory)
            : this()
        {
            this.stateFactory = stateFactory;

            foreach (var item in list) AddItem(item);

            list.ListChanged += list_ListChanged;
        }

        public void ClearChanges()
        {
            foreach (var kv in AddedItems)
                ObservedItems[kv.Key] = kv.Value;
            foreach (var kv in ObservedItems)
                kv.Value.ClearChanges();
            AddedItems.Clear();
            RemovedItems.Clear();
        }

        void list_ListChanged(NotifyingListEventArgs<TList> e)
        {
            TState state;
            switch (e.Change)
            {
                case NotifyingListEventArgs.ListChange.Add:
                    AddItem(e.Item);
                    break;
                case NotifyingListEventArgs.ListChange.AddMany:
                    foreach (var item in e.Items) AddItem(item);
                    break;
                case NotifyingListEventArgs.ListChange.Remove:
                    state = AddedItems.TryRemove(e.Item);
                    if (state == null) state = ObservedItems.TryRemove(e.Item);
                    RemovedItems[e.Item] = state;
                    break;
                case NotifyingListEventArgs.ListChange.RemoveMany:
                    foreach (var item in e.Items)
                    {
                        state = AddedItems.TryRemove(item);
                        if (state == null) state = ObservedItems.TryRemove(item);
                        RemovedItems[item] = state;
                    }
                    break;
                case NotifyingListEventArgs.ListChange.Set:
                    state = AddedItems.TryRemove(e.OldItem);
                    if (state == null) state = ObservedItems.TryRemove(e.OldItem);
                    RemovedItems[e.OldItem] = state;
                    AddItem(e.Item);
                    break;
                case NotifyingListEventArgs.ListChange.Clear:
                    foreach (var kv in AddedItems)
                        RemovedItems[kv.Key] = kv.Value;
                    AddedItems.Clear();
                    foreach (var kv in ObservedItems)
                        RemovedItems[kv.Key] = kv.Value;
                    ObservedItems.Clear();
                    break;
            }
        }

        public void ApplyTo(object o)
        {
            var list = o as NotifyingList<TList>;
            if (list != null) ApplyTo(list);
        }

        public void ApplyTo(NotifyingList<TList> notifyingList)
        {
            foreach (var kv in RemovedItems)
            {
                kv.Value.ApplyTo(kv.Key);
                notifyingList.Remove(kv.Key);
            }
            foreach (var kv in ObservedItems)
                if (kv.Value.Changed)
                    kv.Value.ApplyTo(kv.Key);
            foreach (var kv in AddedItems)
            {
                notifyingList.Add(kv.Key);
                kv.Value.ApplyTo(kv.Key);
            }
        }

        private void AddItem(TList item)
        {
            var state = stateFactory(item);
            AllItems[item] = AddedItems[item] = state;
        }

        private void RemoveItem(TList item, TState state)
        {
            RemovedItems[item] = state;
            AllItems.TryRemove(item);
        }
    }
}
