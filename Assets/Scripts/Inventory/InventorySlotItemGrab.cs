using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkPixelRPGUI.Scripts.UI.Equipment
{
    /// <summary>
    /// Xử lý tương tác kéo/thả từ các ô InventorySlot (raycast theo tag).
    /// </summary>
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

            if (targetSlot.IsEmpty())
            {
                // Cấm thả vào ô trống
                draggedSlot.ResetVisual();
                DragItemHolder.Instance.DropItem();
                return;
            }

            else if (targetSlot is InventorySlot)
            {
                // SWAP: chỉ cho swap giữa các inventory slot
                Debug.Log("Target Slot is not empty. Proceeding with SwapItems.");
                var tempItem = targetSlot.Item;
                targetSlot.PlaceItem(DragItemHolder.Instance.dragItem);
                draggedSlot.PlaceItem(tempItem);
            }
            else
            {
                // Không swap với EquipmentSlot đang có item
                Debug.Log("Cannot swap with EquipmentSlot.");
                draggedSlot.ResetVisual();
                DragItemHolder.Instance.DropItem();
                return;
            }

            draggedSlot.ResetVisual();
            targetSlot.ResetVisual();

            DragItemHolder.Instance.DropItem();

            //if (inventory != null)
            //{
            //    inventory.RemoveBlanks();
            //}
        }

        public void OnDrag(PointerEventData eventData)
        {
            // blank implementation
        }

        private static Slot FindSlot(Vector3 mousePosition, string tag)
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            var slotGo = results.Select(r => r.gameObject).FirstOrDefault(go => go.CompareTag(tag));
            return slotGo == null ? null : slotGo.GetComponentInParent<Slot>();
        }
    }
}