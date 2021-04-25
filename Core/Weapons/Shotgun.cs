using Godot;
using Ludum48.Core.Managers;

namespace Ludum48.Core.Weapons
{
    public class Shotgun : WeaponBase
    {
        public Shotgun() : base()
        {
        }

        [Export]
        public override float BulletSpeed { get; protected set; } = 200f;

        [Export]
        public override int MagSize { get; protected set; } = 8;

        [Export]
        public override int ProjectilesPerShot { get; protected set; } = 5;

        [Export]
        public override float RateOfFire { get; protected set; } = 2;

        [Export]
        public override float Recoil { get; protected set; } = 100f;

        [Export]
        public override float ReloadDuration { get; protected set; } = 2f;

        protected override void AdditionalLogic()
        {
            AmmoCount -= AmmoPerShot;

            GameManager.Instance.Player.ApplyImpulse((GlobalPosition - GetGlobalMousePosition()).Normalized() * Recoil);
        }

        protected override void SpawnProjectiles()
        {
            var deltaAngle = 90 / ProjectilesPerShot;

            for (int i = 0; i < ProjectilesPerShot; i++)
            {
                var bullet = InstanceBullet();

                bullet.Direction = (GetGlobalMousePosition() - GlobalPosition).Normalized().Rotated(Mathf.Deg2Rad(-45 + i * deltaAngle));

                GetParent().GetParent().GetParent().AddChild(bullet);
            }
        }
    }
}