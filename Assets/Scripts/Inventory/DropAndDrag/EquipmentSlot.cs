using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Inventory
{
    public class EquipmentSlot : Slot
    {
        [SerializeField] private ItemType allowedType; // Loại item được phép (Weapon, Helmet, v.v.)

        public ItemType AllowedType => allowedType;

        public override void PlaceItem(Item itemToPlace)
        {
            if (itemToPlace != null && itemToPlace.CachedDef.Type != allowedType)
            {
                Debug.LogWarning($"Cannot place {itemToPlace.CachedDef.Type} in {allowedType} slot: {gameObject.name}");
                return;
            }
            base.PlaceItem(itemToPlace);
            Debug.Log($"Placed item in EquipmentSlot: {gameObject.name}");
        }

        public override void ClearSlot()
        {
            base.ClearSlot();
        }
    }
}