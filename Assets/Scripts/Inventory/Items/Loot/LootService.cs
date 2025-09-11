// Scripts/Items/LootService.cs
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Database;            // DatabaseManager / ItemDB
// Các type bạn đã có sẵn:
// - Rarity, RarityConfig, RarityStat
// - Stats
// - ItemDefinition (Equipment)
// - AreaDropTable (Equipment)
// - AreaDropTable_Consumable / AreaDropTable_Material (bước 8)
// - ConsumableDefinition / MaterialDefinition (bước 3/4)
// - Scripts.Inventory.Item, Scripts.Inventory.AffixEntry

public class LootService : MonoBehaviour
{
    [Header("Config")]
    public RarityConfig rarityConfig;

    [Tooltip("Bảng drop trang bị (giữ nguyên hệ cũ)")]
    public AreaDropTable[] areaTables;

    [Tooltip("Bảng drop đồ tiêu hao (mới)")]
    public AreaDropTable_Consumable[] consumableTables;

    [Tooltip("Bảng drop nguyên liệu (mới)")]
    public AreaDropTable_Material[] materialTables;

    System.Random rng;

    void Awake()
    {
        rng = new System.Random(Environment.TickCount);
    }

    // ============================================================
    // 1) DROP TRANG BỊ (giữ nguyên flow cũ + lưu DB)
    // ============================================================
    public Scripts.Inventory.Item GenerateOneItemByRegion(int regionId, int playerLevel)
    {
        var tab = Array.Find(areaTables, t => t && t.RegionId == regionId);
        if (!tab) { Debug.LogWarning($"[Loot] No AreaDropTable for region {regionId}"); return null; }

        var def = tab.PickItem(rng);
        if (!def) { Debug.LogWarning("[Loot] PickItem returned null"); return null; }

        // --- Level theo rule của Area ---
        int lvl;
        if (playerLevel >= tab.ItemLevelRange.Min && playerLevel <= tab.ItemLevelRange.Max)
            lvl = tab.RollItemLevel(rng, playerLevel);
        else if (playerLevel < tab.ItemLevelRange.Min)
            lvl = tab.ItemLevelRange.Min;
        else
            lvl = tab.ItemLevelRange.Max;

        var rar = tab.RollRarity(rng);

        // Base stats (đã scale theo level)
        var baseStats = def.GetStatsAtLevel(lvl);

        // Roll affix theo rarity (N dòng, không lặp)
        var affixes = RollRarityAffixes(rar, rarityConfig, rng);

        // Cộng affix vào stat cuối
        var finalStats = ApplyAffixes(baseStats, affixes);

        // Giá bán
        int price = (rarityConfig != null)
            ? rarityConfig.ComputeSellPrice(def.BaseGoldValue, lvl, rar)
            : def.BaseGoldValue;

        var icon = def.Icon ? def.Icon : def.TypeIcon;

        var uiItem = new Scripts.Inventory.Item(
            icon, def.Id, lvl, rar, finalStats, price, def, affixes
        );

        Debug.Log($"[DROP] {def.DisplayName} | Lv {lvl} | {rar} | Affixes {affixes.Count} | Price {price}");
                 //Debug.Log("Call save");
        //DatabaseManager.Instance.itemDB.SaveItem(uiItem);
        return uiItem;     

    }

    // ============================================================
    // 2) DROP CONSUMABLE (mới) - CHƯA LƯU DB
    // ============================================================
    public Scripts.Inventory.Item GenerateConsumableByRegion(int regionId)
    {
        var tab = consumableTables?.FirstOrDefault(t => t && t.RegionId == regionId);
        if (!tab) { Debug.LogWarning($"[Loot] No consumable table for Region {regionId}"); return null; }

        var pick = tab.PickItem(rng);
        if (!pick) return null;

        int price = Mathf.Max(0, pick.BaseGoldValue);
        var icon = pick.Icon;
        var uiItem = new Scripts.Inventory.Item(
            sprite: icon,
            defId: pick.Id,
            itemLevel: 1,
            rarity: Rarity.Common,
            scaledStats: default,
            sellPrice: price,
            cachedDef: null,
            affixes: null
        );


        // KHÔNG lưu DB ở bước này để tránh vỡ LoadItems() (đang chỉ reconstruct Equipment)
        Debug.Log($"[Loot] Picked consumable: {pick.DisplayName} | Id={pick.Id} | Icon={(pick.Icon ? pick.Icon.name : "NULL")} | Price={price}");
        return uiItem;
    }

