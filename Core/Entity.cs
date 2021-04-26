using System.Collections.Generic;

using Godot;

using Ludum48.Core.Time;
using Ludum48.Core.Weapons;

namespace Ludum48.Core
{
    public abstract class Entity : TimeObject
    {
        protected List<Bullet> _bullets = new List<Bullet>();
        protected float _currentHealth;
        protected Area2D _hitBox;
        protected Vector2 _velocity = Vector2.Zero;

        public Entity()
        {
            _currentHealth = MaxHealth;
        }

        public virtual float CurrentHealth
        {
            get { return _currentHealth; }

            protected set { _currentHealth = value > 0 ? (value < MaxHealth ? value : MaxHealth) : 0; }
        }

        public override bool IsActive
        {
            get => base.IsActive;

            protected set
            {
                base.IsActive = value;

                if (value)
                {
                    if (_hitBox.GetParent() != this)
                    {
                        CallDeferred("add_child", _hitBox);
                    }
                }
                else
                {
                    if (_hitBox.GetParent() == this)
                    {
                        CallDeferred("remove_child", _hitBox);
                    }
                }
            }
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
        }

        public virtual void GetDamage(float damage, Entity source)
        {
            CurrentHealth -= damage;

            if (CurrentHealth <= 0)
            {
                Die(source);
            }
        }

        public void RegisterBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
            bullet.OwnerEntity = this;
        }

        protected abstract void Die(Entity source);

        private void OnHitBoxBodyEntered(Node body)
        {
            var bullet = body as Bullet;

            if (bullet == null)
            {
                return;
            }

            GetDamage(bullet.Hit(), bullet.OwnerEntity);
        }
    }
}