using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("Autoload (Resources)")]
    public bool autoLoadResources = true;
    public string resourcesRoot = "Items";                 // Resources/Items
    public string equipmentFolder = "Equipment";           // Resources/Items/Equipment
    public string consumableFolder = "Consumables";        // Resources/Items/Consumables
    public string materialFolder = "Materials";            // Resources/Items/Materials

    // Giữ API cũ cho EQUIPMENT
    public List<ItemDefinition> definitions = new List<ItemDefinition>();   // legacy
    Dictionary<string, ItemDefinition> _equipMap;

    // Bổ sung:
    public List<ConsumableDefinition> consumables = new List<ConsumableDefinition>();
    public List<MaterialDefinition> materials = new List<MaterialDefinition>();
    Dictionary<string, IItemDefinition> _anyMap;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _equipMap = new Dictionary<string, ItemDefinition>();
        _anyMap = new Dictionary<string, IItemDefinition>();

        if (autoLoadResources)
        {
            // 1) Equipment (giữ y nguyên behavior cũ)
            var eq = Resources.LoadAll<ItemDefinition>($"{resourcesRoot}/{equipmentFolder}");
            definitions.Clear();
            definitions.AddRange(eq);

            // 2) Consumables
            var cs = Resources.LoadAll<ConsumableDefinition>($"{resourcesRoot}/{consumableFolder}");
            consumables.Clear();
            consumables.AddRange(cs);

            // 3) Materials
            var ms = Resources.LoadAll<MaterialDefinition>($"{resourcesRoot}/{materialFolder}");
            materials.Clear();
            materials.AddRange(ms);
        }

        // Build maps
        foreach (var d in definitions)
            if (d && !string.IsNullOrEmpty(d.Id))
            {
                _equipMap[d.Id] = d;
                _anyMap[d.Id] = (IItemDefinition)d; // Category = Equipment
            }

        foreach (var c in consumables)
            if (c && !string.IsNullOrEmpty(c.Id))
                _anyMap[c.Id] = c;

        foreach (var m in materials)
            if (m && !string.IsNullOrEmpty(m.Id))
                _anyMap[m.Id] = m;

        // (tuỳ chọn) cảnh báo nếu trùng Id giữa các loại
        // -> có thể thêm validation sau
    }

    // --- API cũ (giữ nguyên) ---
    public ItemDefinition GetItemById(string id)
        => (!string.IsNullOrEmpty(id) && _equipMap.TryGetValue(id, out var def)) ? def : null;

    // --- API mới (gộp) ---
    public IItemDefinition GetAnyById(string id)
        => (!string.IsNullOrEmpty(id) && _anyMap.TryGetValue(id, out var def)) ? def : null;

    public ConsumableDefinition GetConsumableById(string id)
        => (!string.IsNullOrEmpty(id)) ? consumables.Find(x => x && x.Id == id) : null;

    public MaterialDefinition GetMaterialById(string id)
        => (!string.IsNullOrEmpty(id)) ? materials.Find(x => x && x.Id == id) : null;
}
