using Godot;

namespace GlobalGameJam2021.Core.Managers
{
    public class GameManager
    {
        static GameManager()
        {
            OS.CenterWindow();
        }

        private GameManager()
        {
        }

        public static GameManager Instance { get; } = new GameManager();
    }
}