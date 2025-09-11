using UnityEngine;

public enum ConsumableEffectKind
{
    HealHealthFlat,      // +x máu
    HealManaFlat,        // +x mana
    HealHealthPercent,   // +% máu tối đa
    HealManaPercent,     // +% mana tối đa
    AddBuffFlat,         // +x vào 1 chỉ số (vd Strength)
    AddBuffPercent,      // +% vào 1 chỉ số (vd BaseDamage %)
    Custom               // hook về code tùy chỉnh sau
}

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Game/Items/Consumable Definition")]
public class ConsumableDefinition : ScriptableObject, IItemDefinition
{
     string IItemDefinition.Id => Id;
    string IItemDefinition.DisplayName => DisplayName;
    Sprite IItemDefinition.Icon => Icon;
    int IItemDefinition.BaseGoldValue => BaseGoldValue;
    int IItemDefinition.MaxStack => MaxStack;
    ItemCategory IItemDefinition.Category => Category;

    [Header("Identity")]
    public string Id;
    public string DisplayName;
    [TextArea(2, 4)] public string Description;
    public Sprite Icon;

    [Header("Economy/Stack")]
    [Min(1)] public int BaseGoldValue = 5;
    [Min(1)] public int MaxStack = 99;

    [Header("Effect")]
    public ConsumableEffectKind EffectKind = ConsumableEffectKind.HealHealthFlat;
    public float Amount = 50f;             // trị số chính (máu/mana/buff)
    public float DurationSeconds = 0f;     // 0 = instant (heal ngay)
    public float CooldownSeconds = 0f;     // 0 = không CD

    // IItemDefinition
    public ItemCategory Category => ItemCategory.Consumable;
}