    // ============================================================
    // 3) DROP MATERIAL (mới) - CHƯA LƯU DB
    // ============================================================
    public Scripts.Inventory.Item GenerateMaterialByRegion(int regionId)
    {
        var tab = materialTables?.FirstOrDefault(t => t && t.RegionId == regionId);
        if (!tab) { Debug.LogWarning($"[Loot] No material table for Region {regionId}"); return null; }

        var pick = tab.PickItem(rng);
        if (!pick) return null;

        int price = Mathf.Max(0, pick.BaseGoldValue);
        var icon = pick.Icon;

        var uiItem = new Scripts.Inventory.Item(
         sprite: icon,
         defId: pick.Id,
         itemLevel: 1,
         rarity: Rarity.Common,
         scaledStats: default,
         sellPrice: price,
         cachedDef: null,
         affixes: null
     );

        // KHÔNG lưu DB ở bước này
        Debug.Log($"[DROP] Material: {pick.DisplayName} | Price {price}");
        return uiItem;
    }

    // ============================================================
    // Helpers
    // ============================================================
    static List<Scripts.Inventory.AffixEntry> RollRarityAffixes(Rarity r, RarityConfig rc, System.Random rng)
    {
        var result = new List<Scripts.Inventory.AffixEntry>();
        if (rc == null) return result;

        int need = Mathf.Max(0, rc.GetLineCount(r)); // Uncommon=1, Rare=2, Epic=3, Legendary=4

        // Pool các stat có giá trị > 0 ở tier hiện tại
        var pool = new List<RarityStat>();
        foreach (var e in rc.BonusTable)
        {
            float v = rc.GetBonusValue(e.Stat, r);
            if (!Mathf.Approximately(v, 0f))
                pool.Add(e.Stat);
        }

        rng ??= new System.Random();

        // Rút thăm không lặp
        for (int i = 0; i < need && pool.Count > 0; i++)
        {
            int idx = rng.Next(0, pool.Count);
            var stat = pool[idx];
            pool.RemoveAt(idx);

            float v = rc.GetBonusValue(stat, r);
            result.Add(new Scripts.Inventory.AffixEntry { Stat = stat, Value = v });
        }
        return result;
    }

    static Stats ApplyAffixes(Stats s, List<Scripts.Inventory.AffixEntry> affixes)
    {
        foreach (var a in affixes)
        {
            switch (a.Stat)
            {
                // Attributes
                case RarityStat.Strength: s.Strength += Mathf.RoundToInt(a.Value); break;
                case RarityStat.Agility: s.Agility += Mathf.RoundToInt(a.Value); break;
                case RarityStat.Intelligence: s.Intelligence += Mathf.RoundToInt(a.Value); break;
                case RarityStat.Vitality: s.Vitality += Mathf.RoundToInt(a.Value); break;
                case RarityStat.Wisdom: s.Wisdom += Mathf.RoundToInt(a.Value); break;

                // Resources
                case RarityStat.Health: s.MaxHealth += Mathf.RoundToInt(a.Value); break;
                case RarityStat.Mana: s.MaxMana += Mathf.RoundToInt(a.Value); break;

                // Combat
                case RarityStat.Armor: s.Armor += Mathf.RoundToInt(a.Value); break;
                case RarityStat.BaseDamage: s.BaseDamage += Mathf.RoundToInt(a.Value); break;
                case RarityStat.CritChance: s.CritChance += a.Value; break;         // %
                case RarityStat.CritDamagePct: s.CritDamagePct += a.Value; break;   // %

                // Regens
                case RarityStat.HealthRegenPerSec: s.HealthRegenPerSec += a.Value; break; 
                case RarityStat.ManaRegenPerSec: s.ManaRegenPerSec += a.Value; break;
            }
        }

        s.CritChance = Mathf.Clamp(s.CritChance, 0f, 100f);
        return s;
    }
}
