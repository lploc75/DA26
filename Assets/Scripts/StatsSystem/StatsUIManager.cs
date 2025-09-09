// Thuộc nhóm: StatsSystem
// Reference: PlayerStatsManager (biến playerStats)
// Gián tiếp reference tới CharacterStats (thông qua playerStats.currentStats).
using UnityEngine;
using TMPro;

public class StatsUIManager : MonoBehaviour
{
    [Header("Liên kết Player")]
    public PlayerStatsManager playerStats;

    [Header("Core Values")]
    public TMP_Text hpValue;
    public TMP_Text manaValue;
    public TMP_Text hungerValue;
    public TMP_Text sanityValue;
    public TMP_Text staminaValue;

    [Header("Attributes Values")]
    public TMP_Text strValue;
    public TMP_Text agiValue;
    public TMP_Text wisValue;
    public TMP_Text vitValue;
    public TMP_Text intValue;

    [Header("Derived Values")]
    public TMP_Text armorValue;
    public TMP_Text baseDamageValue;
    public TMP_Text critChanceValue;
    public TMP_Text critDamageValue;
    public TMP_Text healthRegenValue;
    public TMP_Text manaRegenValue;

    [Header("Progression Values")]
    public TMP_Text levelValue;
    public TMP_Text remainPointsValue;

    void Update()
    {
        if (playerStats == null || playerStats.currentStats == null) return;
        var stats = playerStats.currentStats;

        // Core
        hpValue.text = $"{stats.CurrentHealth}/{stats.MaxHealth}";
        manaValue.text = $"{stats.CurrentMana}/{stats.MaxMana}";
        sanityValue.text = $"{stats.CurrentSanity}/{stats.MaxSanity}";
        staminaValue.text = $"{stats.CurrentStamina}/{stats.MaxStamina}";
        hungerValue.text = $"{stats.CurrentHunger}/{stats.MaxHunger}";

        // Attributes
        strValue.text = stats.STR.ToString();
        agiValue.text = stats.AGI.ToString();
        intValue.text = stats.INT.ToString();
        vitValue.text = stats.VIT.ToString();
        wisValue.text = stats.WIS.ToString();

        // Derived
        armorValue.text = stats.Armor.ToString();
        baseDamageValue.text = stats.BaseDamage.ToString();
        critChanceValue.text = $"{stats.CritChance:F1}%";
        critDamageValue.text = $"{stats.CritDamage:F1}%";
        healthRegenValue.text = $"{stats.HealthRegen:F1}";
        manaRegenValue.text = $"{stats.ManaRegen:F1}";

        // Progression
        levelValue.text = stats.Level.ToString();
        remainPointsValue.text = stats.RemainPoints.ToString();
    }
    public void OnSaveButton()
    {
        playerStats.SaveStats();
    }

    public void OnLoadButton()
    {
        var savedData = playerStats.dbManager.LoadPlayer();
        if (savedData != null)
            playerStats.ApplyDataToStats(savedData);
    }

}
