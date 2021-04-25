using Godot;
using System.Collections.Generic;

namespace Ludum48.Core.Managers
{
    public enum TimeState { Replay, Rewind, Normal }

    public class GameManager : Object
    {
        public const int MaxFramesPlayed = RewindTime * 60;
        public const int MaxFramesStored = MaxFramesPlayed * 8;
        public const int RewindTime = 3;

        private static PackedScene PlayerScene = GD.Load<PackedScene>("res://Scenes/Player.tscn");
        private List<TimeObject> _timeObjects = new List<TimeObject>();
        private LinkedList<Player> Players = new LinkedList<Player>();

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

        public void NewPlayer()
        {
            Player.Activate(false);
            Player.ToggleTimeIndicator(true, MainPlayer.Depth == 1 ? Colors.Green : MainPlayer.Depth == 2 ? Colors.Yellow : Colors.Red);

            var oldPlayer = Player;

            Player = (Player)PlayerScene.Instance();
            SceneManager.CurrentLevel.SpawnPlayer();
            Player.GlobalPosition = oldPlayer.GlobalPosition;
            Player.GlobalRotation = oldPlayer.GlobalRotation;
            Player.UpdateAmmoCount();
            Players.AddLast(Player);
        }

        public void Register(TimeObject timeObject)
        {
            _timeObjects.Add(timeObject);

            timeObject.FrameFactor = System.Math.Max(1, MainPlayer.Depth * 2);
        }

        public void ResetPlayer()
        {
            foreach (var player in Players)
            {
                Unregister(player);

                player.Activate(false);
                player.QueueFree();
            }

            Players.Clear();

            Player = MainPlayer;
            Player.Activate(true);
            Player.ToggleTimeIndicator(false, Colors.White);
            Player.UpdateAmmoCount();
        }

        public void SlowDown()
        {
            foreach (var obj in _timeObjects)
            {
                obj.Depth++;
            }
        }

        public void StartNormal()
        {
            UIManager.ToggleReplayScreen(false);
            UIManager.ToggleTimer(false);

            foreach (var obj in _timeObjects)
            {
                obj.StartNormal();
            }

            ResetPlayer();
        }

        public void StartReplay()
        {
            UIManager.ToggleRewindScreen(false);
            UIManager.ToggleReplayScreen(true);

            foreach (var obj in _timeObjects)
            {
                obj.Depth++;
                obj.StartReplay();
            }

            NewPlayer();
        }

        public void StartRewind()
        {
            if (!MainPlayer.CanRewind())
            {
                return;
            }

            UIManager.ToggleRewindScreen(true);
            UIManager.ToggleTimer(true);

            foreach (var obj in _timeObjects)
            {
                obj.StartRewind();
            }
        }

        public void Unregister(TimeObject timeObject)
        {
            _timeObjects.Remove(timeObject);
        }

        private void OnMainPlayerFrameChanged(bool rewind)
        {
            float time = RewindTime * (1f - 1f * MainPlayer.CurrentFrame / MaxFramesPlayed);
            UIManager.UpdateTimer(time, MainPlayer.Depth, rewind);
        }
    }
}