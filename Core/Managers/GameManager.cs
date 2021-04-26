using System.Collections.Generic;

using Godot;

using Ludum48.Core.Enemies;
using Ludum48.Core.Time;

namespace Ludum48.Core.Managers
{
    public class GameManager : Object
    {
        public const int MaxFramesPlayed = RewindTime * 60;
        public const int MaxFramesStored = MaxFramesPlayed * 8;
        public const int RewindTime = 3;

        private static PackedScene PlayerScene = GD.Load<PackedScene>("res://Scenes/Player.tscn");
        private Enemy _deathSource = null;
        private List<TimeObject> _timeObjects = new List<TimeObject>();

        private GameManager()
        {
            ResetGame();
        }

        public static GameManager Instance { get; } = new GameManager();

        public Player MainPlayer { get; private set; }

        public Player Player { get; private set; }

        public SceneManager SceneManager { get; } = new SceneManager();

        public UIManager UIManager { get; } = (UIManager)GD.Load<PackedScene>("res://Scenes/UI/UIManager.tscn").Instance();

        public void OnPlayerDeath(Enemy source)
        {
            if (MainPlayer.Depth > 0)
            {
                ResetGame();
                SceneManager.LoadMainMenu();
            }
            else
            {
                _deathSource = source;
                _deathSource.ToggleMarkSprite(true);

                StartRewind();
            }
        }

        public void Register(TimeObject timeObject)
        {
            _timeObjects.Add(timeObject);

            timeObject.FrameFactor = System.Math.Max(1, MainPlayer.Depth * 2);
        }

        public void StartNormal()
        {
            UIManager.ToggleReplayScreen(false);
            UIManager.UpdateTimer(0, 0, false);

            foreach (var obj in _timeObjects)
            {
                obj.StartNormal();
            }

            ResetPlayer();

            UIManager.UpdateTimeScale(MainPlayer.TimeScale);
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

            UIManager.UpdateTimeScale(MainPlayer.TimeScale);
        }

        public void StartRewind()
        {
            if (!MainPlayer.CanRewind())
            {
                //Debugger.Break();
                return;
            }

            UIManager.ToggleRewindScreen(true);

            foreach (var obj in _timeObjects)
            {
                obj.StartRewind();
            }
        }

        public void Unregister(TimeObject timeObject)
        {
            _timeObjects.Remove(timeObject);
        }

        private void NewPlayer(Color color)
        {
            var oldPlayer = Player;

            Player = PlayerScene.Instance<Player>();
            SceneManager.CurrentLevel.SpawnPlayer();
            Player.GlobalPosition = oldPlayer.GlobalPosition;
            Player.GlobalRotation = oldPlayer.GlobalRotation;
            Player.UpdateAmmoCount();

            oldPlayer.Activate(false);

            if (oldPlayer == MainPlayer)
            {
                oldPlayer.ToggleTimeIndicator(color);
            }
            else
            {
                Unregister(oldPlayer);
                oldPlayer.DeleteBullets();
                oldPlayer.QueueFree();
            }
        }

        private void OnMainPlayerFrameChanged(bool rewind)
        {
            float percent = 1f * MainPlayer.CurrentFrame / MaxFramesPlayed;
            UIManager.UpdateTimer(percent, MainPlayer.Depth, rewind);
        }

        private void ResetGame()
        {
            _timeObjects.Clear();
            _deathSource = null;

            MainPlayer = PlayerScene.Instance<Player>();
            MainPlayer.Connect(nameof(Player.OnFrameChanged), this, nameof(OnMainPlayerFrameChanged));

            Player = MainPlayer;

            UIManager.UpdateTimeScale(MainPlayer.TimeScale);
        }

        private void ResetPlayer()
        {
            if (Player != MainPlayer)
            {
                Unregister(Player);

                Player.Activate(false);
                Player.DeleteBullets();
                Player.QueueFree();

                Player = MainPlayer;
            }

            Player.Activate(true);
            Player.ToggleTimeIndicator(Colors.White);
            Player.UpdateAmmoCount();

            _deathSource.ToggleMarkSprite(false);
            _deathSource = null;
        }
    }
}