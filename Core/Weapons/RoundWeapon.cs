using Godot;

namespace Ludum48.Core.Weapons
{
    public class RoundWeapon : WeaponBase
    {
        protected override void AdditionalLogic()
        {
        }

        protected override void SpawnProjectiles()
        {
            var deltaAngle = 360 / ProjectilesPerShot;
            var muzzleDistance = Position.DistanceTo(_muzzlePoint.Position);
            var direction = GlobalPosition.DirectionTo(_muzzlePoint.GlobalPosition);

            for (int i = 0; i < ProjectilesPerShot; i++)
            {
                var bullet = InstanceBullet();

                bullet.Direction = direction.Rotated(Mathf.Deg2Rad(i * deltaAngle));
                bullet.GlobalPosition = GlobalPosition + bullet.Direction * muzzleDistance;
                bullet.GlobalRotation = bullet.Direction.Angle();

                GetParent().GetParent().AddChild(bullet);
            }
        }
    }
}