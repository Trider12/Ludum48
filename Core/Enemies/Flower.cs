namespace Ludum48.Core.Enemies
{
    public class Flower : RangedEnemy
    {
        public override void _Ready()
        {
            base._Ready();

            _weapon.BulletName = "FlowerBullet";
            _weapon.ProjectilesPerShot = 8;
        }
    }
}