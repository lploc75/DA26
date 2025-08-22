using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Stats/Base Stats")]
public class BaseStats : ScriptableObject
{

    [Header("Chỉ số cơ bản")]
    public int HP;
    public int MP;
    public int SP;
    public int Hunger;
    public int Speed;
    public int AttackSpeed;
    public int CriticalChance; // Tỷ lệ chí mạng
    public int CriticalDamage; // Sát thương chí mạng
    public int BaseDamage;

    [Header("Chỉ số thuộc tính")]
    public int STR; // Sức mạnh
    public int INT; // Trí tuệ
    public int DUR; // Độ bền
    public int PER; // Nhận thức
    public int VIT; // Thể lực
    public int AGI; // Nhanh nhẹn
}
