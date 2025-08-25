// Thuộc nhóm: StatsSystem
// Reference: BaseStats (dùng trong constructor).
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    // ===== Core =====
    public int CurrentHealth;
    public int MaxHealth;

    public int CurrentMana;
    public int MaxMana;

    public int CurrentHunger;
    public int MaxHunger;

    public int CurrentSanity;
    public int MaxSanity;

    public int CurrentStamina;
    public int MaxStamina;

    // ===== Attributes =====
    public int STR;
    public int AGI;
    public int INT;
    public int VIT;
    public int WIS;

    // ===== Derived =====
    public int Armor;
    public int BaseDamage;
    public float CritChance;
    public float CritDamage;
    public float HealthRegen;
    public float ManaRegen;

    // ===== Progression =====
    public int Level;
    public int RemainPoints;

    public CharacterStats(BaseStats baseStats)
    {
        // Max value từ base stats
        MaxHealth = baseStats.Health;
        MaxMana = baseStats.Mana;
        MaxHunger = baseStats.Hunger;
        MaxSanity = baseStats.Sanity;
        MaxStamina = baseStats.Stamina;

        // Current khởi tạo bằng Max
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
        CurrentHunger = MaxHunger;
        CurrentSanity = MaxSanity;
        CurrentStamina = MaxStamina;

        STR = baseStats.STR;
        AGI = baseStats.AGI;
        INT = baseStats.INT;
        VIT = baseStats.VIT;
        WIS = baseStats.WIS;

        Armor = baseStats.Armor;
        BaseDamage = baseStats.BaseDamage;
        CritChance = baseStats.CritChance;
        CritDamage = baseStats.CritDamage;
        HealthRegen = baseStats.HealthRegen;
        ManaRegen = baseStats.ManaRegen;

        Level = baseStats.Level;
        RemainPoints = baseStats.RemainPoints;

        RecalculateStats();
    }

    public void RecalculateStats(bool resetCurrent = false)
    {
        // Max values từ Attributes
        MaxHealth = VIT * 10;
        MaxMana = INT * 10;
        MaxHunger = 100;
        MaxSanity = WIS * 5;
        MaxStamina = AGI * 10;

        if (resetCurrent) // khi level up/reset,
        {
            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;
            CurrentHunger = MaxHunger;
            CurrentSanity = MaxSanity;
            CurrentStamina = MaxStamina;
        }
        else         // RecalculateStats(false) khi chỉ thay đổi stat nhưng không muốn full hồi.
        {
            CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
            CurrentMana = Mathf.Min(CurrentMana, MaxMana);
            CurrentHunger = Mathf.Min(CurrentHunger, MaxHunger);
            CurrentSanity = Mathf.Min(CurrentSanity, MaxSanity);
            CurrentStamina = Mathf.Min(CurrentStamina, MaxStamina);
        }
        // Derived
        Armor = VIT * 2;
        BaseDamage = STR * 2;
        CritChance = AGI * 0.5f + (WIS * 0.2f);
        CritDamage = 150 + STR * 2;
        HealthRegen = VIT * 0.2f;
        ManaRegen = INT * 0.2f + WIS * 0.1f;
    }
}
