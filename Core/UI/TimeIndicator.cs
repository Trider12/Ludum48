using Godot;

namespace Ludum48.Core.UI
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
            ChangeColor(Colors.White);
        }

        public void ChangeColor(Color color)
        {
            Modulate = color;
        }
    }
}