using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct IntRange { public int Min; public int Max; }

[Serializable]
public struct RarityDistribution
{
    [Range(0, 100)] public float Common;
    [Range(0, 100)] public float Uncommon;
    [Range(0, 100)] public float Rare;
    [Range(0, 100)] public float Epic;
    [Range(0, 100)] public float Legendary;

    public float Sum() => Common + Uncommon + Rare + Epic + Legendary;

    public Rarity RollRarity(System.Random rng)
    {
        double roll = rng.NextDouble() * 100.0;
        double acc = Common;
        if (roll < acc) return Rarity.Common;
        acc += Uncommon;
        if (roll < acc) return Rarity.Uncommon;
        acc += Rare;
        if (roll < acc) return Rarity.Rare;
        acc += Epic;
        if (roll < acc) return Rarity.Epic;
        return Rarity.Legendary;
    }
}

[CreateAssetMenu(fileName = "AreaDropTable", menuName = "Game/Loot/Area Drop Table")]
public class AreaDropTable : ScriptableObject
{
    [Header("Khu vực")]
    public int RegionId;          // 1..6
    public string RegionName;

    [Header("Level rơi trong khu (vd: 5–7)")]
    public IntRange ItemLevelRange;

    [Header("Tỉ lệ rarity trong khu (≈100%)")]
    public RarityDistribution RarityRates;

    [Header("Pool item rơi trong khu (map từ Excel 'at R#')")]
    public List<ItemDefinition> ItemPool = new List<ItemDefinition>();

    [Header("Quy tắc roll level quanh player")]
    [Range(0, 1)] public float ChanceSame = 0.70f;
    [Range(0, 1)] public float ChancePlusOne = 0.20f;
    [Range(0, 1)] public float ChanceMinusOne = 0.10f;

    public bool Validate(out string message)
    {
        if (ItemLevelRange.Min > ItemLevelRange.Max) { message = "ItemLevelRange Min > Max"; return false; }
        float sum = RarityRates.Sum();
        if (Mathf.Abs(sum - 100f) > 0.01f) { message = $"Rarity sum = {sum} (không ≈ 100)"; return false; }
        if (Mathf.Abs((ChanceSame + ChancePlusOne + ChanceMinusOne) - 1f) > 0.001f) { message = "ChanceSame/+1/-1 không cộng thành 1"; return false; }
        message = "OK"; return true;
    }

    public ItemDefinition PickItem(System.Random rng)
    {
        if (ItemPool == null || ItemPool.Count == 0) return null;
        int idx = rng.Next(0, ItemPool.Count);
        return ItemPool[idx];
    }

    public int RollItemLevel(System.Random rng, int playerLevel)
    {
        double r = rng.NextDouble();
        int lvl = playerLevel;
        if (r < ChanceSame) { }
        else if (r < ChanceSame + ChancePlusOne) lvl += 1;
        else lvl -= 1;

        return Mathf.Clamp(lvl, ItemLevelRange.Min, ItemLevelRange.Max);
    }

    public Rarity RollRarity(System.Random rng) => RarityRates.RollRarity(rng);
}
