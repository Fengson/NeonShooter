using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Cube;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;

namespace NeonShooter.AppWarp.Events
{
    public class ProjectileHitEvent : BaseEvent<Projectile, EnemyProjectile, ProjectileHit>
    {
        public const string IdKey = "Id";
        public const string AcceptedKey = "Accepted";

        public override string Key { get { return "ProjectileHit"; } }
        public override string[] SubKeys { get { return new[] { IdKey, AcceptedKey }; } }

        protected override JsonObject ToJson(ProjectileHit hit)
        {
            return new JsonObject(
                new JsonPair(IdKey, hit.Id));
        }
        
        protected override ProjectileHit ToArg(EnemyProjectile sender, JSONNode json)
        {
            return new ProjectileHit(sender.ParentWeapon.Player, json[IdKey].AsLong());
        }

        protected override InvokableAction<ProjectileHit> GetAction(Projectile parent)
        {
            return parent.ProjectileHit;
        }

        protected override InvokableAction<ProjectileHit> GetAction(EnemyProjectile sender)
        {
            return sender.ProjectileHit;
        }

        public ProjectileHitEvent(appwarp appwarp, Projectile projectile)
            : base(appwarp, projectile)
        {
        }
    }
}
