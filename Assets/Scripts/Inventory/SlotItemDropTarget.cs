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
            var dragItemHolder = DragItemHolder.Instance;
            // Chỉ placeholder khi đang kéo và slot trống
            if (dragItemHolder.dragging && slot.IsEmpty())
            {
                dragItemHolder.TargetSlotToDrop(slot);
            }
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            DragItemHolder.Instance.RemoveTarget(slot);
        }
    }
}