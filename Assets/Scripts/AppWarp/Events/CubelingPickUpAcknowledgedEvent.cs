using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Cube;
using NeonShooter.Utils;

namespace NeonShooter.AppWarp.Events
{
    public class CubelingPickUpAcknowledgedEvent : BaseEvent<Player, EnemyPlayer, PickUpAcknowledge>
    {
        public const string IdKey = "Id";
        public const string AcceptedKey = "Accepted";

        public override string Key { get { return "CubelingPickUpAcknowledged"; } }
        public override string[] SubKeys { get { return new[] { IdKey, AcceptedKey }; } }

        protected override JsonObject ToJson(PickUpAcknowledge pickUp)
        {
            return new JsonObject(
                new JsonPair(IdKey, pickUp.Id),
                new JsonPair(AcceptedKey, pickUp.Accepted));
        }
        
        protected override PickUpAcknowledge ToArg(EnemyPlayer sender, JSONNode json)
        {
            return new PickUpAcknowledge(sender, Parent, json[IdKey].AsLong(), json[AcceptedKey].AsBool);
        }

        protected override InvokableAction<PickUpAcknowledge> GetAction(Player parent)
        {
            return parent.CubelingPickUpAcknowledged;
        }

        protected override InvokableAction<PickUpAcknowledge> GetAction(EnemyPlayer sender)
        {
            return sender.CubelingPickUpAcknowledged;
        }

        public CubelingPickUpAcknowledgedEvent(appwarp appwarp, Player player)
            : base(appwarp, player)
        {
        }
    }
}
