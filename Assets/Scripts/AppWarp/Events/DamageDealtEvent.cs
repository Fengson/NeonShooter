using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using System;
using UnityEngine;

namespace NeonShooter.AppWarp.Events
{
    public class DamageDealtEvent : BaseEvent<Player, EnemyPlayer, Damage>
    {
        public const string AmountKey = "Amount";
        public const string EffectKey = "Effect";

        public override string Key { get { return "DamageDealt"; } }

        protected override JsonObject ToJson(Damage damage)
        {
            return new JsonObject(
                new JsonPair(AmountKey, damage.Amount),
                new JsonPair(EffectKey, damage.Effect.ToJson()));
        }
        
        protected override Damage ToArg(EnemyPlayer sender, JSONNode json)
        {
            if (json == null || json[AmountKey] == null || json[EffectKey] == null)
                throw new ArgumentException(string.Format("Invalid JSONNode (null or missing valid keys: {0}, {1}.", AmountKey, EffectKey));
            return new Damage(sender, Parent, json[AmountKey].AsInt, json[EffectKey].AsEnum<DamageEffect>());
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
