// Scripts/Inventory/Item.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Inventory
{
    [Serializable]
    public struct AffixEntry
    {
        public RarityStat Stat;
        public float Value;
    }

    /// <summary>
    /// Item runtime (UI dùng để hiển thị/kéo-thả). Mang theo dữ liệu đã roll.
    /// </summary>
    [Serializable]
    public class Item
    {
        [SerializeField] private Sprite sprite;           // icon hiển thị
        public Sprite Sprite => sprite;

        // Metadata/runtime
        public string DefId;
        public int ItemLevel;
        public Rarity Rarity;
        public Stats ScaledStats;                         // stat đã scale base + cộng affix (kết quả cuối)
        public int SellPrice;
        public List<AffixEntry> Affixes;                  // ✅ các dòng rarity đã roll

        [NonSerialized] public ItemDefinition CachedDef;

        public Item(Sprite sprite,
                    string defId, int itemLevel, Rarity rarity,
                    Stats scaledStats, int sellPrice,
                    ItemDefinition cachedDef,
                    List<AffixEntry> affixes = null)
        {
            this.sprite = sprite;
            DefId = defId;
            ItemLevel = itemLevel;
            Rarity = rarity;
            ScaledStats = scaledStats;
            SellPrice = sellPrice;
            CachedDef = cachedDef;
            Affixes = affixes ?? new List<AffixEntry>(0);
        }
    }
}
