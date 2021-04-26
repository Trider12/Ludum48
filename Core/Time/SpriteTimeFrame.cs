namespace Ludum48.Core.Time
{
    public class SpriteTimeFrame : BaseTimeFrame
    {
        public int SpriteFrame { get; set; }

        public override BaseTimeFrame Clone()
        {
            return new SpriteTimeFrame { Position = Position, Rotation = Rotation, IsActive = IsActive, SpriteFrame = SpriteFrame };
        }
    }
}