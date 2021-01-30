using GlobalGameJam2021.Core.Managers;
using Godot;

namespace GlobalGameJam2021.Core
{
    public class Item : MovableEntity
    {
        private Sprite _sprite;
        private bool _taken = false;
        public bool IsHolding { get; set; } = false;

        public override void _Input(InputEvent @event)
        {
            if (IsPhysicsProcessing())
            {
                return;
            }

            if (@event is InputEventMouseButton eventKey)
            {
                if (eventKey.ButtonIndex == (int)ButtonList.Left && !eventKey.IsEcho())
                {
                    if (_sprite.GetRect().HasPoint(ToLocal(eventKey.Position)))
                    {
                        if (eventKey.Pressed)
                        {
                            if (!_taken)
                            {
                                _taken = GameManager.Instance.InventoryManager.AddItemToInventory(this);
                            }
                            else if (!IsHolding)
                            {
                                GameManager.Instance.InventoryManager.TakeItemFromInventory(this);
                            }
                        }
                        else if (IsHolding)
                        {
                            GameManager.Instance.InventoryManager.ReturnItemToInventory();
                        }
                    }
                }
            }
        }

        public override void _Ready()
        {
            base._Ready();

            _sprite = GetNode<Sprite>("Sprite");
        }
    }
}