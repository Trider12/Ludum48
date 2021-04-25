using Godot;
using Ludum48.Core.Enemies;
using System.Collections.Generic;

namespace Ludum48.Core.Managers
{
    public enum TimeState { Replay, Rewind, Normal }

    public class GameManager : Object
    {
        public const int MaxFramesPlayed = RewindTime * 60;
        public const int MaxFramesStored = MaxFramesPlayed * 8;
        public const int RewindTime = 3;

        private static PackedScene EnemyScene = GD.Load<PackedScene>("res://Scenes/Enemies/Enemy1.tscn");
        private static PackedScene PlayerScene = GD.Load<PackedScene>("res://Scenes/Player.tscn");

        private List<Enemy> _currentEnemies = new List<Enemy>();
        private List<Enemy> _enemyClones = new List<Enemy>();
        private List<Enemy> _originalEnemies = new List<Enemy>();
        private LinkedList<Player> _players = new LinkedList<Player>();
        private List<TimeObject> _timeObjects = new List<TimeObject>();

        private GameManager()
        {
            Player = MainPlayer;
            Player.Connect(nameof(Player.OnFrameChanged), this, nameof(OnMainPlayerFrameChanged));
        }

        public static GameManager Instance { get; } = new GameManager();

        public Player MainPlayer { get; } = (Player)PlayerScene.Instance();

        public Player Player { get; private set; }

        public SceneManager SceneManager { get; } = new SceneManager();

        public UIManager UIManager { get; } = (UIManager)GD.Load<PackedScene>("res://Scenes/UI/UIManager.tscn").Instance();

        public void Register(TimeObject timeObject)
        {
            _timeObjects.Add(timeObject);

            timeObject.FrameFactor = System.Math.Max(1, MainPlayer.Depth * 2);

            var enemy = timeObject as Enemy;

            if (enemy != null)
            {
                if (enemy.IsClone)
                {
                    _enemyClones.Add(enemy);
                }
                else
                {
                    _originalEnemies.Add(enemy);
                }
            }
        }

        public void StartNormal()
        {
            UIManager.ToggleReplayScreen(false);
            //UIManager.ToggleTimer(false);
            UIManager.UpdateTimer(0, 0, false);

            foreach (var obj in _timeObjects)
            {
                obj.StartNormal();
            }

            ResetPlayer();
            ResetEnemies();
        }

        public void StartReplay(bool slowdown)
        {
            UIManager.ToggleRewindScreen(false);
            UIManager.ToggleReplayScreen(true);

            foreach (var obj in _timeObjects)
            {
                if (slowdown)
                {
                    obj.Depth++;
                }

                obj.StartReplay();
            }

            var color = MainPlayer.Depth == 1 ? Colors.Green : MainPlayer.Depth == 2 ? Colors.Yellow : Colors.Red;

            NewPlayer(color);
            NewEnemies(color);
        }

        public void StartRewind()
        {
            if (!MainPlayer.CanRewind())
            {
                return;
            }

            UIManager.ToggleRewindScreen(true);
            //UIManager.ToggleTimer(true);

            foreach (var obj in _timeObjects)
            {
                obj.StartRewind();
            }
        }

        public void Unregister(TimeObject timeObject)
        {
            _timeObjects.Remove(timeObject);
        }

        private void NewEnemies(Color color)
        {
            if (_currentEnemies.Count == 0)
            {
                _currentEnemies = new List<Enemy>(_originalEnemies);
            }

            foreach (var enemy in _currentEnemies)
            {
                enemy.ToggleTimeIndicator(color);
            }

            _currentEnemies.Clear();

            foreach (var enemy in _originalEnemies)
            {
                var clone = EnemyScene.Instance<Enemy>();
                clone.IsClone = true;
                clone.GlobalPosition = enemy.GlobalPosition;
                clone.GlobalRotation = enemy.GlobalRotation;
                SceneManager.CurrentLevel.SpawnEntity(clone);
                _currentEnemies.Add(clone);
            }
        }

        private void NewPlayer(Color color)
        {
            Player.Activate(false);
            Player.ToggleTimeIndicator(color);

            var oldPlayer = Player;

            Player = PlayerScene.Instance<Player>();
            SceneManager.CurrentLevel.SpawnPlayer();
            Player.GlobalPosition = oldPlayer.GlobalPosition;
            Player.GlobalRotation = oldPlayer.GlobalRotation;
            Player.UpdateAmmoCount();
            _players.AddLast(Player);
        }

        private void OnMainPlayerFrameChanged(bool rewind)
        {
            float percent = 1f * MainPlayer.CurrentFrame / MaxFramesPlayed;
            UIManager.UpdateTimer(percent, MainPlayer.Depth, rewind);
        }

        private void ResetEnemies()
        {
            foreach (var clone in _enemyClones)
            {
                Unregister(clone);

                clone.QueueFree();
            }

            _enemyClones.Clear();
            _currentEnemies.Clear();

            foreach (var enemy in _originalEnemies)
            {
                enemy.ToggleTimeIndicator(Colors.White);
            }
        }

        private void ResetPlayer()
        {
            foreach (var player in _players)
            {
                Unregister(player);

                player.Activate(false);
                player.QueueFree();
            }

            _players.Clear();

            Player = MainPlayer;
            Player.Activate(true);
            Player.ToggleTimeIndicator(Colors.White);
            Player.UpdateAmmoCount();
        }
    }
}