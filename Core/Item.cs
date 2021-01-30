using GlobalGameJam2021.Core.Managers;
using Godot;

namespace GlobalGameJam2021.Core
{
    public class Item : Node2D
    {
        private Sprite _sprite;
        private bool _taken = false;
        public bool IsHolding { get; set; } = false;

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventKey)
            {
                if (eventKey.Pressed && eventKey.ButtonIndex == (int)ButtonList.Left && !eventKey.IsEcho())
                {
                    if (_sprite.GetRect().HasPoint(ToLocal(eventKey.Position)))
                    {
                        if (!_taken)
                        {
                            _taken = GameManager.Instance.InventoryManager.AddItemToInventory(this);
                        }
                        else if (!IsHolding)
                        {
                            GameManager.Instance.InventoryManager.TakeItemFromInventory(this);
                            IsHolding = true;
                        }
                    }
                }
            }
        }

        public override void _Ready()
        {
            _sprite = GetNode<Sprite>("Sprite");
        }
    }
}