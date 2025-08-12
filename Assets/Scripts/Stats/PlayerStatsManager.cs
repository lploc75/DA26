using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("Dữ liệu gốc (ScriptableObject)")]
    public BaseStats baseStats;

    [Header("Dữ liệu runtime")]
    public CharacterStats currentStats;

    void Start()
    {
        // Tạo bản sao runtime từ dữ liệu gốc
        currentStats = new CharacterStats(baseStats);

        // Tính toán chỉ số ban đầu dựa trên thuộc tính
        currentStats.RecalculateStats();

        // Debug để kiểm tra
        Debug.Log($"HP ban đầu: {currentStats.HP}");
        Debug.Log($"Base Damage ban đầu: {currentStats.BaseDamage}");
    }
    // TEST TĂNG CHỈ SỐ
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space)) // Nhấn Space để cộng điểm
    //    {
    //        IncreaseAttribute("STR", 5);
    //        IncreaseAttribute("INT", 5);
    //        IncreaseAttribute("DUR", 5);
    //        IncreaseAttribute("PER", 5);
    //        IncreaseAttribute("VIT", 5);
    //        IncreaseAttribute("AGI", 5);
    //    }
    //}
    // Hàm cộng điểm thuộc tính
    public void IncreaseAttribute(string attributeName, int amount)
    {
        switch (attributeName.ToUpper())
        {
            case "STR": currentStats.STR += amount; break;
            case "INT": currentStats.INT += amount; break;
            case "DUR": currentStats.DUR += amount; break;
            case "PER": currentStats.PER += amount; break;
            case "VIT": currentStats.VIT += amount; break;
            case "AGI": currentStats.AGI += amount; break;
            default:
                Debug.LogWarning("Thuộc tính không tồn tại!");
                return;
        }

        // Tính lại chỉ số sau khi tăng
        currentStats.RecalculateStats();

        Debug.Log($"{attributeName} tăng {amount} → HP mới: {currentStats.HP}");
    }
}
