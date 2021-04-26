using Godot;

namespace Ludum48.Core.Time
{
    public class BaseTimeFrame
    {
        public virtual BaseTimeFrame Clone()
        {
            return new BaseTimeFrame { Position = Position, IsActive = IsActive, Rotation = Rotation };
        }

        public bool IsActive { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
    }
}