using UnityEngine;

[CreateAssetMenu(fileName = "NewMaterial", menuName = "Game/Items/Material Definition")]
public class MaterialDefinition : ScriptableObject, IItemDefinition
{
    public ItemCategory Category => ItemCategory.Material;

    // ✅ explicit interface
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
    [Min(0)] public int BaseGoldValue = 1;
    [Min(1)] public int MaxStack = 999;

    // IItemDefinition
   
}
