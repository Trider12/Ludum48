using System.Collections.Generic;
using Godot;

namespace GlobalGameJam2021.Core.Managers
{
    public class InventoryManager : Control
    {
        private List<ItemSlot> _itemSlots = new List<ItemSlot>();

        public Item HoldingItem { get; private set; } = null;

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

            GameManager.Instance.InventoryManager = this;
        }

        public bool AddItemToInventory(Item item)
        {
            if (HoldingItem != null)
            {
                // should never happen
                if (HoldingItem == item)
                {
                    ReturnItemToInventory();
                }

                return false;
            }

            foreach (var slot in _itemSlots)
            {
                if (slot.Item == null)
                {
                    var pos = item.GlobalPosition;

                    slot.Item = item;
                    item.GetParent().RemoveChild(item);
                    slot.AddChild(item);

                    item.GlobalPosition = pos;

                    item.MoveTo(slot.RectGlobalPosition + slot.RectSize / 2);

                    return true;
                }
            }

            return false;
        }

        public void ReturnItemToInventory()
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
            if (HoldingItem != null)
            {
                return;
            }

            HoldingItem = item;
            HoldingItem.IsHolding = true;
        }
    }
}