using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Inventory
{
    /// <summary>
    /// Ô item trong kho đồ. Hiện đang chỉ kế thừa Slot và log thêm.
    /// </summary>
    public class InventorySlot : Slot
    {
        // Kế thừa tất cả chức năng từ Slot
        // Có thể thêm logic riêng cho InventorySlot nếu cần
        public override void PlaceItem(Item itemToPlace)
        {
            base.PlaceItem(itemToPlace);
            Debug.Log($"Placed item in InventorySlot: {gameObject.name}");
        }

        public override void ClearSlot()
        {
            base.ClearSlot();           
        }
    }
}