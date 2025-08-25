using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkPixelRPGUI.Scripts.UI.Equipment
{
    public class InventorySlotItemGrab : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private const string InventorySlot = "InventorySlot";
        [SerializeField] private Inventory inventory;
        private List<Slot> _slots;

        private void Start()
        {
            _slots = inventory.GetComponentsInChildren<Slot>().ToList();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            var inventorySlot = FindSlot(eventData.position);
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

            // Khai báo draggedSlot sớm, trước khi sử dụng
            var draggedSlot = DragItemHolder.Instance.GetDraggedSlot();

            var targetSlot = FindSlot(eventData.position);
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

            // Nếu drop lên source, cancel
            if (targetSlot == draggedSlot)
            {
                draggedSlot.ResetVisual();
                DragItemHolder.Instance.DropItem();
                return;
            }

            // Remove placeholder nếu có
            targetSlot.RemovePlaceholder();

            if (targetSlot.IsEmpty())
            {
                Debug.Log("Target Slot is empty. Moving item.");
                targetSlot.PlaceItem(DragItemHolder.Instance.dragItem);
                draggedSlot.ClearSlot();
            }
            else
            {
                Debug.Log("Target Slot is not empty. Proceeding with SwapItems.");
                var tempItem = targetSlot.Item;
                targetSlot.PlaceItem(DragItemHolder.Instance.dragItem);
                draggedSlot.PlaceItem(tempItem);
            }

            // Reset visual source và target
            draggedSlot.ResetVisual();
            targetSlot.ResetVisual();

            DragItemHolder.Instance.DropItem();

            inventory.RemoveBlanks();
        }

        public void OnDrag(PointerEventData eventData)
        {
            // blank implementation
        }

        private static Slot FindSlot(Vector3 mousePosition)
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            var slotGo = results.Select(r => r.gameObject).FirstOrDefault(go => go.CompareTag(InventorySlot));
            return slotGo == null ? null : slotGo.GetComponentInParent<Slot>();
        }
    }
}