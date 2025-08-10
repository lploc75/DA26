using UnityEngine;

[System.Serializable]
public class ItemTest
{
    public string itemName;
    public Sprite icon;
    public SlotType slotType;

    public ItemTest(string name, Sprite icon, SlotType slotType)
    {
        this.itemName = name;
        this.icon = icon;
        this.slotType = slotType;
    }
}
