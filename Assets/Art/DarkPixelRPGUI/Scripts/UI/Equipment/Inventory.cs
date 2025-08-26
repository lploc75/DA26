using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkPixelRPGUI.Scripts.UI.Equipment
{
    public class Inventory : MonoBehaviour
    {
        private List<InventorySlot> _slots;

        private void Start()
        {
            _slots = GetComponentsInChildren<InventorySlot>().ToList();
            if (_slots.Count == 0)
            {
                Debug.LogError("No InventorySlots found in Inventory!");
            }
            else
            {
                foreach (var slot in _slots)
                {
                    if (slot.Item != null || slot.ItemIcon.sprite != null)
                    {
                        slot.ShowItem();
                        Debug.Log($"Initialized slot {slot.gameObject.name} with item or sprite");
                    }
                }
            }
        }

        public bool HasFreeSpace()
        {
            return _slots.Any(slot => slot.IsEmpty());
        }

        public InventorySlot GetNextEmptySlotForItem()
        {
            return _slots.Find(s => s.IsEmpty());
        }

        // Phương thức mới để thêm item runtime
        public void AddItem(Item newItem)
        {
            if (!HasFreeSpace())
            {
                Debug.LogWarning("No free space in inventory!");
                return;
            }

            var emptySlot = GetNextEmptySlotForItem();
            if (emptySlot != null)
            {
                emptySlot.PlaceItem(newItem);
                emptySlot.ResetVisual(); // Đảm bảo visual update để kéo thả được
                Debug.Log($"Added new item to slot: {emptySlot.gameObject.name}");
            }
        }

        public void RemoveBlanks()
        {
            var items = _slots.Where(s => !s.IsEmpty() && s.Item != null).Select(s => s.Item).ToList();
            for (var i = 0; i < _slots.Count; i++)
            {
                if (i < items.Count)
                {
                    _slots[i].PlaceItem(items[i]);
                }
                else
                {
                    _slots[i].ClearSlot();
                }
            }
            Debug.Log($"RemoveBlanks: Kept {items.Count} items, cleared remaining slots.");
        }

        public void SwapItems(int draggedSlotIndex, int targetSlotIndex)
        {
            var draggedItem = _slots[draggedSlotIndex].Item;
            var targetItem = _slots[targetSlotIndex].Item;

            if (draggedItem != null && targetItem != null)
            {
                _slots[draggedSlotIndex].PlaceItem(targetItem);
                _slots[targetSlotIndex].PlaceItem(draggedItem);
            }
            else
            {
                Debug.LogWarning("One of the slots is empty, cannot swap.");
            }
        }
    }
}