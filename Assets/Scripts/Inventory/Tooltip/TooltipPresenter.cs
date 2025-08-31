// Scripts/UI/TooltipPresenter.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Scripts.Inventory;

public class TooltipPresenter : MonoBehaviour
{
    [Header("Header")]
    public TMP_Text nameTMP;
    public TMP_Text descTMP;
    public Image typeImage;
    public TMP_Text rarityTMP;
    public TMP_Text levelTMP;

    [System.Serializable]
    public class AttrLine
    {
        public TMP_Text label;   // Text cha (nhãn)
        public TMP_Text value;   // Text con (số)
        public GameObject root;  // optional container cho dòng
    }

    [Header("Attribute Lines (6 lines)")]
    public AttrLine[] lines = new AttrLine[6];

    [Header("Config (chỉ dùng để lấy màu rarity)")]
    public RarityConfig rarityConfig;
    public bool colorize = true;

    public void ShowFor(Item uiItem)
    {
        if (uiItem == null) { gameObject.SetActive(false); return; }
        var def = uiItem.CachedDef ?? ItemDatabase.Instance?.GetItemById(uiItem.DefId);
        if (def == null) { gameObject.SetActive(false); return; }

        // Header
        if (nameTMP) nameTMP.text = string.IsNullOrEmpty(def.DisplayName) ? def.Id : def.DisplayName;
        if (descTMP) descTMP.text = def.Description;
        if (typeImage) typeImage.sprite = def.TypeIcon;
        if (rarityTMP) rarityTMP.text = Paint(uiItem.Rarity.ToString(), RarityColor(uiItem.Rarity));
        if (levelTMP) levelTMP.text = uiItem.ItemLevel.ToString();

        // Rows: 2 base + affix đã roll (tối đa 4 để vừa UI)
        var rows = BuildRows(def, uiItem);
        RenderRows(rows);

        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    // ---------- Rows ----------

    struct Row { public string label; public string value; public bool isRarity; public Row(string l, string v, bool r) { label = l; value = v; isRarity = r; } }

    List<Row> BuildRows(ItemDefinition def, Item uiItem)
    {
        var result = new List<Row>(6);

        // BASE (chỉ từ SO, scale theo level) – chọn 2 dòng phù hợp loại
        var b = def.GetStatsAtLevel(uiItem.ItemLevel);

        var baseCandidates = new List<(string label, float val, bool pct)>
        {
            // Attributes
            ("Strength", b.Strength, false),
            ("Agility", b.Agility, false),
            ("Intelligence", b.Intelligence, false),
            ("Vitality", b.Vitality, false),
            ("Wisdom", b.Wisdom, false),

            // Combat
            ("Damage", b.BaseDamage, false),
            ("Armor", b.Armor, false),
            ("Crit Chance", b.CritChance, true),
            ("Crit Damage", b.CritDamagePct, true),

            // Resources
            ("Max HP", b.MaxHealth, false),
            ("Max MP", b.MaxMana, false),
        };

        int picked = 0;
        foreach (var c in PrioritizeByType(def.Type, baseCandidates))
        {
            if (IsZero(c.val)) continue;
            result.Add(new Row(c.label, FormatVal(c.val, c.pct, true), false));
            if (++picked >= 2) break;
        }

        // RARITY: dùng chính các affix đã roll
        int added = 0;
        if (uiItem.Affixes != null)
        {
            foreach (var a in uiItem.Affixes)
            {
                var (label, isPct) = MapStatToLabel(a.Stat);
                if (string.IsNullOrEmpty(label)) continue;
                result.Add(new Row(label, FormatVal(a.Value, isPct, true), true));
                if (++added >= 4) break; // vừa UI của bạn
            }
        }

        // Cap 6 dòng
        if (result.Count > 6) result.RemoveRange(6, result.Count - 6);
        return result;
    }

    IEnumerable<(string label, float val, bool pct)> PrioritizeByType(
        ItemType type, List<(string label, float val, bool pct)> all)
    {
        string[] order = type switch
        {
            ItemType.Weapon => new[] { "Damage", "Crit Chance", "Crit Damage", "Strength", "Agility", "Intelligence", "Max HP", "Armor" },
            ItemType.Helmet => new[] { "Armor", "Max HP", "Max MP", "Strength", "Intelligence", "Wisdom" },
            ItemType.Chest => new[] { "Armor", "Max HP", "Max MP", "Vitality", "Strength", "Wisdom" },
            ItemType.Pants => new[] { "Armor", "Max HP", "Max MP", "Agility", "Vitality", "Strength" },
            ItemType.Boots => new[] { "Armor", "Max HP", "Max MP", "Agility", "Strength", "Vitality" },
            ItemType.Ring => new[] { "Max HP", "Max MP", "Crit Chance", "Crit Damage", "Wisdom", "Intelligence" },
            ItemType.Necklace => new[] { "Max HP", "Max MP", "Crit Chance", "Crit Damage", "Wisdom", "Intelligence" },
            _ => new[] { "Damage", "Armor", "Max HP", "Max MP", "Strength", "Agility" }
        };

        foreach (var key in order)
            foreach (var c in all)
                if (c.label == key) yield return c;

        foreach (var c in all) yield return c;
    }

    static (string label, bool isPercent) MapStatToLabel(RarityStat s) => s switch
    {
        RarityStat.Strength => ("Strength", false),
        RarityStat.Agility => ("Agility", false),
        RarityStat.Intelligence => ("Intelligence", false),
        RarityStat.Vitality => ("Vitality", false),
        RarityStat.Wisdom => ("Wisdom", false),

        RarityStat.Health => ("Max HP", false),
        RarityStat.Mana => ("Max MP", false),

        RarityStat.Armor => ("Armor", false),
        RarityStat.BaseDamage => ("Damage", false),

        RarityStat.CritChance => ("Crit Chance", true),
        RarityStat.CritDamagePct => ("Crit Damage", true),

        RarityStat.HealthRegenPerSec => ("HP Regen", false),
        RarityStat.ManaRegenPerSec => ("MP Regen", false),
        _ => (null, false)
    };

    // ---------- Render ----------

    void RenderRows(List<Row> rows)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            var l = lines[i];
            if (l == null || l.label == null || l.value == null) continue;

            var go = l.root ? l.root : l.label.gameObject;

            if (i < rows.Count)
            {
                var row = rows[i];
                go.SetActive(true);

                l.label.text = row.label;
                if (colorize)
                {
                    var col = row.isRarity ? new Color32(255, 170, 30, 255) : new Color32(0, 255, 102, 255);
                    l.value.text = Paint(row.value, col);
                }
                else l.value.text = row.value;
            }
            else
            {
                go.SetActive(false);
                l.label.text = "";
                l.value.text = "";
            }
        }
    }

    // ---------- Utils ----------
    static bool IsZero(float v) => Mathf.Approximately(v, 0f);

    string FormatVal(float val, bool percent, bool plus)
    {
        var sign = plus ? (val >= 0 ? "+" : "-") : "";
        if (percent) return $"{sign}{Mathf.Abs(val):0.#}%";
        return $"{sign}{Mathf.RoundToInt(Mathf.Abs(val))}";
    }

    string Paint(string s, Color c) => $"<color=#{ColorUtility.ToHtmlStringRGB(c)}>{s}</color>";

    Color RarityColor(Rarity r) => r switch
    {
        Rarity.Common => new Color32(200, 200, 200, 255),
        Rarity.Uncommon => new Color32(80, 200, 120, 255),
        Rarity.Rare => new Color32(80, 160, 255, 255),
        Rarity.Epic => new Color32(180, 80, 255, 255),
        Rarity.Legendary => new Color32(255, 170, 30, 255),
        _ => Color.white
    };
}
