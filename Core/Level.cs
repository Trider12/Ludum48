using Godot;
using Ludum48.Core.Managers;

namespace Ludum48.Core
{
    public class Level : Node2D
    {
        private YSort _ySort;

        public Navigation2D Navigation2D { get; private set; }

        public override void _Ready()
        {
            Navigation2D = GetNode<Navigation2D>("Navigation2D");
            _ySort = GetNode<YSort>("YSort");
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

        public void SpawnPlayer()
        {
            _ySort.AddChild(GameManager.Instance.Player);
        }

        private void OnLevelCleared()
        {
        }
    }
}