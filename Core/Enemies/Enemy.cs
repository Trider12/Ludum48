using Godot;
using Ludum48.Core.Managers;
using Ludum48.Core.Weapons;
using System.Collections.Generic;
using System.Linq;

namespace Ludum48.Core.Enemies
{
    public class Enemy : Entity
    {
        protected AnimationPlayer _animationPlayer = null;
        protected Area2D _armorBox;
        protected float SightRadius = 800;
        protected float StopAtRadius = 90;
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

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _armorBox = GetNode<Area2D>("ArmorBox");
            _armorBox.Connect("body_entered", this, nameof(OnArmorBoxBodyEntered));

            AddChild(_pathUpdateTimer);
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
            IsActive = false;
        }

        private void Attack()
        {
            if (!_animationPlayer.IsPlaying())
            {
                _animationPlayer.Play("attack");
            }
        }

        private void OnArmorBoxBodyEntered(Node body)
        {
            var bullet = body as Bullet;

            if (bullet == null)
            {
                return;
            }

            bullet.Pop();
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