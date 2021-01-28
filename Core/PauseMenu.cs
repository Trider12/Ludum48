using GlobalGameJam2021.Core;
using Godot;

public class PauseMenu : Control
{
    private void TogglePause()
    {
        var tree = GetTree();
        tree.Paused = !tree.Paused;
        Visible = tree.Paused;
    }

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
        SceneManager.Instance.ChangeSceneToMainMenu();
    }
    public void _on_ResumeButton_pressed()
    {
        var tree = GetTree();
        tree.Paused = false;
        Visible = false;
    }

    public void _on_SaveButton_pressed()
    {
    }
}