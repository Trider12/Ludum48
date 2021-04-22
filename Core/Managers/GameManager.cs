using Godot;

namespace Ludum48.Core.Managers
{
    public class GameManager
    {
        private GameManager()
        {
        }

        public static GameManager Instance { get; } = new GameManager();

        public SceneManager SceneManager { get; } = new SceneManager();
        public UIManager UIManager { get; } = (UIManager)GD.Load<PackedScene>("res://Scenes/UI/UIManager.tscn").Instance();
    }
}