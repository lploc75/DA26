using UnityEngine;
using Scripts.Inventory;

public class LootTestSpawner : MonoBehaviour
{
    [Header("Refs")]
    public LootService lootService; // kéo từ Systems
    public Inventory inventory;     // KÉO CHÍNH GameObject BagSlots (có Inventory)

    [Header("Test params")]
    public int currentRegionId = 1;
    public int playerLevel = 3;
    public KeyCode triggerKey = KeyCode.I;

    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            if (!lootService || !inventory) { Debug.LogWarning("Missing refs"); return; }
            var item = lootService.GenerateOneItemByRegion(currentRegionId, playerLevel);
            if (item != null)
            {   
                inventory.AddItem(item);
                Debug.Log($"[TEST] + {item.DefId} Lv{item.ItemLevel} ({item.Rarity}) into inventory");
            }
        }
    }
}
