namespace Ludum48.Core.Time
{
    public class SpriteTimeFrame : HealthTimeFrame
    {
        public int SpriteFrame { get; set; }

        public override BaseTimeFrame Clone()
        {
            return new SpriteTimeFrame { Position = Position, Rotation = Rotation, IsActive = IsActive, Health = Health, SpriteFrame = SpriteFrame };
        }
    }
}