using Godot;

using Ludum48.Core.Managers;
using Ludum48.Core.Time;

namespace Ludum48.Core.Weapons
{
    public class Bullet : TimeObject
    {
        [Export]
        public float LifeTime = 300f;

        [Export]
        private int _bouncesLeft = 5;

        private CollisionShape2D _collisionShape;
        private Timer _timer = new Timer();

        public float Damage { get; set; }

        public Vector2 Direction { get; set; }

        public override bool IsActive
        {
            get => base.IsActive;

            protected set
            {
                base.IsActive = value;

                _collisionShape.SetDeferred("disabled", !value);
            }
        }

        public Entity OwnerEntity { get; set; }
        public float Speed { get; set; }

        public override void _Ready()
        {
            base._Ready();

            _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

            AddChild(_timer);
            _timer.OneShot = true;
            _timer.Connect("timeout", this, nameof(OnTimer));
            _timer.Start(LifeTime);

            if (OwnerEntity != null)
            {
                OwnerEntity.RegisterBullet(this);
            }
        }

        public float Hit()
        {
            IsActive = false;
            return Damage;
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
                    Hit();
                }
            }
        }

        public override void Process(float delta)
        {
        }

        private void OnTimer()
        {
            GameManager.Instance.Unregister(this);
            QueueFree();
        }
    }
}