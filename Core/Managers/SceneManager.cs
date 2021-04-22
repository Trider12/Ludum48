using Godot;
using Ludum48.Core.UI;
using Ludum48.Core.Utility;
using System.Collections.Generic;

namespace Ludum48.Core.Managers
{
    public class SceneManager : Node2D
    {
        private static readonly Dictionary<string, PackedScene> _levels = PrefabHelper.LoadPrefabsDictionary("res://Scenes/Levels", new string[] { });
        private MainMenu _mainMenu;
        private SceneTree _tree;

        public SceneManager()
        {
            _tree = (SceneTree)Engine.GetMainLoop();
            _mainMenu = _tree.Root.GetNode<MainMenu>("/root/MainMenu"); // start scene
        }

        [Signal]
        public delegate void OnLevelChange();

        public Level CurrentLevel { get; private set; } = null;

        public void LoadDemoLevel()
        {
            LoadLevel("empty");
        }

        public void LoadLevel(string levelName)
        {
            LoadLevel(_levels[levelName].Instance());
        }

        public void LoadMainMenu()
        {
            _tree.Root.RemoveChild(GameManager.Instance.UIManager);
            _tree.Root.AddChild(_mainMenu);
            GameManager.Instance.UIManager.ToggleHUD(false);
        }

        private void LoadLevel(Node level)
        {
            EmitSignal(nameof(OnLevelChange));

            if (CurrentLevel == null)
            {
                _tree.Root.RemoveChild(_mainMenu);
                _tree.Root.AddChild(GameManager.Instance.UIManager);
            }
            else
            {
                CurrentLevel.RemovePlayer();
                CurrentLevel.QueueFree();
            }

            CurrentLevel = (Level)level;
            _tree.Root.CallDeferred("add_child", CurrentLevel);
            CurrentLevel.CallDeferred(nameof(CurrentLevel.SpawnPlayer));

            GameManager.Instance.UIManager.ToggleHUD(true);
        }
    }
}