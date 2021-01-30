using Godot;

namespace GlobalGameJam2021.Core
{
    public class MovableEntity : KinematicBody2D
    {
        private const float Tolerance = 5;
        private Vector2 _destination;

        [Signal]
        public delegate void FinishedMovement();

        protected virtual float Speed { get; set; } = 200;

        public override void _PhysicsProcess(float delta)
        {
            if (Mathf.Abs(GlobalPosition.x - _destination.x) <= Tolerance || Mathf.Abs(GlobalPosition.y - _destination.y) <= Tolerance)
            {
                GlobalPosition = _destination;
                SetPhysicsProcess(false);

                EmitSignal(nameof(FinishedMovement));

                return;
            }

            var dir = _destination - GlobalPosition;
            var velocity = Speed * dir.Normalized();

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