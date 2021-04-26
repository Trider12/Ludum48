using System.Collections.Generic;
using System.Linq;

using Godot;

using Ludum48.Core.Managers;

namespace Ludum48.Core.Enemies
{
    public abstract class Enemy : Entity
    {
        private Sprite _markSprite;
        private Navigation2D _navigation2D;
        private List<Vector2> _path = new List<Vector2>();
        private float _pathUpdateInterval = 0.5f;
        private Timer _pathUpdateTimer = new Timer();

        public Enemy() : base()
        {
            _pathUpdateTimer.Connect("timeout", this, nameof(OnPathUpdateTimer));
        }

        public override float MaxHealth { get; protected set; } = 3;

        protected virtual float SightRadius { get; } = 2000;

        protected virtual float StopAtRadius { get; } = 90;

        protected virtual float WalkRadius { get; } = 600;

        public override void _Ready()
        {
            base._Ready();

            _markSprite = GetNode<Sprite>("MarkSprite");

            AddChild(_pathUpdateTimer);
        }

        public override void EraseFromFuture()
        {
            base.EraseFromFuture();

            foreach (var bullet in _bullets)
            {
                bullet.EraseFromFuture();
            }

            ToggleMarkSprite(false);
        }

        public override void PhysicsProcess(float delta)
        {
            if (!IsActive)
            {
                return;
            }

            var player = GameManager.Instance.Player;

            float distanceSquared = player.GlobalPosition.DistanceSquaredTo(GlobalPosition);

            if (distanceSquared < SightRadius * SightRadius)
            {
                if (_pathUpdateTimer.IsStopped())
                {
                    _pathUpdateTimer.Start(_pathUpdateInterval);
                }

                if (distanceSquared < WalkRadius * WalkRadius)
                {
                    if (distanceSquared > StopAtRadius * StopAtRadius)
                    {
                        if (_path.Count < 2)
                        {
                            UpdatePath();
                        }

                        if (_path.Count > 0)
                        {
                            var velocity = (_path[1] - GlobalPosition).Normalized() * MaxSpeed;

                            MoveAndSlide(velocity);
                        }
                    }
                    else
                    {
                        TryAttack();
                    }
                }

                LookAt(player.GlobalPosition);
            }
            else
            {
                _pathUpdateTimer.Stop();
            }
        }

        public override void Process(float delta)
        {
        }

        public void ToggleMarkSprite(bool visible)
        {
            _markSprite.Visible = visible;
        }

        protected abstract void TryAttack();

        private void OnPathUpdateTimer()
        {
            UpdatePath();
        }

        private void UpdatePath()
        {
            if (_navigation2D == null)
            {
                if (GameManager.Instance.SceneManager.CurrentLevel != null)
                {
                    _navigation2D = GameManager.Instance.SceneManager.CurrentLevel.Navigation2D;
                }
                else
                {
                    return;
                }
            }

            _path = _navigation2D.GetSimplePath(Position, GameManager.Instance.Player.Position).ToList();
        }
    }
}