namespace Ludum48.Core.Enemies
{
    public class Boss : RangedEnemy
    {
        public override void _Ready()
        {
            base._Ready();

            _weapon.BulletName = "BossBullet";
            _weapon.ProjectilesPerShot = 1;
        }
    }
}