using Godot;
using Ludum48.Core.Managers;

namespace Ludum48.Core
{
    public class Level : Node2D
    {
        private Vector2 _playerSpawnPosition;
        private YSort _ySort;
        public Navigation2D Navigation2D { get; private set; }

        public override void _Ready()
        {
            Navigation2D = GetNode<Navigation2D>("Navigation2D");
            _ySort = GetNode<YSort>("YSort");
            _playerSpawnPosition = GetNode<Node2D>("PlayerSpawn").GlobalPosition;
        }

        public void RemoveEntity(Entity entity)
        {
            _ySort.RemoveChild(entity);
        }

        public void RemovePlayer()
        {
            _ySort.RemoveChild(GameManager.Instance.Player);
        }

        public void SpawnEntity(Entity entity)
        {
            _ySort.AddChild(entity);
        }

        public void SpawnPlayer(bool atSpawn = false)
        {
            _ySort.AddChild(GameManager.Instance.Player);

            if (atSpawn)
            {
                GameManager.Instance.Player.GlobalPosition = _playerSpawnPosition;
            }
        }

        private void OnLevelCleared()
        {
        }
    }
}