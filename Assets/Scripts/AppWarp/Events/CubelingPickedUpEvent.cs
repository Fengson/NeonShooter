using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Cube;
using NeonShooter.Utils;

namespace NeonShooter.AppWarp.Events
{
    public class CubelingPickedUpEvent : BaseEvent<Player, EnemyPlayer, PickUp>
    {
        public const string IdKey = "Id";

        public override string Key { get { return "CubelingPickedUp"; } }
        public override string[] SubKeys { get { return new[] { IdKey }; } }

        protected override JsonObject ToJson(PickUp pickUp)
        {
            return new JsonObject(
                new JsonPair(IdKey, pickUp.Id));
        }
        
        protected override PickUp ToArg(EnemyPlayer sender, JSONNode json)
        {
            return new PickUp(sender, Parent, json[IdKey].AsLong());
        }

        protected override InvokableAction<PickUp> GetAction(Player parent)
        {
            return parent.CubelingPickedUp;
        }

        protected override InvokableAction<PickUp> GetAction(EnemyPlayer sender)
        {
            return sender.CubelingPickedUp;
        }

        public CubelingPickedUpEvent(appwarp appwarp, Player player)
            : base(appwarp, player)
        {
        }
    }
}
