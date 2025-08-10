using UnityEngine;
using System.Linq;

public class InventoryInitializer : MonoBehaviour
{
    public Transform inventoryPanel; // gán transform của InventoryPanel
    public Sprite woodSwordIcon, leatherArmorIcon, capIcon, amuletIcon;
    private ItemSlotUI[] inventorySlots;

    void Awake()
    {
        // Lấy tất cả slot có slotType=None trong InventoryPanel
        inventorySlots = inventoryPanel.GetComponentsInChildren<ItemSlotUI>(true)
                                       .Where(s => s.slotType == SlotType.None)
                                       .ToArray();
    }

    void Start()
    {
        var sword = new ItemTest("Wood Sword", woodSwordIcon, SlotType.Weapon);
        var armor = new ItemTest("Leather Armor", leatherArmorIcon, SlotType.Armor);
        var cap = new ItemTest("Travel Cap", capIcon, SlotType.Headgear);
        var amulet = new ItemTest("Old Amulet", amuletIcon, SlotType.Amulet);

        if (inventorySlots.Length > 0) inventorySlots[0].SetItem(sword);
        if (inventorySlots.Length > 1) inventorySlots[1].SetItem(armor);
        if (inventorySlots.Length > 2) inventorySlots[2].SetItem(cap);
        if (inventorySlots.Length > 3) inventorySlots[3].SetItem(amulet);
    }
}
