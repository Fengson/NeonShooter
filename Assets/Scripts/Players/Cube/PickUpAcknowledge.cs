namespace NeonShooter.Players.Cube
{
    public class PickUpAcknowledge
    {
        public BasePlayer Spawner { get; private set; }
        public BasePlayer Claimer { get; private set; }
        public long Id { get; private set; }
        public bool Accepted { get; private set; }

        public PickUpAcknowledge(BasePlayer spawner, BasePlayer claimer, long id, bool accepted)
        {
            Spawner = spawner;
            Claimer = claimer;
            Id = id;
            Accepted = accepted;
        }

        public PickUpAcknowledge(PickUp pickUp, bool accepted)
            : this(pickUp.Spawner, pickUp.Claimer, pickUp.Id, accepted)
        {
        }
    }
}
