// Thuộc nhóm: StatsSystem
// Không reference script khác trong nhóm (là ScriptableObject gốc).
using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Stats/Base Stats")]
public class BaseStats : ScriptableObject
{
    [Header("Chỉ số cơ bản")]
    public int Health;
    public int Mana;
    public int Hunger;
    public int Sanity;
    public int Stamina;

    [Header("Thuộc tính chính")]
    public int STR; // Strength
    public int AGI; // Agility
    public int INT; // Intelligence
    public int VIT; // Vitality
    public int WIS; // Wisdom

    [Header("Chỉ số dẫn xuất")]
    public int Armor;
    public int BaseDamage;
    public float CritChance;   // %
    public float CritDamage;   // %
    public float HealthRegen;  // HP / sec
    public float ManaRegen;    // MP / sec

    [Header("Tiến trình")]
    public int Level = 1;
    public int RemainPoints = 0;
}
