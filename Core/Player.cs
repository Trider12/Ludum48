using Godot;

using Ludum48.Core.Enemies;
using Ludum48.Core.Managers;
using Ludum48.Core.Time;
using Ludum48.Core.UI;
using Ludum48.Core.Weapons;

namespace Ludum48.Core
{
    public class Player : Entity
    {
        private const float DashDelay = 1;
        private const float DashForce = 400;

        private Camera2D _camera;
        private bool _canDash = true;
        private int _currentFrame = 0;
        private WeaponBase _currentWeapon = null;
        private Timer _dashTimer = new Timer();

        private float _maxHealth = 1;
        private ReloadBar _reloadBar;
        private TimeIndicator _timeIndicator;

        public Player() : base()
        {
            _dashTimer.OneShot = true;
            _dashTimer.Connect("timeout", this, nameof(OnDashTimeout));
        }

        [Signal]
        public delegate void OnFrameChanged(bool rewind);

        public int CurrentFrame
        {
            get { return _currentFrame; }

            private set
            {
                _currentFrame = value;
                EmitSignal(nameof(OnFrameChanged), TimeState == TimeState.Rewind);
            }
        }

        public override float CurrentHealth
        {
            get { return _currentHealth; }

            protected set
            {
                base.CurrentHealth = value;
                GameManager.Instance.UIManager.UpdateHealth(_currentHealth, _maxHealth);
            }
        }

        public override float MaxHealth
        {
            get { return _maxHealth; }

            protected set
            {
                _maxHealth = value;
                GameManager.Instance.UIManager.UpdateHealth(_currentHealth, _maxHealth);
            }
        }

        protected override float MaxSpeed { get; } = 300f;

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("reload"))
            {
                _currentWeapon.StartReloading();
                _reloadBar.StartReloading(_currentWeapon.ReloadDuration);
            }
            if (inputEvent.IsActionPressed("dash"))
            {
                Dash();
            }
            //if (inputEvent.IsActionPressed("rewind"))
            //{
            //    GameManager.Instance.StartRewind();
            //}
        }

        public override void _Ready()
        {
            base._Ready();

            _camera = GetNode<Camera2D>("Camera2D");

            _reloadBar = GetNode<ReloadBar>("ReloadBar");
            _reloadBar.Connect(nameof(ReloadBar.ReloadFinished), this, nameof(OnReloadBarFinished));

            _hitBox.Connect("area_entered", this, nameof(OnHitBoxAreaEntered));

            _timeIndicator = GetNode<TimeIndicator>("TimeIndicator");

            _currentWeapon = GetNode<WeaponBase>("Weapon");

            UpdateHUD();

            AddChild(_dashTimer);

            GameManager.Instance.SceneManager.Connect(nameof(SceneManager.OnLevelChange), this, nameof(OnLevelChange));
        }

        public void Activate(bool value)
        {
            SetProcessInput(value);
            _camera.Current = value;
            _currentWeapon.SetProcess(value);
        }

        public void ApplyImpulse(Vector2 velocity)
        {
            _velocity += velocity;
        }

        public bool CanRewind()
        {
            return TimeState != TimeState.Rewind && Depth < 3 && SavedFrames.Count >= GameManager.MaxFramesPlayed;
        }

        public void DeleteBullets()
        {
            foreach (var bullet in _bullets)
            {
                GameManager.Instance.Unregister(bullet);
                bullet.QueueFree();
            }
        }

        public override void PhysicsProcess(float delta)
        {
            var inputVector = new Vector2(Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"), Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")).Normalized();

            if (inputVector.IsEqualApprox(Vector2.Zero))
            {
                _velocity = _velocity.MoveToward(Vector2.Zero, Friction * delta);
            }
            else
            {
                _velocity = _velocity.MoveToward(inputVector * MaxSpeed, Acceleration * delta);
            }

            _velocity = MoveAndSlide(_velocity);

            LookAt(GetGlobalMousePosition());
        }

        public override void Process(float delta)
        {
            if (Input.IsActionPressed("fire"))
            {
                _currentWeapon.TryShoot();
            }
        }

        public void ToggleTimeIndicator(Color color)
        {
            _timeIndicator.ChangeColor(color);
        }

        public void UpdateAmmoCount()
        {
            GameManager.Instance.UIManager.UpdateAmmoCount(_currentWeapon.AmmoCount);
        }

        protected override void CustomNormalLogic()
        {
            _currentFrame = System.Math.Min(_currentFrame + 1, GameManager.MaxFramesPlayed * FrameFactor);
        }

        protected override void CustomReplayLogic()
        {
            CurrentFrame++;

            if (CurrentFrame == GameManager.MaxFramesPlayed * FrameFactor)
            {
                GameManager.Instance.StartNormal();
            }
        }

        protected override void CustomRewindLogic()
        {
            CurrentFrame--;

            if (CurrentFrame == 0)
            {
                GameManager.Instance.StartReplay(true);
            }
        }

        protected override void Die(Entity source)
        {
            GameManager.Instance.OnPlayerDeath(source as Enemy);
        }

        private void Dash()
        {
            if (!_canDash)
            {
                return;
            }

            var dir = (GetGlobalMousePosition() - GlobalPosition).Normalized();

            ApplyImpulse(dir * DashForce);

            _canDash = false;

            _dashTimer.Start(DashDelay);
        }

        private void OnDashTimeout()
        {
            _canDash = true;
        }

        private void OnHitBoxAreaEntered(Area2D area)
        {
            if (area.CollisionLayer == 256) // EnemyHitbox
            {
                GetDamage(1, area.GetParent<Entity>());
            }
        }

        private void OnLevelChange()
        {
            _reloadBar.InterruptReloading();
        }

        private void OnReloadBarFinished()
        {
            _currentWeapon.FinishReloading();
        }

        private void UpdateHUD()
        {
            var uiManager = GameManager.Instance.UIManager;

            uiManager.UpdateHealth(CurrentHealth, MaxHealth);
            uiManager.UpdateAmmoCount(_currentWeapon.AmmoCount);
        }
    }
}