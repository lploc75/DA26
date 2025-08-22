using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public int HP;
    public int MP;
    public int SP;
    public int Hunger;
    public int Speed;
    public int AttackSpeed;
    public int CriticalChance;
    public int CriticalDamage;
    public int BaseDamage;

    public int STR;
    public int INT;
    public int DUR;
    public int PER;
    public int VIT;
    public int AGI;

    public CharacterStats(BaseStats baseStats)
    {
        // Copy dữ liệu gốc
        HP = baseStats.HP;
        MP = baseStats.MP;
        SP = baseStats.SP;
        Hunger = baseStats.Hunger;
        Speed = baseStats.Speed;
        AttackSpeed = baseStats.AttackSpeed;
        CriticalChance = baseStats.CriticalChance;
        CriticalDamage = baseStats.CriticalDamage;
        BaseDamage = baseStats.BaseDamage;

        STR = baseStats.STR;
        INT = baseStats.INT;
        DUR = baseStats.DUR;
        PER = baseStats.PER;
        VIT = baseStats.VIT;
        AGI = baseStats.AGI;
    }

    public void RecalculateStats()
    {
        // Ví dụ công thức tính chỉ số cơ bản từ thuộc tính
        HP = VIT * 10;
        MP = INT * 6;
        SP = DUR * 5;
        Hunger = 95 + DUR;
        BaseDamage = STR * 2;
        Speed = AGI * 2;
        // Attack Speed: Mỗi điểm AGI tăng 0.1, mỗi điểm STR tăng 0.05
        AttackSpeed = Mathf.RoundToInt(AGI * 0.1f + STR * 0.05f);
        CriticalChance = Mathf.RoundToInt(PER * 0.2f);
        CriticalDamage = BaseDamage + (STR * 2);
    }
}
