using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "AreaDropTable_Material", menuName = "Game/Loot/Area Drop Table (Material)")]
public class AreaDropTable_Material : ScriptableObject
{
    [Header("Region filter")]
    public int RegionId = 1;
    public string RegionName = "Grassland";

    [Header("Pool & Weights")]
    public Entry[] Pool;

    [Serializable]
    public struct Entry
    {
        public MaterialDefinition item;   // ScriptableObject của nguyên liệu
        [Range(0f, 1f)] public float weight;
    }

    public bool Validate(out string msg)
    {
        msg = "";
        if (Pool == null || Pool.Length == 0) { msg = "Pool is empty."; return false; }
        float sum = Pool.Where(e => e.item).Sum(e => e.weight);
        if (sum <= 0f) { msg = "Total weight ≤ 0."; return false; }
        return true;
    }

    public MaterialDefinition PickItem(System.Random rng)
    {
        float total = Pool.Where(e => e.item).Sum(e => e.weight);
        if (total <= 0f) return null;
        double roll = rng.NextDouble() * total;
        double acc = 0;
        foreach (var e in Pool)
        {
            if (!e.item) continue;
            acc += e.weight;
            if (roll <= acc) return e.item;
        }
        return Pool.LastOrDefault(p => p.item).item;
    }
}
