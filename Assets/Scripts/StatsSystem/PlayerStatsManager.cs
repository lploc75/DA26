// Thuộc nhóm: StatsSystem
// Reference: BaseStats (biến baseStats)
// Reference: CharacterStats (biến currentStats)
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    [Header("Dữ liệu gốc (ScriptableObject)")]
    public BaseStats baseStats;

    [Header("Dữ liệu runtime")]
    public CharacterStats currentStats;

    void Start()
    {
        currentStats = new CharacterStats(baseStats);
        currentStats.RecalculateStats(true);

        // Debug test
        Debug.Log($"==== PLAYER STATS ====");
        Debug.Log($"Level: {currentStats.Level}");
        Debug.Log($"Health: {currentStats.CurrentHealth}/{currentStats.MaxHealth}");
        Debug.Log($"Mana: {currentStats.CurrentMana}/{currentStats.MaxMana}");
        Debug.Log($"Sanity: {currentStats.CurrentSanity}/{currentStats.MaxSanity}");
        Debug.Log($"Stamina: {currentStats.CurrentStamina}/{currentStats.MaxStamina}");
        Debug.Log($"Hunger: {currentStats.CurrentHunger}/{currentStats.MaxHunger}");

        Debug.Log($"STR: {currentStats.STR}, AGI: {currentStats.AGI}, INT: {currentStats.INT}, VIT: {currentStats.VIT}, WIS: {currentStats.WIS}");

        Debug.Log($"Armor: {currentStats.Armor}");
        Debug.Log($"Base Damage: {currentStats.BaseDamage}");
        Debug.Log($"Crit Chance: {currentStats.CritChance:F1}%");
        Debug.Log($"Crit Damage: {currentStats.CritDamage:F1}%");
        Debug.Log($"HP Regen: {currentStats.HealthRegen:F1}");
        Debug.Log($"Mana Regen: {currentStats.ManaRegen:F1}");
        Debug.Log($"Remain Points: {currentStats.RemainPoints}");
        Debug.Log("=======================");
    }
    public void AddPointToStat(string statName)
    {
        if (currentStats.RemainPoints <= 0)
        {
            Debug.Log("Không còn điểm để cộng!");
            return;
        }

        switch (statName.ToUpper())
        {
            case "STR": currentStats.STR++; break;
            case "AGI": currentStats.AGI++; break;
            case "INT": currentStats.INT++; break;
            case "VIT": currentStats.VIT++; break;
            case "WIS": currentStats.WIS++; break;
            default:
                Debug.LogWarning("Thuộc tính không tồn tại!");
                return;
        }

        currentStats.RemainPoints--;
        currentStats.RecalculateStats();

        Debug.Log($"[{statName}] +1 → STR:{currentStats.STR} | " +
                  $"AGI:{currentStats.AGI} | INT:{currentStats.INT} | " +
                  $"VIT:{currentStats.VIT} | WIS:{currentStats.WIS} | " +
                  $"Remain:{currentStats.RemainPoints}");
    }
}
