using NeonShooter.Players;
using NeonShooter.Utils;
using System.Collections.Generic;

namespace NeonShooter.AppWarp.Events
{
    public class PlayerEvents
    {
        Dictionary<string, IReceivableEvent<EnemyPlayer>> events;

        public IReceivableEvent<EnemyPlayer> this[string jsonKey]
        {
            get { return events.TryGet(jsonKey); }
        }

        public DamageDealtEvent DamageDealt { get; private set; }

        public PlayerEvents(appwarp appwarp, Player player)
        {
            DamageDealt = new DamageDealtEvent(appwarp, player);

            events = new Dictionary<string, IReceivableEvent<EnemyPlayer>>();
            foreach (var re in new[] { DamageDealt })
                events.Add(re.Key, re);
        }
    }
}
