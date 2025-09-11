using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Inventory
{

    /// <summary>
    /// Khi con trỏ (drag item) đi vào/ra một Slot cụ thể.
    /// Nếu slot đang trống -> hiển thị placeholder cho item đang kéo.
    /// </summary>
    public class SlotItemDropTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Slot slot;


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slot == null)
            {
                Debug.LogError("Slot chưa gán trong SlotItemDropTarget!", this);
                return;
            }

            var dragItemHolder = DragItemHolder.Instance;
            if (dragItemHolder == null || !dragItemHolder.dragging || !slot.IsEmpty())
                return;

            if (slot is EquipmentSlot equipSlot && (dragItemHolder.dragItem?.CachedDef.Type != equipSlot.AllowedType))
            {
                return; // Không placeholder nếu type không khớp
            }
            dragItemHolder.TargetSlotToDrop(slot);
        }



        public void OnPointerExit(PointerEventData eventData)
        {
            DragItemHolder.Instance.RemoveTarget(slot);
        }
    }
}