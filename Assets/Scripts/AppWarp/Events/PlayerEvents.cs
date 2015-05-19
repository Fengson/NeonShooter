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
        public CubelingPickedUpEvent CubelingPickedUp { get; private set; }
        public CubelingPickUpAcknowledgedEvent CubelingPickUpAcknowledged { get; private set; }

        public PlayerEvents(appwarp appwarp, Player player)
        {
            DamageDealt = new DamageDealtEvent(appwarp, player);
            CubelingPickedUp = new CubelingPickedUpEvent(appwarp, player);
            CubelingPickUpAcknowledged = new CubelingPickUpAcknowledgedEvent(appwarp, player);

            events = new Dictionary<string, IReceivableEvent<EnemyPlayer>>();
            var eventArray = new IReceivableEvent<EnemyPlayer>[] { DamageDealt, CubelingPickedUp, CubelingPickUpAcknowledged };
            foreach (var re in eventArray)
                events.Add(re.Key, re);
        }
    }
}
