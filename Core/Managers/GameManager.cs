namespace GlobalGameJam2021.Core.Managers
{
    public class GameManager
    {
        static GameManager()
        {
        }

        private GameManager()
        {
        }

        public static GameManager Instance { get; } = new GameManager();
    }
}