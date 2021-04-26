using Godot;

using Ludum48.Core.Time;
using Ludum48.Core.Weapons;

namespace Ludum48.Core.Enemies
{
    public class RangedEnemy : Enemy
    {
        protected WeaponBase _weapon;
        private AnimatedSprite _animatedSprite;

        protected override float StopAtRadius { get; } = 1000f;

        protected override float WalkRadius { get; } = 1500f;

        public override void _Ready()
        {
            base._Ready();

            _weapon = GetNode<WeaponBase>("Weapon");
            _weapon.OwnerEntity = this;

            _animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            _animatedSprite.Connect("animation_finished", this, nameof(OnAnimationFinished));
        }

        public void OnAnimationFinished()
        {
            _weapon.TryShoot();
            _animatedSprite.Stop();
            _animatedSprite.Frame = 0;
        }

        public override void StartRewind()
        {
            base.StartRewind();

            _animatedSprite.Stop();
        }

        protected override void ApplyFrame(BaseTimeFrame frame)
        {
            base.ApplyFrame(frame);

            _animatedSprite.Frame = (frame as SpriteTimeFrame).SpriteFrame;
        }

        protected override void Die(Node source)
        {
            IsActive = false;

            if (TimeState == TimeState.Replay)
            {
                EraseFromFuture();
            }

            _animatedSprite.Stop();
            _animatedSprite.Frame = 0;
        }

        protected override BaseTimeFrame GetTimeFrame()
        {
            return new SpriteTimeFrame { Position = Position, Rotation = Rotation, IsActive = IsActive, SpriteFrame = _animatedSprite.Frame };
        }

        protected override void TryAttack()
        {
            if (!_animatedSprite.Playing)
            {
                _animatedSprite.Play("attack");
            }
        }
    }
}