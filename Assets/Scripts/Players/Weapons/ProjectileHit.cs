namespace NeonShooter.Players.Weapons
{
	public class ProjectileHit
	{
		public BasePlayer Owner { get; private set; }
		public long Id { get; private set; }
		//public HitEffect Effect{ get; private set; }
		
		public ProjectileHit(BasePlayer owner, long id)
		{
			Owner = owner;
			Id = id;
		}
	}
}
