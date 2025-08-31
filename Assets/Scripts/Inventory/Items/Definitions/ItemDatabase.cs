using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }
    public List<ItemDefinition> definitions = new List<ItemDefinition>();
    public bool autoLoadResources = true;
    public string resourcesFolder = "Items"; // Resources/Items

    Dictionary<string, ItemDefinition> _map;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (autoLoadResources)
        {
            var all = Resources.LoadAll<ItemDefinition>(resourcesFolder);
            definitions.Clear(); definitions.AddRange(all);
        }
        _map = new Dictionary<string, ItemDefinition>();
        foreach (var d in definitions)
            if (d && !string.IsNullOrEmpty(d.Id)) _map[d.Id] = d;
    }

    public ItemDefinition GetItemById(string id)
        => (!string.IsNullOrEmpty(id) && _map.TryGetValue(id, out var def)) ? def : null;
}
