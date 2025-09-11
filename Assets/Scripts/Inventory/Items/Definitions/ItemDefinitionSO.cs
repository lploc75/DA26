using UnityEngine;

public enum ItemType
{
    Weapon, Helmet, Chest, Pants, Boots, Ring, Necklace
}

public enum ClassRestriction
{
    None, Warrior, Archer, Wizard
}

/// <summary>
/// Cách xác định "Level yêu cầu trang bị":
/// - UseItemLevel: Level yêu cầu = chính ItemLevel (level khi rơi/roll).
/// - FixedValue: Level yêu cầu cố định theo SO (FixedEquipLevel).
/// </summary>
public enum EquipRequirementMode
{
    UseItemLevel, FixedValue
}

[System.Serializable]
public struct Stats
{
    // Resources
    public int MaxHealth;
    public int MaxMana;
    public int MaxHunger;
    public int MaxSanity;
    public int MaxStamina;

    public float HealthRegenPerSec;
    public float ManaRegenPerSec;
    public float StaminaRegenPerSec;
    public float HungerDecayPerSec;
    public float SanityRegenPerSec;

    // Attributes
    public int Strength;
    public int Agility;
    public int Intelligence;
    public int Vitality;
    public int Wisdom;

    // Combat
    public int Armor;
    public int BaseDamage;
    [Range(0, 100)] public float CritChance; // %
    public float CritDamagePct;              // %
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item Definition")]
public class ItemDefinition : ScriptableObject, IItemDefinition   // 👈 thêm interface
{
    [Header("Identity")]
    public string Id;
    public string DisplayName;
    [TextArea(2, 4)] public string Description;
    public Sprite Icon;
    public Sprite TypeIcon;
    public ItemType Type;
    public ClassRestriction ClassOnly = ClassRestriction.None;

    [Header("Economy")]
    public int BaseGoldValue = 10;

    [Header("Equip Requirement")]
    public EquipRequirementMode EquipMode = EquipRequirementMode.UseItemLevel; // ✅
    [Min(1)] public int FixedEquipLevel = 1;                                   // ✅ dùng khi EquipMode=FixedValue

    [Header("Base Stats (Lv1)")]
    public Stats BaseStats;

    [Header("Scaling")]
    [Tooltip("Growth per level (e.g. 0.15 = +15% each level; áp dụng cho MỌI stat base)")]
    public float GrowthPercent = 0.15f;
    ItemCategory IItemDefinition.Category => ItemCategory.Equipment;
    int IItemDefinition.MaxStack => 1;
    string IItemDefinition.Id => Id;
    string IItemDefinition.DisplayName => DisplayName;
    Sprite IItemDefinition.Icon => Icon;
    int IItemDefinition.BaseGoldValue => BaseGoldValue;
    /// <summary>
    /// Trả về stat đã scale theo level (mọi stat base tăng 15%/cấp).
    /// </summary>
    public Stats GetStatsAtLevel(int level)
    {
        Stats result = BaseStats;
        float factor = Mathf.Pow(1f + GrowthPercent, level - 1);

        // Resources
        result.MaxHealth = Mathf.RoundToInt(result.MaxHealth * factor);
        result.MaxMana = Mathf.RoundToInt(result.MaxMana * factor);
        result.MaxHunger = Mathf.RoundToInt(result.MaxHunger * factor);
        result.MaxSanity = Mathf.RoundToInt(result.MaxSanity * factor);
        result.MaxStamina = Mathf.RoundToInt(result.MaxStamina * factor);

        result.HealthRegenPerSec *= factor;
        result.ManaRegenPerSec *= factor;
        result.StaminaRegenPerSec *= factor;
        result.HungerDecayPerSec *= factor;
        result.SanityRegenPerSec *= factor;

        // Attributes
        result.Strength = Mathf.RoundToInt(result.Strength * factor);
        result.Agility = Mathf.RoundToInt(result.Agility * factor);
        result.Intelligence = Mathf.RoundToInt(result.Intelligence * factor);
        result.Vitality = Mathf.RoundToInt(result.Vitality * factor);
        result.Wisdom = Mathf.RoundToInt(result.Wisdom * factor);

        // Combat
        result.Armor = Mathf.RoundToInt(result.Armor * factor);
        result.BaseDamage = Mathf.RoundToInt(result.BaseDamage * factor);
        result.CritChance *= factor;
        result.CritDamagePct *= factor;

        return result;
    }

    /// <summary>
    /// Tính Level yêu cầu để trang bị cho tooltip/logic.
    /// </summary>
    public int GetRequiredLevel(int itemLevelFromInstance)
    {
        return EquipMode == EquipRequirementMode.UseItemLevel
            ? Mathf.Max(1, itemLevelFromInstance)
            : Mathf.Max(1, FixedEquipLevel);
    }
}
