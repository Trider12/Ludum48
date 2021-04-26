namespace Ludum48.Core.Enemies
{
    public class Avocado : RangedEnemy
    {
        public override void _Ready()
        {
            base._Ready();

            _weapon.BulletName = "AvocadoBullet";
            _weapon.ProjectilesPerShot = 4;
        }
    }
}