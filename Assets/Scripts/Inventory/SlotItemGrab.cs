using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkPixelRPGUI.Scripts.UI.Equipment
{
    /// <summary>
    /// Xử lý khi bắt đầu kéo item từ một Slot cụ thể.
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
            DragItemHolder.Instance.DropItem();
        }


        public void OnDrag(PointerEventData eventData)
        {
            // bắt buộc implement do interface nhưng không cần xử lý
        }
    }
}