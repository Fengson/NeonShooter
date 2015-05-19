namespace NeonShooter.Players.Cube
{
    public class PickUp
    {
        public BasePlayer Spawner { get; private set; }
        public BasePlayer Claimer { get; private set; }
        public long Id { get; private set; }

        public PickUp(BasePlayer spawner, BasePlayer claimer, long id)
        {
            Spawner = spawner;
            Claimer = claimer;
            Id = id;
        }
    }
}
