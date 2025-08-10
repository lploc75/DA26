using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;             // Ảnh icon hiển thị trong slot

    [Header("Config")]
    public SlotType slotType = SlotType.None; // Inventory: None, Equipment: Weapon/Armor/...

    [Header("State")]
    public ItemTest currentItem;        // Item đang chứa

    public bool IsEmpty => currentItem == null;

    void Awake()
    {
        Refresh();
    }

    public void SetItem(ItemTest item)
    {
        currentItem = item;
        Refresh();
    }

    public void ClearSlot()
    {
        currentItem = null;
        Refresh();
    }

    void Refresh()
    {
        if (iconImage == null) return;

        if (currentItem != null && currentItem.icon != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = currentItem.icon;
        }
        else
        {
            iconImage.enabled = false;
            iconImage.sprite = null;
        }
    }
}
