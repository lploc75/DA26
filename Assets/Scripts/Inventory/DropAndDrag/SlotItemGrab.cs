using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Inventory
{
    /// <summary>
    /// Xử lý khi bắt đầu kéo item từ một Slot cụ thể (hỗ trợ cả inventory và equip).
    /// </summary>
    public class SlotItemGrab : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private Slot slot; // Slot gắn kèm script này

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (slot.IsEmpty()) return; // không cho kéo nếu slot trống
            DragItemHolder.Instance.StartDrag(slot);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (DragItemHolder.Instance == null || !DragItemHolder.Instance.dragging)
            {
                Debug.LogError("DragItemHolder null or not dragging!");
                return;
            }

            var draggedSlot = DragItemHolder.Instance.GetDraggedSlot();
            var targetSlot = InventorySlotItemGrab.FindSlot(eventData.position, "InventorySlot") ?? InventorySlotItemGrab.FindSlot(eventData.position, "EquipmentSlot");

            if (targetSlot == null || targetSlot == draggedSlot)
            {
                draggedSlot.ResetVisual();
                DragItemHolder.Instance.DropItem();
                return;
            }

            targetSlot.RemovePlaceholder();

            // Nếu drag từ equip và drop vào inventory slot trống
            if (draggedSlot is EquipmentSlot && targetSlot is InventorySlot inventoryTarget && inventoryTarget.IsEmpty())
            {
                var dragItem = DragItemHolder.Instance.dragItem;
                inventoryTarget.PlaceItem(dragItem);
                draggedSlot.ClearSlot();
                draggedSlot.ResetVisual();
                inventoryTarget.ResetVisual();
                DragItemHolder.Instance.DropItem();
                // Compact inventory nếu cần
                var inventory = FindObjectOfType<Inventory>(); // Giả sử có Inventory component
                if (inventory != null) inventory.RemoveBlanks();
                return;
            }

            // Giữ nguyên logic khác (equip hoặc swap inventory) từ InventorySlotItemGrab.cs
            // ... (copy logic từ OnEndDrag của InventorySlotItemGrab.cs vào đây nếu cần hợp nhất)

            draggedSlot.ResetVisual();
            targetSlot.ResetVisual();
            DragItemHolder.Instance.DropItem();
        }

        public void OnDrag(PointerEventData eventData)
        {
            // blank implementation
        }
    }
}