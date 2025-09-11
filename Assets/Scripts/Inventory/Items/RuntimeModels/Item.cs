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
        public Sprite Sprite { get; private set; }
        public string DefId { get; private set; }
        public int ItemLevel { get; private set; }
        public Rarity Rarity { get; private set; }
        public Stats ScaledStats { get; private set; }
        public int SellPrice { get; private set; }
        public ItemDefinition CachedDef { get; private set; }
        public List<AffixEntry> Affixes { get; private set; }

        public Item(Sprite sprite, string defId, int itemLevel, Rarity rarity,
                    Stats scaledStats, int sellPrice,
                    ItemDefinition cachedDef,
                    List<AffixEntry> affixes = null)
        {
            Sprite = sprite;
            DefId = defId;
            ItemLevel = itemLevel;
            Rarity = rarity;
            ScaledStats = scaledStats;
            SellPrice = sellPrice;
            CachedDef = cachedDef;
            Affixes = affixes ?? new List<AffixEntry>();
        }
    }
}