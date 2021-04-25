using Godot;
using Ludum48.Core.Managers;
using System.Collections.Generic;
using System.Linq;

namespace Ludum48.Core.Enemies
{
    public class Enemy : Entity
    {
        protected float SightRadius = 800;
        protected float StopAtRadius = 50;
        protected float WalkRadius = 600;

        private Navigation2D _navigation2D;
        private List<Vector2> _path = new List<Vector2>();
        private float _pathUpdateInterval = 0.5f;
        private Timer _pathUpdateTimer = new Timer();

        public Enemy() : base()
        {
            _pathUpdateTimer.Connect("timeout", this, nameof(OnPathUpdateTimer));
        }

        public override void _Ready()
        {
            base._Ready();

            AddChild(_pathUpdateTimer);
        }

        public override void PhysicsProcess(float delta)
        {
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

                        var velocity = (_path[1] - GlobalPosition).Normalized() * MaxSpeed;

                        MoveAndSlide(velocity);
                    }
                    else
                    {
                        Attack();
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

        protected override void Die()
        {
        }

        private void Attack()
        {
        }

        private void OnPathUpdateTimer()
        {
            UpdatePath();
        }

        private void UpdatePath()
        {
            if (_navigation2D == null)
            {
                _navigation2D = GameManager.Instance.SceneManager.CurrentLevel.Navigation2D;
            }

            _path = _navigation2D.GetSimplePath(Position, GameManager.Instance.Player.Position).ToList();
        }
    }
}