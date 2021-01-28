using GlobalGameJam2021.Core;
using Godot;

public class MainMenu : Control
{
    public void _on_PlayButton_pressed()
    {
        SceneManager.Instance.ChangeSceneToDemoLevel();
    }

    public void _on_QuitButton_pressed()
    {
        GetTree().Quit();
    }
}