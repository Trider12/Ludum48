using Godot;

namespace Ludum48.Core.Weapons
{
    public class Rifle : WeaponBase
    {
        public Rifle() : base()
        {
        }

        [Export]
        public override float RateOfFire { get; protected set; } = 1f;

        protected override void AdditionalLogic()
        {
            AmmoCount -= AmmoPerShot;
        }

        protected override void SpawnProjectiles()
        {
            var bullet = InstanceBullet();

            GetParent().GetParent().AddChild(bullet);
        }
    }
}