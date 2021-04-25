using Godot;
using Ludum48.Core.Managers;

namespace Ludum48.Core.Weapons
{
    public class Bullet : TimeObject
    {
        [Export]
        public float LifeTime = 300f;

        private AnimatedSprite _animatedSprite;

        [Export]
        private int _bouncesLeft = 5;

        private Timer _timer = new Timer();

        public float Damage { get; set; }

        public Vector2 Direction { get; set; }

        public Entity Owner { get; set; }
        public float Speed { get; set; }

        public override void _Ready()
        {
            base._Ready();

            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _animatedSprite.Connect("animation_finished", this, nameof(OnAnimationFinished));

            AddChild(_timer);
            _timer.OneShot = true;
            _timer.Connect("timeout", this, nameof(OnTimer));
            _timer.Start(LifeTime);
        }

        public void FillRemainingFrames()
        {
            var tuple = SavedFrames.Last.Value;
            while (SavedFrames.Count < GameManager.MaxFramesPlayed * FrameFactor)
            {
                SavedFrames.AddLast(tuple);
            }
        }

        public override void PhysicsProcess(float delta)
        {
            var collision = MoveAndCollide(Direction * Speed * delta);

            if (collision != null)
            {
                if (_bouncesLeft > 0)
                {
                    Direction = Direction.Bounce(collision.Normal);
                    _bouncesLeft--;
                }
                else
                {
                    Pop();
                }
            }
        }

        public float Pop()
        {
            IsActive = false;
            _animatedSprite.Play();
            return Damage;
        }

        public override void Process(float delta)
        {
        }

        private void OnAnimationFinished()
        {
            GameManager.Instance.Unregister(this);
            QueueFree();
        }

        private void OnTimer()
        {
            Pop();
        }
    }
}