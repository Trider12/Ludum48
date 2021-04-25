using Godot;
using Ludum48.Core.Weapons;
using System.Collections.Generic;

namespace Ludum48.Core
{
    public abstract class Entity : TimeObject
    {
        protected AnimationPlayer _animationPlayer = null;
        protected AnimationNodeStateMachinePlayback _animationState = null;
        protected AnimationTree _animationTree = null;
        protected List<Bullet> _bullets;
        protected float _currentHealth;

        public Entity()
        {
            _currentHealth = MaxHealth;
        }

        public virtual float CurrentHealth
        {
            get { return _currentHealth; }

            protected set { _currentHealth = value > 0 ? (value < MaxHealth ? value : MaxHealth) : 0; }
        }

        public virtual float MaxHealth { get; set; } = 100;

        protected virtual float Acceleration { get; } = 1000;

        protected virtual float Friction { get; } = 500;

        protected virtual float MaxSpeed { get; } = 200;

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

        protected abstract void Die();
    }
}