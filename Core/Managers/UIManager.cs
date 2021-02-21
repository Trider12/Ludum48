using Godot;

namespace GlobalGameJam2021.Core.Managers
{
    public class UIManager : CanvasLayer
    {
        private Control _hud;
        private Control _pauseMenu;

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
            GameManager.Instance.SceneManager.LoadMainMenu();
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

            GameManager.Instance.UIManager = this;
        }

        public void ToggleHUD(bool visible)
        {
            _hud.Visible = visible;
        }

        private void TogglePause()
        {
            var tree = GetTree();
            tree.Paused = !tree.Paused;
            _pauseMenu.Visible = tree.Paused;
        }
    }
}