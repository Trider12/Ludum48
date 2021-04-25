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
        public virtual float MaxHealth { get; protected set; } = 100;
        protected virtual float Acceleration { get; } = 3000;
        protected virtual float Friction { get; } = 3000;
        protected virtual float MaxSpeed { get; } = 200;

        public override void _Ready()
        {
            base._Ready();

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
    }
}