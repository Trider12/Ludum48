namespace GlobalGameJam2021.Core.Managers
{
    public class GameManager
    {
        private GameManager()
        {
        }

        public static GameManager Instance { get; } = new GameManager();

        public InventoryManager InventoryManager { get; set; }
        public UIManager UIManager { get; set; }
        public WaypointManager WaypointManager { get; set; }
    }
}