using Godot;
using Ludum48.Core.Managers;

namespace Ludum48.Core
{
    public class Level : Node2D
    {
        public override void _Ready()
        {
        }

        public void RemovePlayer()
        {
            RemoveChild(GameManager.Instance.Player);
        }

        public void SpawnPlayer()
        {
            AddChild(GameManager.Instance.Player);
        }

        private void OnLevelCleared()
        {
        }
    }
}