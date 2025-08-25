using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkPixelRPGUI.Scripts.UI.Equipment
{
    public class Inventory : MonoBehaviour
    {
        private List<Slot> _slots;

        private void Start()
        {
            _slots = GetComponentsInChildren<Slot>().ToList();
        }

        public bool HasFreeSpace()
        {
            return _slots.Any(slot => slot.IsEmpty());
        }

        public Slot GetNextEmptySlotForItem()
        {
            return _slots.Find(s => s.IsEmpty());
        }

        public void RemoveBlanks()
        {
            var items = _slots.Where(s => !s.IsEmpty()).Select(s => s.Item).ToList();
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
        }

        public void SwapItems(int draggedSlotIndex, int targetSlotIndex)
        {
            // Lấy vật phẩm từ ô nguồn (dragged slot) và ô đích (target slot)
            var draggedItem = _slots[draggedSlotIndex].Item;
            var targetItem = _slots[targetSlotIndex].Item;

            // Đảm bảo rằng ô nguồn và ô đích không phải là trống
            if (draggedItem != null && targetItem != null)
            {
                // Swap vật phẩm giữa hai ô
                _slots[draggedSlotIndex].PlaceItem(targetItem);  // Đặt vật phẩm từ ô đích vào ô nguồn
                _slots[targetSlotIndex].PlaceItem(draggedItem);  // Đặt vật phẩm từ ô nguồn vào ô đích
            }
            else
            {
                Debug.LogWarning("One of the slots is empty, cannot swap.");
            }
        }

    }
}
