using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Cube;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;

namespace NeonShooter.AppWarp.Events
{
    public class DamageDealtEvent : BaseEvent<Player, EnemyPlayer, Damage>
    {
        public const string AmountKey = "Amount";
        public const string EffectKey = "Effect";

        public override string Key { get { return "DamageDealt"; } }
        public override string[] SubKeys { get { return new[] { AmountKey, EffectKey }; } }

        protected override JsonObject ToJson(Damage damage)
        {
            return new JsonObject(
                new JsonPair(AmountKey, damage.Amount),
                new JsonPair(EffectKey, damage.Effect.ToJson()));
        }
        
        protected override Damage ToArg(EnemyPlayer sender, JSONNode json)
        {
            return new Damage(sender, Parent, json[AmountKey].AsInt, json[EffectKey].AsEnum<CubelingSpawnEffect>());
        }

        protected override InvokableAction<Damage> GetAction(Player parent)
        {
            return parent.DamageDealt;
        }

        protected override InvokableAction<Damage> GetAction(EnemyPlayer sender)
        {
            return sender.DamageDealt;
        }

        public DamageDealtEvent(appwarp appwarp, Player player)
            : base(appwarp, player)
        {
        }
    }
}
