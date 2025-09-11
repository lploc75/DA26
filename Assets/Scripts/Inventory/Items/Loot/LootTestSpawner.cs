// Scripts/Items/LootTestSpawner.cs
using UnityEngine;
using Scripts.Inventory;

public class LootTestSpawner : MonoBehaviour
{
    [Header("Refs")]
    public LootService lootService; // Kéo từ Systems
    public Inventory inventory;     // Kéo chính GameObject BagSlots (có Inventory)

    [Header("Test params")]
    public int currentRegionId = 1;
    public int playerLevel = 3;

    [Header("Keys")]
    public KeyCode triggerEquipKey = KeyCode.I;       // rơi Equipment (và lưu DB)
    public KeyCode triggerConsumableKey = KeyCode.O;  // rơi Consumable (không lưu)
    public KeyCode triggerMaterialKey = KeyCode.P;    // rơi Material (không lưu)

    void Update()
    {
        if (!lootService || !inventory) return;

        // Equipment
        if (Input.GetKeyDown(triggerEquipKey))
        {
            Debug.Log("▶️ Pressed I → Generate Equipment");
            var item = lootService.GenerateOneItemByRegion(currentRegionId, playerLevel);
            if (item != null)
            {
                Debug.Log($"[Spawner] Equip generated: {item.DefId} | Sprite={item.Sprite}");
                inventory.AddItem(item);
            }
            else Debug.LogWarning("[Spawner] Equip drop returned null");
        }

        // Consumable
        if (Input.GetKeyDown(triggerConsumableKey))
        {
            Debug.Log("▶️ Pressed O → Generate Consumable");
            var item = lootService.GenerateConsumableByRegion(currentRegionId);
            if (item != null)
            {
                Debug.Log($"[Spawner] Consumable generated: {item.DefId} | Sprite={item.Sprite}");
                if (item.Sprite == null)
                    Debug.LogWarning("[Spawner] ❌ Consumable Sprite is NULL");
                inventory.AddItem(item);
            }
            else Debug.LogWarning("[Spawner] Consumable drop returned null");
        }

        // Material
        if (Input.GetKeyDown(triggerMaterialKey))
        {
            Debug.Log("▶️ Pressed P → Generate Material");
            var item = lootService.GenerateMaterialByRegion(currentRegionId);
            if (item != null)
            {
                Debug.Log($"[Spawner] Material generated: {item.DefId} | Sprite={item.Sprite}");
                if (item.Sprite == null)
                    Debug.LogWarning("[Spawner] ❌ Material Sprite is NULL");
                inventory.AddItem(item);
            }
            else Debug.LogWarning("[Spawner] Material drop returned null");
        }
    }
}
