using GlobalGameJam2021.Core.Managers;
using Godot;

public class MainMenu : Control
{
    public override void _EnterTree()
    {
        // call static constructors
        _ = GameManager.Instance;
        _ = SceneManager.Instance;
    }

    public void _on_PlayButton_pressed()
    {
        SceneManager.Instance.LoadDemoLevel();
    }

    public void _on_QuitButton_pressed()
    {
        GetTree().Quit();
    }
}