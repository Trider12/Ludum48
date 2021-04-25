using Godot;

namespace GlobalGameJam2021.Core.UI
{
    public class TimeIndicator : Sprite
    {
        private Vector2 _offset;
        private Node2D _parent;

        public override void _Process(float delta)
        {
            GlobalPosition = _parent.GlobalPosition + _offset;
            GlobalRotation = 0;
        }

        public override void _Ready()
        {
            _offset = Position;
            _parent = GetParent<Node2D>();
        }

        public void ChangeColor(Color color)
        {
            Modulate = color;
        }
    }
}