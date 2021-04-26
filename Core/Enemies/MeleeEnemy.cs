using Godot;

using Ludum48.Core.Time;
using Ludum48.Core.Weapons;

namespace Ludum48.Core.Enemies
{
    public class MeleeEnemy : Enemy
    {
        protected AnimationPlayer _animationPlayer = null;
        protected Area2D _armorBox;

        public override bool IsActive
        {
            get => base.IsActive;

            protected set
            {
                base.IsActive = value;

                if (value)
                {
                    if (_armorBox.GetParent() != this)
                    {
                        CallDeferred("add_child", _armorBox);
                    }
                }
                else
                {
                    if (_armorBox.GetParent() == this)
                    {
                        CallDeferred("remove_child", _armorBox);
                    }
                }
            }
        }

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _armorBox = GetNode<Area2D>("ArmorBox");
            _armorBox.Connect("body_entered", this, nameof(OnArmorBoxBodyEntered));
        }

        protected override void Die(Node source)
        {
            IsActive = false;

            if (TimeState == TimeState.Replay)
            {
                EraseFromFuture();
            }
        }

        protected override void TryAttack()
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

            bullet.Hit();
        }
    }
}