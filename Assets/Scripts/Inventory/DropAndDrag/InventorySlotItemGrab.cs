using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Inventory
{
    public class InventorySlotItemGrab : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private const string InventorySlotTag = "InventorySlot";
        private const string EquipmentSlotTag = "EquipmentSlot";
        [SerializeField] private Inventory inventory;
        private List<InventorySlot> _slots;

        private void Start()
        {
            _slots = inventory.GetComponentsInChildren<InventorySlot>().ToList();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var inventorySlot = FindSlot(eventData.position, InventorySlotTag);
            if (inventorySlot == null || inventorySlot.IsEmpty()) return;

            Debug.Log("Starting drag for slot: " + inventorySlot.name);

            DragItemHolder.Instance.StartDrag(inventorySlot);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("OnEndDrag called");

            if (DragItemHolder.Instance == null)
            {
                Debug.LogError("DragItemHolder.Instance is null!");
                return;
            }

            var draggedSlot = DragItemHolder.Instance.GetDraggedSlot();
            var targetSlot = FindSlot(eventData.position, InventorySlotTag) ?? FindSlot(eventData.position, EquipmentSlotTag);

            if (targetSlot == null)
            {
                Debug.LogError("Target slot is null.");
                if (draggedSlot != null) draggedSlot.ResetVisual();
                DragItemHolder.Instance.DropItem();
                return;
            }

            Debug.Log("Target Slot: " + targetSlot.name);

            if (draggedSlot == null)
            {
                Debug.LogError("Dragged Slot is null!");
                DragItemHolder.Instance.DropItem();
                return;
            }

            if (targetSlot == draggedSlot)
            {
                draggedSlot.ResetVisual();
                DragItemHolder.Instance.DropItem();
                return;
            }

            targetSlot.RemovePlaceholder();

            // Xử lý drop vào EquipmentSlot
            if (targetSlot is EquipmentSlot equipTarget)
            {
                var dragItem = DragItemHolder.Instance.dragItem;
                if (dragItem.CachedDef.Type != equipTarget.AllowedType)
                {
                    Debug.LogWarning("Item type does not match EquipmentSlot type.");
                    draggedSlot.ResetVisual();
                    DragItemHolder.Instance.DropItem();
                    return;
                }

                if (equipTarget.IsEmpty())
                {
                    // Equip vào slot trống
                    equipTarget.PlaceItem(dragItem);
                    draggedSlot.ClearSlot();
                }
                else
                {
                    // Swap: unequip item cũ về inventory slot trống
                    if (!inventory.HasFreeSpace())
                    {
                        Debug.LogWarning("No free space in inventory for unequip!");
                        draggedSlot.ResetVisual();
                        DragItemHolder.Instance.DropItem();
                        return;
                    }
                    var unequipItem = equipTarget.Item;
                    var emptyInventorySlot = inventory.GetNextEmptySlotForItem();
                    emptyInventorySlot.PlaceItem(unequipItem);
                    equipTarget.PlaceItem(dragItem);
                    draggedSlot.ClearSlot();
                }
            }
            else if (targetSlot is InventorySlot)
            {
                // Logic swap inventory cũ
                if (!targetSlot.IsEmpty())
                {
                    Debug.Log("Target Slot is not empty. Proceeding with SwapItems.");
                    var tempItem = targetSlot.Item;
                    targetSlot.PlaceItem(DragItemHolder.Instance.dragItem);
                    draggedSlot.PlaceItem(tempItem);
                }
                else
                {
                    // Cấm thả vào ô trống inventory
                    draggedSlot.ResetVisual();
                    DragItemHolder.Instance.DropItem();
                    return;
                }
            }

            draggedSlot.ResetVisual();
            targetSlot.ResetVisual();

            DragItemHolder.Instance.DropItem();

            // Compact inventory sau drop
            inventory.RemoveBlanks();
        }

        public void OnDrag(PointerEventData eventData)
        {
            // blank implementation
        }

        public static Slot FindSlot(Vector3 mousePosition, string tag)
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // Debug để kiểm tra raycast
            if (results.Count == 0)
            {
                Debug.LogWarning("RaycastAll không tìm thấy object nào!");
            }
            else
            {
                foreach (var res in results)
                {
                    Debug.Log($"Raycast hit: {res.gameObject.name} (tag: {res.gameObject.tag})");
                }
            }

            var slotGo = results.FirstOrDefault(r => r.gameObject.CompareTag(tag)).gameObject;
            return slotGo != null ? slotGo.GetComponentInParent<Slot>() : null;
        }
    }
}