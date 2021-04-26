namespace Ludum48.Core.Enemies
{
    public class Star : RangedEnemy
    {
        public override void _Ready()
        {
            base._Ready();

            _weapon.BulletName = "WaterMelonBullet";
            _weapon.ProjectilesPerShot = 5;
        }
    }
}