using Godot;

namespace GlobalGameJam2021.Core.Managers
{
    public class SceneManager
    {
        private Node _currentSceneInstance;
        private PackedScene _demoLevelScene;
        private MainMenu _mainMenu;
        private SceneTree _tree;
        private UIManager _uiManager;

        private SceneManager()
        {
            _tree = (SceneTree)Engine.GetMainLoop();
            _mainMenu = _tree.Root.GetNode<MainMenu>("/root/MainMenu"); // start scene
            _uiManager = (UIManager)GD.Load<PackedScene>("res://Scenes/UIManager.tscn").Instance();

            _demoLevelScene = GD.Load<PackedScene>("res://Scenes/DemoLevel.tscn");
        }

        public static SceneManager Instance { get; } = new SceneManager();

        public void LoadDemoLevel()
        {
            LoadLevel(_demoLevelScene.Instance());
        }

        public void LoadLevel(string levelName)
        {
            GD.Print("Next level");
        }

        public void LoadMainMenu()
        {
            _currentSceneInstance.QueueFree();
            _tree.Root.RemoveChild(_uiManager);
            _tree.Root.AddChild(_mainMenu);
            _uiManager.ToggleHUD(false);
        }

        private void LoadLevel(Node level)
        {
            _tree.Root.RemoveChild(_mainMenu);
            _currentSceneInstance = level;
            _tree.Root.AddChild(_uiManager);
            _tree.Root.AddChild(_currentSceneInstance);
            _uiManager.ToggleHUD(true);
        }
    }
}