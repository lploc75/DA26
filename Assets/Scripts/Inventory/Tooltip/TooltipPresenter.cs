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

    [Header("Price")]
    public TMP_Text priceTMP;
    [Tooltip("Đuôi đơn vị vàng hiển thị sau số (vd: ' g')")]
    public string goldSuffix = " g";
    [Tooltip("Rút gọn 1.2K / 3.4M thay vì số đầy đủ")]
    public bool compactGold = true;
    [Tooltip("Hiển thị dấu phân cách hàng nghìn nếu không rút gọn")]
    public bool thousandSep = true;

    [System.Serializable]
    public class AttrLine
    {
        public TMP_Text label;   // Text cha (nhãn)
        public TMP_Text value;   // Text con (số)
        public GameObject root;  // optional container cho dòng
    }

    [Header("Attribute Lines (6 lines)")]
    public AttrLine[] lines = new AttrLine[6];

    [Header("Config (chỉ dùng màu theo rarity)")]
    public RarityConfig rarityConfig;
    public bool colorize = true;

    // ============================================================
    // ENTRY: phân nhánh theo loại định nghĩa (Equipment / Consumable / Material)
    // ============================================================
    // ==== REPLACE method ShowFor ====
    public void ShowFor(Item uiItem)
    {
        if (uiItem == null) { gameObject.SetActive(false); return; }

        // 1) Equipment path (Giữ nguyên behaviour cũ)
        var equipDef = uiItem.CachedDef ?? ItemDatabase.Instance?.GetItemById(uiItem.DefId);
        if (equipDef != null)
        {
            Debug.Log($"[Tooltip] Equipment def found for {uiItem.DefId}");
            RenderEquipment(equipDef, uiItem);
            gameObject.SetActive(true);
            return;
        }

        // 2) Không phải Equipment -> thử load Consumable/Material từ Resources
        //    NOTE: chỉnh lại folder nếu bạn để khác (ví dụ "Items/Consumables", "Items/Materials")
        var candsConsum = Resources.LoadAll<ConsumableDefinition>("Consumables");
        var cdef = candsConsum != null ? System.Linq.Enumerable.FirstOrDefault(candsConsum, d => d && d.Id == uiItem.DefId) : null;

        if (cdef != null)
        {
            Debug.Log($"[Tooltip] Consumable def found for {uiItem.DefId}");
            RenderConsumable(cdef, uiItem);
            gameObject.SetActive(true);
            return;
        }

        var candsMat = Resources.LoadAll<MaterialDefinition>("Materials");
        var mdef = candsMat != null ? System.Linq.Enumerable.FirstOrDefault(candsMat, d => d && d.Id == uiItem.DefId) : null;

        if (mdef != null)
        {
            Debug.Log($"[Tooltip] Material def found for {uiItem.DefId}");
            RenderMaterial(mdef, uiItem);
            gameObject.SetActive(true);
            return;
        }

        Debug.LogWarning($"[Tooltip] No definition found for {uiItem.DefId} (neither Equipment nor Consumable/Material). Hiding tooltip.");
        gameObject.SetActive(false);
    }

    // ==== ADD new methods below (keep your existing RenderEquipment/BuildRows/RenderRows/Utils) ====

    // CONSUMABLE
    // Build các dòng hiệu ứng từ ConsumableDefinition (đa hiệu ứng)
    // Build các dòng hiệu ứng từ ConsumableDefinition (đa hiệu ứng)
    List<(string label, string value)> BuildConsumableLines(ConsumableDefinition def)
    {
        var result = new List<(string, string)>(6);
        if (def.Effects == null || def.Effects.Length == 0) return result;

        foreach (var e in def.Effects)
        {
            // Map target → nhãn
            string label = e.Target switch
            {
                EffectTarget.Health => "HP",
                EffectTarget.Mana => "MP",
                EffectTarget.Hunger => "Hunger",
                EffectTarget.Sanity => "Sanity",
                EffectTarget.Stamina => "Stamina",
                _ => "Effect"
            };

            // Giá trị hiển thị: theo Flat/Percent (+x | +x%)
            string value = e.AmountType == AmountType.Percent
                ? FormatVal(e.Amount, percent: true, plus: true)   // ví dụ: +10%
                : FormatVal(e.Amount, percent: false, plus: true); // ví dụ: +25

            result.Add((label, value));
            if (result.Count >= 6) break; // tối đa 6 dòng như yêu cầu
        }

        return result;
    }

    // MATERIAL
    void RenderMaterial(MaterialDefinition def, Item uiItem)
    {
        if (nameTMP) nameTMP.text = string.IsNullOrEmpty(def.DisplayName) ? def.Id : def.DisplayName;
        if (descTMP) descTMP.text = def.Description;

        if (typeImage) typeImage.sprite = null;
        if (rarityTMP) rarityTMP.text = "";
        if (levelTMP) levelTMP.text = "";

        int price = uiItem.SellPrice > 0 ? uiItem.SellPrice : def.BaseGoldValue;
        if (priceTMP) priceTMP.text = FormatGold(price);

        ClearAllLines();
        for (int i = 0; i < lines.Length; i++) HideLine(i);
    }

    // Tuỳ hóa "công dụng" thành 1 dòng gọn

    public void Hide() => gameObject.SetActive(false);

    // ============================================================
    // EQUIPMENT (giữ nguyên behaviour cũ)
    // ============================================================
    void RenderEquipment(ItemDefinition def, Item uiItem)
    {
        // Header
        if (nameTMP) nameTMP.text = string.IsNullOrEmpty(def.DisplayName) ? def.Id : def.DisplayName;
        if (descTMP) descTMP.text = def.Description;
        if (typeImage) typeImage.sprite = def.TypeIcon;
        if (rarityTMP) rarityTMP.text = Paint(uiItem.Rarity.ToString(), RarityColor(uiItem.Rarity));
        if (levelTMP) levelTMP.text = uiItem.ItemLevel.ToString();

        // Price
        if (priceTMP) priceTMP.text = FormatGold(uiItem.SellPrice);

        // Rows: BASE + RARITY
        var rows = BuildRows(def, uiItem);
        RenderRows(rows);
    }
    // CONSUMABLE (đa hiệu ứng: hiển thị từng effect thành 1 dòng)
    void RenderConsumable(ConsumableDefinition def, Item uiItem)
    {
        // Header
        if (nameTMP) nameTMP.text = string.IsNullOrEmpty(def.DisplayName) ? def.Id : def.DisplayName;
        if (descTMP) descTMP.text = def.Description;

        if (typeImage) typeImage.sprite = null; // có thể set icon loại "Consumable" riêng nếu bạn có
        if (rarityTMP) rarityTMP.text = "";     // chưa dùng rarity cho consumable
        if (levelTMP) levelTMP.text = "";       // không hiển thị level

        // Price: ưu tiên giá trên item (nếu đã tính), fallback base
        int price = (uiItem != null && uiItem.SellPrice > 0) ? uiItem.SellPrice : def.BaseGoldValue;
        if (priceTMP) priceTMP.text = FormatGold(price);

        // Lines: mỗi EffectEntry → 1 dòng, tối đa 6
        ClearAllLines();

        var rows = BuildConsumableLines(def);
        int count = Mathf.Min(rows.Count, lines.Length);
        for (int i = 0; i < count; i++)
        {
            var row = rows[i]; // (label, value)
            SetLine(i, row.label, row.value, isRarity: false);
        }
        // Ẩn các dòng thừa
        for (int i = count; i < lines.Length; i++) HideLine(i);
    }



    struct Row { public string label; public string value; public bool isRarity; public Row(string l, string v, bool r) { label = l; value = v; isRarity = r; } }

    List<Row> BuildRows(ItemDefinition def, Item uiItem)
    {
        var result = new List<Row>(6);

        var b = def.GetStatsAtLevel(uiItem.ItemLevel);

        var baseCandidates = new List<(string label, float val, bool pct)>
        {
            ("Strength", b.Strength, false),
            ("Agility", b.Agility, false),
            ("Intelligence", b.Intelligence, false),
            ("Vitality", b.Vitality, false),
            ("Wisdom", b.Wisdom, false),

            ("Damage", b.BaseDamage, false),
            ("Armor", b.Armor, false),
            ("Crit Chance", b.CritChance, true),
            ("Crit Damage", b.CritDamagePct, true),

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

        int added = 0;
        if (uiItem.Affixes != null)
        {
            foreach (var a in uiItem.Affixes)
            {
                var (label, isPct) = MapStatToLabel(a.Stat);
                if (string.IsNullOrEmpty(label)) continue;
                result.Add(new Row(label, FormatVal(a.Value, isPct, true), true));
                if (++added >= 4) break;
            }
        }

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

    void RenderRows(List<Row> rows)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            var l = lines[i];
            var go = (l?.root) ? l.root : l?.label?.gameObject;

            if (l == null || l.label == null || l.value == null)
            {
                if (go) go.SetActive(false);
                if (l != null) Debug.LogWarning($"[TooltipPresenter] Missing ref at line {i} (label or value).");
                continue;
            }

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

    // Helpers cho Consumable/Material: bật/tắt từng dòng
    void ClearAllLines()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i]?.root) lines[i].root.SetActive(false);
            if (lines[i]?.label) lines[i].label.text = "";
            if (lines[i]?.value) lines[i].value.text = "";
        }
    }

    void HideLine(int i)
    {
        var l = (i >= 0 && i < lines.Length) ? lines[i] : null;
        if (l == null) return;
        if (l.root) l.root.SetActive(false);
        if (l.label) l.label.text = "";
        if (l.value) l.value.text = "";
    }

    void SetLine(int i, string label, string value, bool isRarity)
    {
        var l = (i >= 0 && i < lines.Length) ? lines[i] : null;
        if (l == null || l.label == null || l.value == null)
        {
            Debug.LogWarning($"[TooltipPresenter] Missing refs for line {i}");
            return;
        }
        if (l.root) l.root.SetActive(true);
        l.label.text = label;
        if (colorize)
        {
            var col = isRarity ? new Color32(255, 170, 30, 255) : new Color32(0, 255, 102, 255);
            l.value.text = Paint(value, col);
        }
        else l.value.text = value;
    }

    // ---------- Utils ----------
    static bool IsZero(float v) => Mathf.Approximately(v, 0f);

    string FormatVal(float val, bool percent, bool plus)
    {
        var sign = plus ? (val >= 0 ? "+" : "-") : "";
        if (percent) return $"{sign}{Mathf.Abs(val):0.#}%";
        return $"{sign}{Mathf.RoundToInt(Mathf.Abs(val))}";
    }

    string FormatGold(int v)
    {
        if (compactGold)
        {
            if (v >= 1_000_000) return $"{v / 1_000_000f:0.#}M{goldSuffix}";
            if (v >= 1_000) return $"{v / 1_000f:0.#}K{goldSuffix}";
            return $"{v}{goldSuffix}";
        }
        return (thousandSep ? v.ToString("#,0") : v.ToString()) + goldSuffix;
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

    void OnValidate()
    {
        if (lines == null) return;
        for (int i = 0; i < lines.Length; i++)
        {
            var l = lines[i];
            if (l != null && l.label != null && l.value == null)
            {
                var child = l.label.GetComponentInChildren<TMP_Text>(true);
                if (child != null && child != l.label) lines[i].value = child;
            }
        }
    }
}
