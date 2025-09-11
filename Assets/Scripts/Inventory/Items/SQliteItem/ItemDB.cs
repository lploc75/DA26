using System.IO;
using UnityEngine;
using Unity.VisualScripting.Dependencies.Sqlite;
using System.Collections.Generic;
using System.Linq;

public class ItemDB
{
    private SQLiteConnection _conn;

    public ItemDB(string dbName = "game.db")
    {
        string dbPath = Path.Combine(Application.persistentDataPath, dbName);
        _conn = new SQLiteConnection(dbPath);
        _conn.CreateTable<ItemEntity>();
        Debug.Log("✅ SQLite DB initialized at: " + dbPath);
    }

    public void SaveItem(Scripts.Inventory.Item item)
    {
        var entity = new ItemEntity
        {
            DefId = item.DefId,
            ItemLevel = item.ItemLevel,
            Rarity = (int)item.Rarity,
            SellPrice = item.SellPrice,
            AffixesJson = JsonUtility.ToJson(new AffixListWrapper { Affixes = item.Affixes })
        };
        _conn.Insert(entity);
        Debug.Log($"💾 Insert into DB: {entity.DefId}, Lvl {entity.ItemLevel}, Rarity {entity.Rarity}");
    }

    public List<Scripts.Inventory.Item> LoadItems()
    {
        var results = new List<Scripts.Inventory.Item>();

        // B1: kiểm tra kết quả query từ SQLite
        var entities = _conn.Table<ItemEntity>().ToList();
        Debug.Log($"[DB] Loaded {entities.Count} entities from DB");

        foreach (var e in entities)
        {
            Debug.Log($"[DB] Entity: DefId={e.DefId}, Level={e.ItemLevel}, Rarity={e.Rarity}, Price={e.SellPrice}");

            // B2: lấy definition từ ItemDatabase
            var def = ItemDatabase.Instance.GetItemById(e.DefId);
            if (def == null)
            {
                Debug.LogWarning($"[DB] ItemDefinition not found for DefId={e.DefId}");
                continue;
            }

            // B3: parse affixes
            var affixesWrapper = JsonUtility.FromJson<AffixListWrapper>(e.AffixesJson);
            if (affixesWrapper == null)
            {
                Debug.LogWarning($"[DB] Failed to parse affixes JSON for DefId={e.DefId}, Json={e.AffixesJson}");
            }
            var affixes = affixesWrapper?.Affixes;

            // B4: stats từ definition
            var stats = def.GetStatsAtLevel(e.ItemLevel);
            if (stats.Equals(default(Stats)))
            {
                Debug.LogWarning($"[DB] Stats is null for DefId={e.DefId}, Level={e.ItemLevel}");
            }

            // B5: tạo object Item
            var item = new Scripts.Inventory.Item(
                def.Icon,
                e.DefId,
                e.ItemLevel,
                (Rarity)e.Rarity,
                stats,
                e.SellPrice,
                def,
                affixes
            );

            Debug.Log($"[DB] Created Item: {item.DefId}, Level={item.ItemLevel}, Rarity={item.Rarity}");

            results.Add(item);
        }

        Debug.Log($"[DB] Final loaded items count = {results.Count}");
        return results;
    }

}

[System.Serializable]
public class AffixListWrapper
{
    public List<Scripts.Inventory.AffixEntry> Affixes;
}
