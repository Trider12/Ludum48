using Godot;

namespace GlobalGameJam2021.Core
{
    public class Player : KinematicBody2D
    {
        private const float Tolerance = 5;
        private Vector2 _destination;

        private float _speed = 200;

        public override void _PhysicsProcess(float delta)
        {
            if (Mathf.Abs(GlobalPosition.x - _destination.x) <= Tolerance || Mathf.Abs(GlobalPosition.y - _destination.y) <= Tolerance)
            {
                GlobalPosition = _destination;
                SetPhysicsProcess(false);
                return;
            }

            var dir = _destination - GlobalPosition;
            var velocity = _speed * dir.Normalized();

            MoveAndSlide(velocity);
        }

        public override void _Ready()
        {
            SetPhysicsProcess(false);
        }

        public bool MoveTo(Vector2 globalPos)
        {
            if (IsPhysicsProcessing())
            {
                return false;
            }

            _destination = globalPos;
            SetPhysicsProcess(true);
            return true;
        }
    }
}