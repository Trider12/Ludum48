using Godot;
using Ludum48.Core.Managers;

namespace Ludum48.Core.Weapons
{
    public abstract class WeaponBase : Node2D
    {
        protected static readonly PackedScene BulletScene = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");

        protected Node2D _muzzlePoint;
        private int _ammoCount = 0;
        private bool _canShoot = true;
        private Timer _shootTimer = new Timer();
        private Sprite _sprite;

        protected WeaponBase()
        {
            _ammoCount = MagSize;

            _shootTimer.OneShot = true;
            _shootTimer.Connect("timeout", this, nameof(OnShootTimer));
        }

        [Export]
        public virtual float Accuracy { get; protected set; } = 100f;

        public int AmmoCount
        {
            get { return _ammoCount; }

            protected set
            {
                _ammoCount = value > 0 ? (value < MagSize ? value : MagSize) : 0;
                GameManager.Instance.UIManager.UpdateAmmoCount(_ammoCount);
            }
        }

        [Export]
        public virtual int AmmoPerShot { get; protected set; } = 1;

        [Export]
        public virtual float BulletSpeed { get; protected set; } = 300;

        [Export]
        public virtual float Damage { get; protected set; } = 25;

        [Export]
        public virtual string GunDescription { get; protected set; }

        [Export]
        public virtual string GunName { get; protected set; }

        [Export]
        public virtual int MagSize { get; protected set; } = 30;

        [Export]
        public virtual int ProjectilesPerShot { get; protected set; } = 1;

        [Export]
        public virtual float RateOfFire { get; protected set; } = 2f;

        [Export]
        public virtual float Recoil { get; protected set; } = 0f;

        [Export]
        public virtual float ReloadDuration { get; protected set; } = 1f;

        public float ShotDelay => 1f / RateOfFire;

        public override void _Process(float delta)
        {
            if (Input.IsActionPressed("fire"))
            {
                TryShoot();
            }
        }

        public override void _Ready()
        {
            _sprite = GetNode<Sprite>("Sprite");

            _muzzlePoint = GetNode<Node2D>("Muzzle");

            AddChild(_shootTimer);
        }

        public void FinishReloading()
        {
            AmmoCount = MagSize;
        }

        public void LookLeft()
        {
            _sprite.FlipV = true;
        }

        public void LookRight()
        {
            _sprite.FlipV = false;
        }

        public void StartReloading()
        {
            AmmoCount = 0;
        }

        protected abstract void AdditionalLogic();

        protected Bullet InstanceBullet()
        {
            var bullet = (Bullet)BulletScene.Instance();

            bullet.GlobalPosition = _muzzlePoint.GlobalPosition;
            bullet.Direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();
            bullet.Speed = BulletSpeed;
            bullet.Damage = Damage;

            return bullet;
        }

        protected abstract void SpawnProjectiles();

        private void OnShootTimer()
        {
            _canShoot = true;
        }

        private void TryShoot()
        {
            if (!_canShoot || AmmoCount <= 0)
            {
                return;
            }

            SpawnProjectiles();
            AdditionalLogic();

            _canShoot = false;
            _shootTimer.Start(ShotDelay);
        }
    }
}