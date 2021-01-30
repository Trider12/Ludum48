using System.Collections.Generic;
using Godot;

namespace GlobalGameJam2021.Core.Managers
{
    public class InventoryManager : Control
    {
        private List<ItemSlot> _itemSlots = new List<ItemSlot>();

        public InventoryManager()
        {
            GameManager.Instance.InventoryManager = this;
        }

        public Item HoldingItem { get; private set; } = null;

        public override void _GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton eventKey)
            {
                if (eventKey.Pressed && eventKey.ButtonIndex == (int)ButtonList.Left && !eventKey.IsEcho())
                {
                    if (HoldingItem != null)
                    {
                        ReturnHoldingItemToInventory();
                    }
                }
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (HoldingItem != null)
            {
                HoldingItem.GlobalPosition = GetGlobalMousePosition();
            }
        }

        public override void _Ready()
        {
            foreach (var child in GetChildren())
            {
                if (child is ItemSlot)
                {
                    var slot = child as ItemSlot;
                    _itemSlots.Add(slot);
                }
            }
        }

        public bool AddItemToInventory(Item item)
        {
            if (HoldingItem != null)
            {
                if (HoldingItem == item)
                {
                    ReturnHoldingItemToInventory();
                }

                return false;
            }

            foreach (var slot in _itemSlots)
            {
                if (slot.Item == null)
                {
                    slot.Item = item;
                    item.GetParent().RemoveChild(item);
                    slot.AddChild(item);
                    item.Position = slot.RectSize / 2;

                    return true;
                }
            }

            return false;
        }

        public void ReturnHoldingItemToInventory()
        {
            if (HoldingItem == null)
            {
                return;
            }

            foreach (var slot in _itemSlots)
            {
                if (slot.Item == HoldingItem)
                {
                    HoldingItem.Position = slot.RectSize / 2;
                    HoldingItem.IsHolding = false;
                    HoldingItem = null;

                    return;
                }
            }
        }

        public void TakeItemFromInventory(Item item)
        {
            HoldingItem = item;
            GetTree().SetInputAsHandled();
        }
    }
}