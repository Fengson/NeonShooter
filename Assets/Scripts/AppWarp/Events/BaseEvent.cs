using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Utils;

namespace NeonShooter.AppWarp.Events
{
    public interface IReceivableEvent<TRemote>
    {
        string Key { get; }
        void OnActionReceived(TRemote sender, JSONNode eventArgJson);
    }

    public abstract class BaseEvent<TParent, TRemote, TArg> : IReceivableEvent<TRemote>
    {
        public abstract string Key { get; }

        protected abstract JsonObject ToJson(TArg arg);
        protected abstract TArg ToArg(TRemote sender, JSONNode json);

        protected abstract InvokableAction<TArg> GetAction(TParent parent);
        protected abstract InvokableAction<TArg> GetAction(TRemote sender);

        public TParent Parent { get; private set; }

        protected BaseEvent(appwarp appwarp, TParent parent)
        {
            Parent = parent;
            GetAction(parent).Action += arg => appwarp.SendPlayerEvent(
                new JsonObject(new JsonPair(Key, ToJson(arg))));
        }

        public void OnActionReceived(TRemote sender, JSONNode eventArgJson)
        {
            GetAction(sender).Invoke(ToArg(sender, eventArgJson));
        }
    }
}
