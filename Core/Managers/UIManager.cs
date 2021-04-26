using Godot;

using Ludum48.Core.UI;

namespace Ludum48.Core.Managers
{
    public class UIManager : CanvasLayer
    {
        private const int AmmoBubbleSize = 4;
        private const int HeartSize = 8;
        private const int HeartValue = 10;

        private TextureRect _ammoTexture;
        private Clock _clock;
        private TextureRect _healthEmptyTexture;
        private TextureRect _healthFullTexture;

        private Control _hud;
        private Control _pauseMenu;
        private ColorRect _replayScreen;
        private ColorRect _rewindScreen;
        private Label _timerLabel;
        private ColorRect _timerRect;

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
            {
                TogglePause();
            }
        }

        public void _on_MainMenuButton_pressed()
        {
            TogglePause();
            GameManager.Instance.LoadMenu();
        }

        public void _on_PauseButton_pressed()
        {
            TogglePause();
        }

        public void _on_ResumeButton_pressed()
        {
            var tree = GetTree();
            tree.Paused = false;
            _pauseMenu.Visible = false;
        }

        public void _on_SaveButton_pressed()
        {
        }

        public override void _Ready()
        {
            _hud = GetNode<Control>("HUD");
            _pauseMenu = GetNode<Control>("PauseMenu");
            _rewindScreen = GetNode<ColorRect>("RewindScreen");
            _replayScreen = GetNode<ColorRect>("ReplayScreen");
            _timerLabel = GetNode<Label>("HUD/Timer/Label");
            _timerRect = GetNode<ColorRect>("HUD/Timer");
            _clock = GetNode<Clock>("HUD/Clock");

            _ammoTexture = GetNode<TextureRect>("HUD/LowerLeft/AmmoBar");
            _healthEmptyTexture = GetNode<TextureRect>("HUD/LowerLeft/HealthEmpty");
            _healthFullTexture = GetNode<TextureRect>("HUD/LowerLeft/HealthEmpty/HealthFull");
        }

        public void ToggleHUD(bool visible)
        {
            _hud.Visible = visible;
        }

        public void ToggleReplayScreen(bool visible)
        {
            _replayScreen.Visible = visible;
        }

        public void ToggleRewindScreen(bool visible)
        {
            _rewindScreen.Visible = visible;
        }

        public void UpdateAmmoCount(int count)
        {
            var rectSize = new Vector2(count * AmmoBubbleSize, AmmoBubbleSize);

            _ammoTexture.RectSize = rectSize;

            var rect = new Rect2(0, 0, rectSize);

            ((AtlasTexture)_ammoTexture.Texture).Region = rect;
        }

        //public void ToggleTimer(bool visible)
        //{
        //    //_timerRect.Visible = visible;
        //    _clock.Visible = visible;
        //}
        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            var emptyHeartsCount = Mathf.Round(maxHealth / HeartValue);
            var halfHeartsCount = Mathf.Round(currentHealth / HeartValue * 2);

            ((AtlasTexture)_healthEmptyTexture.Texture).Region = new Rect2(0, 0, emptyHeartsCount * HeartSize, HeartSize);
            ((AtlasTexture)_healthFullTexture.Texture).Region = new Rect2(0, 0, halfHeartsCount * HeartSize / 2, HeartSize);
        }

        public void UpdateTimer(float percent, int depth, bool rewind)
        {
            //_timerRect.Color = rewind || depth == 0 ? Colors.White : depth == 1 ? Colors.Green : depth == 2 ? Colors.Yellow : Colors.Red;
            //_timerLabel.Text = percent.ToString("0.00");

            _clock.UpdateTime(percent, depth);
        }

        public void UpdateTimeScale(float scale)
        {
            _clock.UpdateTimeSpeed(scale);
        }

        private void TogglePause()
        {
            var tree = GetTree();
            tree.Paused = !tree.Paused;
            _pauseMenu.Visible = tree.Paused;
        }
    }
}