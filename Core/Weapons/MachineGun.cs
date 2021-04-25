using Godot;
using Ludum48.Core.Managers;

namespace Ludum48.Core.Weapons
{
    public class MachineGun : WeaponBase
    {
        public MachineGun() : base()
        {
        }

        [Export]
        public override float Damage { get; protected set; } = 15f;

        [Export]
        public override int MagSize { get; protected set; } = 50;

        [Export]
        public override float RateOfFire { get; protected set; } = 20f;

        [Export]
        public override float Recoil { get; protected set; } = 30f;

        [Export]
        public override float ReloadDuration { get; protected set; } = 5f;

        protected override void AdditionalLogic()
        {
            AmmoCount -= AmmoPerShot;

            GameManager.Instance.Player.ApplyImpulse((GlobalPosition - GetGlobalMousePosition()).Normalized() * Recoil);
        }

        protected override void SpawnProjectiles()
        {
            var bullet = InstanceBullet();

            GetParent().GetParent().GetParent().AddChild(bullet);
        }
    }
}