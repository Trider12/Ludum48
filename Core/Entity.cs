using GlobalGameJam2021.Core.UI;
using Godot;
using Ludum48.Core.Weapons;
using System.Collections.Generic;

namespace Ludum48.Core
{
    public abstract class Entity : TimeObject
    {
        protected List<Bullet> _bullets;
        protected float _currentHealth;

        protected Area2D _hitBox;
        protected Vector2 _velocity = Vector2.Zero;

        private TimeIndicator _timeIndicator;

        public Entity()
        {
            _currentHealth = MaxHealth;
        }

        public virtual float CurrentHealth
        {
            get { return _currentHealth; }

            protected set { _currentHealth = value > 0 ? (value < MaxHealth ? value : MaxHealth) : 0; }
        }

        public bool IsClone { get; set; } = false;
        public virtual float MaxHealth { get; protected set; } = 1;
        protected virtual float Acceleration { get; } = 3000;
        protected virtual float Friction { get; } = 3000;
        protected virtual float MaxSpeed { get; } = 200;

        public override void _Ready()
        {
            base._Ready();

            _hitBox = GetNode<Area2D>("HitBox");
            _hitBox.Connect("body_entered", this, nameof(OnHitBoxBodyEntered));

            _timeIndicator = GetNode<TimeIndicator>("TimeIndicator");
        }

        public virtual void GetDamage(float damage)
        {
            CurrentHealth -= damage;

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        public void RegisterBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
            bullet.Owner = this;
        }

        public void ToggleTimeIndicator(Color color)
        {
            _timeIndicator.ChangeColor(color);
        }

        protected abstract void Die();

        private void OnHitBoxBodyEntered(Node body)
        {
            var bullet = body as Bullet;

            if (bullet == null)
            {
                return;
            }

            GetDamage(bullet.Pop());
        }
    }
}