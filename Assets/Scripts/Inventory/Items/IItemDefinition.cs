using UnityEngine;

public enum ItemCategory
{
    Equipment = 0,   // Trang bị (hiện đang dùng)
    Consumable = 1,  // Bình máu/mana, thức ăn, thuốc buff...
    Material = 2,    // Nguyên liệu chế tạo
    Quest = 3,       // Vật phẩm nhiệm vụ (tùy chọn dùng sau)
    Misc = 4         // Linh tinh
}

/// Định nghĩa tối thiểu để quản lý và hiển thị chung.
public interface IItemDefinition
{
    string Id { get; }
    string DisplayName { get; }
    Sprite Icon { get; }
    int BaseGoldValue { get; }
    ItemCategory Category { get; }
    int MaxStack { get; }    // Equipment = 1, Consumable/Material có thể >1
}
