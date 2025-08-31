// Assets/Scripts/Items/RarityConfig.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }

// Các stat mà bảng rarity của bạn hỗ trợ
public enum RarityStat
{
    Strength, Agility, Intelligence, Vitality, Wisdom,
    Health, Mana, Armor, BaseDamage, CritChance, CritDamagePct,
    HealthRegenPerSec, ManaRegenPerSec
}

[Serializable]
public struct LinesPerRarity
{
    [Min(0)] public int Uncommon; // =1
    [Min(0)] public int Rare;     // =2
    [Min(0)] public int Epic;     // =3
    [Min(0)] public int Legendary;// =4

    public int GetCount(Rarity r) => r switch
    {
        Rarity.Uncommon => Uncommon,
        Rarity.Rare => Rare,
        Rarity.Epic => Epic,
        Rarity.Legendary => Legendary,
        _ => 0 // Common = 0 dòng
    };
}

[Serializable]
public struct RarityValue // hệ số giá theo tier
{
    public float Uncommon;  // 1.5
    public float Rare;      // 2.5
    public float Epic;      // 4
    public float Legendary; // 6

    public float Get(Rarity r) => r switch
    {
        Rarity.Uncommon => Uncommon,
        Rarity.Rare => Rare,
        Rarity.Epic => Epic,
        Rarity.Legendary => Legendary,
        _ => 1f // Common
    };
}

[Serializable]
public class RarityBonusEntry
{
    public RarityStat Stat;
    // Giá trị cộng thẳng theo từng tier (không scale theo level)
    public float Uncommon;   // ví dụ Str = 1
    public float Rare;       // ví dụ Str = 2
    public float Epic;       // ví dụ Str = 3
    public float Legendary;  // ví dụ Str = 5

    public float Get(Rarity r) => r switch
    {
        Rarity.Uncommon => Uncommon,
        Rarity.Rare => Rare,
        Rarity.Epic => Epic,
        Rarity.Legendary => Legendary,
        _ => 0f // Common
    };
}

[CreateAssetMenu(fileName = "RarityConfig", menuName = "Game/Loot/Rarity Config")]
public class RarityConfig : ScriptableObject
{
    [Header("Số dòng affix theo tier (Common = 0)")]
    public LinesPerRarity Lines = new LinesPerRarity { Uncommon = 1, Rare = 2, Epic = 3, Legendary = 4 };

    [Header("Giá trị cộng theo bảng rarity (không scale theo level)")]
    public List<RarityBonusEntry> BonusTable = new List<RarityBonusEntry>();

    [Header("Giá bán")]
    public RarityValue PriceMultiplier = new RarityValue { Uncommon = 1.5f, Rare = 2.5f, Epic = 4f, Legendary = 6f };
    [Tooltip("Giá tăng theo cấp: 0.10 = +10% mỗi cấp")]
    public float PriceIncreasePerLevel = 0.10f;

    // Helpers
    public int GetLineCount(Rarity r) => Lines.GetCount(r);

    public float GetBonusValue(RarityStat stat, Rarity r)
    {
        var entry = BonusTable.Find(e => e.Stat == stat);
        return entry != null ? entry.Get(r) : 0f;
    }

    public float GetPriceMultiplier(Rarity r) => PriceMultiplier.Get(r);

    public int ComputeSellPrice(int baseGold, int itemLevel, Rarity r)
    {
        // Common dùng multiplier = 1
        float mul = r == Rarity.Common ? 1f : GetPriceMultiplier(r);
        float levelMul = Mathf.Pow(1f + Mathf.Max(0f, PriceIncreasePerLevel), Mathf.Max(0, itemLevel - 1));
        return Mathf.RoundToInt(baseGold * mul * levelMul);
    }
}

