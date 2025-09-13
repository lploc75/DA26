using UnityEngine;
using System;

/// <summary>
/// Các chỉ số mà consumable có thể hồi/ảnh hưởng.
/// </summary>
public enum EffectTarget
{
    Health,
    Mana,
    Hunger,
    Sanity,
    Stamina
}

/// <summary>
/// Kiểu giá trị: cộng trực tiếp hay % so với Max.
/// </summary>
public enum AmountType
{
    Flat,       // +x trực tiếp
    Percent     // +x% của Max
}

/// <summary>
/// Một dòng hiệu ứng cụ thể của consumable.
/// </summary>
[System.Serializable]
public struct EffectEntry
{
    [Header("What")]
    public EffectTarget Target;      // chỉ số bị tác động

    [Header("How much")]
    public AmountType AmountType;    // Flat hay Percent
    public float Amount;             // Flat: giá trị trực tiếp | Percent: 0–100 (%)

    [Header("Timing (optional)")]
    public float DurationSeconds;    // 0 = instant, >0 = hiệu ứng theo thời gian
    public float CooldownSeconds;    // 0 = không cooldown
}

/// <summary>
/// Định nghĩa item tiêu hao (Consumable) có thể chứa nhiều hiệu ứng.
/// </summary>
[CreateAssetMenu(fileName = "NewConsumable",
    menuName = "Game/Items/Consumable Definition (Multi-Effect)")]
public class ConsumableDefinition : ScriptableObject, IItemDefinition
{
    // ================= IItemDefinition =================
    string IItemDefinition.Id => Id;
    string IItemDefinition.DisplayName => DisplayName;
    Sprite IItemDefinition.Icon => Icon;
    int IItemDefinition.BaseGoldValue => BaseGoldValue;
    int IItemDefinition.MaxStack => MaxStack;
    ItemCategory IItemDefinition.Category => ItemCategory.Consumable;

    // ================= Identity =================
    [Header("Identity")]
    public string Id;
    public string DisplayName;
    [TextArea(2, 4)] public string Description;
    public Sprite Icon;

    // ================= Economy/Stack =================
    [Header("Economy / Stack")]
    [Min(1)] public int BaseGoldValue = 5;
    [Min(1)] public int MaxStack = 99;

    // ================= Effects =================
    [Header("Effects (multi-effect)")]
    public EffectEntry[] Effects;

#if UNITY_EDITOR
    /// <summary>
    /// Kiểm tra nhanh khi chỉnh Inspector để tránh cấu hình sai.
    /// </summary>
    void OnValidate()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Debug.LogWarning($"[ConsumableDefinition] {name}: Missing Id");
            return;
        }

        if (Effects == null || Effects.Length == 0)
        {
            Debug.LogWarning($"[ConsumableDefinition] {name}: No effects defined");
            return;
        }

        for (int i = 0; i < Effects.Length; i++)
        {
            var e = Effects[i];
            if (e.AmountType == AmountType.Percent && (e.Amount < 0f || e.Amount > 100f))
                Debug.LogWarning($"[ConsumableDefinition] {name}: Effect[{i}] Percent should be 0–100");
            if (e.AmountType == AmountType.Flat && e.Amount < 0f)
                Debug.LogWarning($"[ConsumableDefinition] {name}: Effect[{i}] Flat must be >= 0");
        }
    }
#endif
}
