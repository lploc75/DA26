using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkPixelRPGUI.Scripts.UI.Equipment
{
    /// <summary>
    /// Khi con trỏ (vật phẩm đang kéo) đi vào khu vực InventorySlotItemDropTarget
    /// thì nó sẽ tìm slot trống tiếp theo trong inventory và set placeholder.
    /// </summary>
    public class InventorySlotItemDropTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Inventory inventory; // tham chiếu tới inventory chính
        private Slot _placeholdedSlot; // slot đang được hiển thị placeholder

        public void OnPointerEnter(PointerEventData eventData)
        {
            var dragItemHolder = DragItemHolder.Instance;
            // Chỉ chạy khi đang kéo và còn slot trống
            if (!dragItemHolder.dragging || !inventory.HasFreeSpace()) return;


            _placeholdedSlot = inventory.GetNextEmptySlotForItem();
            dragItemHolder.TargetSlotToDrop(_placeholdedSlot);
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            if (_placeholdedSlot == null) return;
            DragItemHolder.Instance.RemoveTarget(_placeholdedSlot);
            _placeholdedSlot = null;
        }
    }
}