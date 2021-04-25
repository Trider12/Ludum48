using Godot;

namespace Ludum48.Core.Weapons
{
    public class Rifle : WeaponBase
    {
        public Rifle() : base()
        {
        }

        [Export]
        public override float RateOfFire { get; protected set; } = 10;

        protected override void AdditionalLogic()
        {
            AmmoCount -= AmmoPerShot;
        }

        protected override void SpawnProjectiles()
        {
            var bullet = InstanceBullet();

            GetParent().GetParent().GetParent().AddChild(bullet);
        }
    }
}