using Godot;

namespace GlobalGameJam2021.Core
{
    public class SceneManager
    {
        public static SceneManager Instance { get; } = new SceneManager();

        private PackedScene _mainMenuScene;
        private PackedScene _demoLevelScene;

        private SceneTree _tree;

        private SceneManager()
        {
            _tree = (SceneTree)Engine.GetMainLoop();
            _mainMenuScene = GD.Load<PackedScene>("res://Scenes/MainMenu.tscn");
            _demoLevelScene = GD.Load<PackedScene>("res://Scenes/DemoLevel.tscn");
        }

        public void ChangeSceneToDemoLevel() => _tree.ChangeSceneTo(_demoLevelScene);

        public void ChangeSceneToMainMenu() => _tree.ChangeSceneTo(_mainMenuScene);
    }
}