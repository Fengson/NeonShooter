using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Utils;
using System.Collections.Generic;

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
        public abstract string[] SubKeys { get; }

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
            bool throwException = eventArgJson == null;
            if (!throwException)
            {
                foreach (var key in SubKeys)
                {
                    throwException = eventArgJson[key] == null;
                    if (throwException) break;
                }
            }
            if (throwException)
                throw new InvalidJSONNodeException(SubKeys);

            GetAction(sender).Invoke(ToArg(sender, eventArgJson));
        }

        public class InvalidJSONNodeException : System.Exception
        {
            public List<string> RequiredKeys { get; private set; }

            public InvalidJSONNodeException(params string[] requiredKeys)
                : base(string.Format("Invalid JSONNode (json is null or given pair{0} key{1} {2} missing: {3}).",
                    requiredKeys.Length == 0 ? "'s" : "s'",
                    requiredKeys.Length == 0 ? "" : "s",
                    requiredKeys.Length == 0 ? "is" : "are",
                    requiredKeys))
            {
                RequiredKeys = new List<string>(requiredKeys);
            }
        }
    }
}
