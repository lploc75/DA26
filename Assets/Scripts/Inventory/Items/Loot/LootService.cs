// Scripts/Items/LootService.cs
using UnityEngine;
using System;
using System.Collections.Generic;

public class LootService : MonoBehaviour
{
    [Header("Config")]
    public RarityConfig rarityConfig;
    public AreaDropTable[] areaTables;

    System.Random rng;

    void Awake() => rng = new System.Random(Environment.TickCount);

    public Scripts.Inventory.Item GenerateOneItemByRegion(int regionId, int playerLevel)
    {
        var tab = Array.Find(areaTables, t => t.RegionId == regionId);
        if (tab == null) { Debug.LogWarning($"[Loot] No AreaDropTable for region {regionId}"); return null; }

        var def = tab.PickItem(rng);
        if (def == null) { Debug.LogWarning("[Loot] PickItem returned null"); return null; }

        // Level theo rule của Area
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

        // ✅ Roll affix theo rarity (N dòng, không lặp)
        var affixes = RollRarityAffixes(rar, rarityConfig, rng);

        // ✅ Cộng affix vào stat cuối cùng
        var finalStats = ApplyAffixes(baseStats, affixes);

        // Giá bán
        int price = rarityConfig != null
            ? rarityConfig.ComputeSellPrice(def.BaseGoldValue, lvl, rar)
            : def.BaseGoldValue;

        var icon = def.Icon ? def.Icon : def.TypeIcon;

        var uiItem = new Scripts.Inventory.Item(
            icon, def.Id, lvl, rar, finalStats, price, def, affixes
        );

        Debug.Log($"[DROP] {def.DisplayName} | Lv {lvl} | {rar} | Affixes {affixes.Count} | Price {price}");
        return uiItem;
    }

    // --- Helpers ---

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
                case RarityStat.CritChance: s.CritChance += a.Value; break; // %
                case RarityStat.CritDamagePct: s.CritDamagePct += a.Value; break; // %

                // Regens
                case RarityStat.HealthRegenPerSec: s.HealthRegenPerSec += a.Value; break;
                case RarityStat.ManaRegenPerSec: s.ManaRegenPerSec += a.Value; break;
            }
        }

        s.CritChance = Mathf.Clamp(s.CritChance, 0f, 100f);
        return s;
    }
}
