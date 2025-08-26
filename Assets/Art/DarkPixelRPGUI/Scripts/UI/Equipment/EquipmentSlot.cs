using UnityEngine;
using UnityEngine.UI;

namespace DarkPixelRPGUI.Scripts.UI.Equipment
{
    public class EquipmentSlot : Slot
    {
        // Có thể thêm logic riêng cho EquipmentSlot, ví dụ: kiểm tra loại item phù hợp
        public override void PlaceItem(Item itemToPlace)
        {
            base.PlaceItem(itemToPlace);
            Debug.Log($"Placed item in EquipmentSlot: {gameObject.name}");
        }

        public override void ClearSlot()
        {
            //base.ClearSlot();
            Debug.Log($"Cleared EquipmentSlot: {gameObject.name}");
        }
    }
}