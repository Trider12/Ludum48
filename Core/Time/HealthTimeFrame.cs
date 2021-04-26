namespace Ludum48.Core.Time
{
    public class HealthTimeFrame : BaseTimeFrame
    {
        public float Health { get; set; }

        public override BaseTimeFrame Clone()
        {
            return new HealthTimeFrame { Position = Position, Rotation = Rotation, IsActive = IsActive, Health = Health };
        }
    }
}