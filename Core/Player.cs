using Godot;
using Ludum48.Core.Managers;
using Ludum48.Core.UI;
using Ludum48.Core.Utility;
using Ludum48.Core.Weapons;
using System.Collections.Generic;
using System.Linq;

namespace Ludum48.Core
{
    public class Player : Entity
    {
        private const float DashDelay = 1;
        private const float DashForce = 400;

        private static readonly Dictionary<string, PackedScene> _weaponScenes = PrefabHelper.LoadPrefabsDictionary("res://Scenes/Weapons");
        private Camera2D _camera;
        private bool _canDash = true;
        private int _currentFrame = 0;
        private WeaponBase _currentWeapon = null;
        private Timer _dashTimer = new Timer();
        private Node2D _gunSlot;
        private Area2D _hitbox;
        private float _maxHealth = 1;
        private ReloadBar _reloadBar;

        private List<WeaponBase> _weapons = _weaponScenes.Select(kv => (WeaponBase)kv.Value.Instance()).ToList();

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
            if (inputEvent.IsActionPressed("slot1"))
            {
                EquipWeapon(0);
            }
            if (inputEvent.IsActionPressed("slot2"))
            {
                EquipWeapon(1);
            }
            if (inputEvent.IsActionPressed("slot3"))
            {
                EquipWeapon(2);
            }
            if (inputEvent.IsActionPressed("reload"))
            {
                _currentWeapon.StartReloading();
                _reloadBar.StartReloading(_currentWeapon.ReloadDuration);
            }
            if (inputEvent.IsActionPressed("dash"))
            {
                Dash();
            }
            if (inputEvent.IsActionPressed("rewind"))
            {
                GameManager.Instance.StartRewind();
            }
        }

        public override void _Ready()
        {
            base._Ready();

            _camera = GetNode<Camera2D>("Camera2D");

            _reloadBar = GetNode<ReloadBar>("ReloadBar");
            _reloadBar.Connect(nameof(ReloadBar.ReloadFinished), this, nameof(OnReloadBarFinished));

            _gunSlot = GetNode<Node2D>("GunSlot");

            _hitbox = GetNode<Area2D>("Hitbox");
            _hitbox.Connect("area_entered", this, nameof(OnHitboxAreaEntered));

            UpdateHUD();

            EquipWeapon();

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
        }

        public void Reset()
        {
            CurrentHealth = MaxHealth;

            foreach (var weapon in _weapons)
            {
                weapon.FinishReloading();
            }

            _velocity = Vector2.Zero;

            EquipWeapon(0);
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

        protected override void Die()
        {
            GameManager.Instance.SceneManager.LoadMainMenu();
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

        private void EquipWeapon(int index = 0)
        {
            if (_currentWeapon != null)
            {
                _gunSlot.RemoveChild(_currentWeapon);
                _reloadBar.InterruptReloading();
            }

            _currentWeapon = _weapons.ElementAt(index);

            _gunSlot.AddChild(_currentWeapon);
            _currentWeapon.Position = Vector2.Zero;
            GameManager.Instance.UIManager.UpdateAmmoCount(_currentWeapon.AmmoCount);
        }

        private void OnDashTimeout()
        {
            _canDash = true;
        }

        private void OnHitboxAreaEntered(Area2D area)
        {
            //if (area.Name.Equals("AttackBox"))
            //{
            //    GetDamage((area.GetParent() as Enemy).Damage);
            //    _animationState.Travel("Hurt");
            //}
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
        }
    }
}