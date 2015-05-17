using NeonShooter.AppWarp.Json;
using NeonShooter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter.AppWarp.States
{
    public class ListState<TList, TState> : IState
        where TList : class
        where TState : class, IState
    {
        public bool Changed
        {
            get
            {
                return
                    NewItems.Count > 0 ||
                    DestroyedItems.Count > 0 ||
                    ObservedItems.Any(i => i.Value.Changed);
            }
        }

        private Func<TList, TState> stateFactory;

        public Dictionary<TList, TState> NewItems { get; private set; }
        public Dictionary<TList, TState> ObservedItems { get; private set; }
        public Dictionary<TList, TState> DestroyedItems { get; private set; }

        public ListState(NotifyingList<TList> list, Func<TList, TState> stateFactory)
        {
            this.stateFactory = stateFactory;

            NewItems = new Dictionary<TList, TState>();
            ObservedItems = new Dictionary<TList, TState>();
            DestroyedItems = new Dictionary<TList, TState>();

            list.OnListChanged += list_OnListChanged;
        }

        public void Clear()
        {
            foreach (var kv in NewItems)
                ObservedItems[kv.Key] = kv.Value;
            foreach (var kv in ObservedItems)
                kv.Value.Clear();
            NewItems.Clear();
            DestroyedItems.Clear();
        }

        public JsonObject ToJson()
        {
            var json = new JsonObject();
            if (NewItems.Count > 0)
                json.Append(new JsonPair("New", new JsonArray(
                    from i in NewItems select (IJsonObject)i.Value.ToJson())));
            if (ObservedItems.Count > 0)
                json.Append(new JsonPair("Observed", new JsonArray(
                    from i in ObservedItems where i.Value.Changed select (IJsonObject)i.Value.ToJson())));
            if (DestroyedItems.Count > 0)
                json.Append(new JsonPair("Destroyed", new JsonArray(
                    from i in ObservedItems select (IJsonObject)i.Value.ToJson())));
            return json;
        }

        void list_OnListChanged(NotifyingList<TList>.NotifyingListEventArgs e)
        {
            TState state;
            switch (e.Change)
            {
                case NotifyingList<TList>.ListChange.ItemAdd:
                    NewItems[e.NewItem] = stateFactory(e.NewItem);
                    break;
                case NotifyingList<TList>.ListChange.ItemRemove:
                    state = NewItems.TryRemove(e.NewItem);
                    if (state == null) state = ObservedItems.TryRemove(e.NewItem);
                    DestroyedItems[e.NewItem] = state;
                    break;
                case NotifyingList<TList>.ListChange.ItemSet:
                    state = NewItems.TryRemove(e.OldItem);
                    if (state == null) state = ObservedItems.TryRemove(e.OldItem);
                    DestroyedItems[e.OldItem] = state;
                    NewItems[e.NewItem] = stateFactory(e.NewItem);
                    break;
                case NotifyingList<TList>.ListChange.Clear:
                    foreach (var kv in NewItems)
                        DestroyedItems[kv.Key] = kv.Value;
                    NewItems.Clear();
                    foreach (var kv in ObservedItems)
                        DestroyedItems[kv.Key] = kv.Value;
                    ObservedItems.Clear();
                    break;
            }
        }
    }
}
