using System;
using UnityEngine;
using Assets.Scripts.StatsSystem.SQLiteItem;

namespace Assets.Scripts.Database
{
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager Instance { get; private set; }

        public CharacterStatsDB statsDB;
        public ItemDB itemDB;
        public PlayerStatsManager statsManager;
        //public InventoryManager inventoryManager;
        void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            itemDB = new ItemDB();
            statsDB = new CharacterStatsDB();
        }

        public void SaveAll()
        {
            if (statsManager != null && statsDB != null)
                statsDB.SavePlayer(statsManager.currentStats);

            // if (inventoryManager != null && itemDB != null)
            //     itemDB.SaveItems(inventoryManager.items);

            Debug.Log("💾 All data saved!");
        }

        public void LoadAll()
        {
            if (statsManager != null && statsDB != null)
            {
                var playerData = statsDB.LoadPlayer();
                if (playerData != null)
                    statsManager.ApplyDataToStats(playerData);
            }

            if (itemDB != null)
            {
                var items = itemDB.LoadItems();
                if (items != null)
                {
                    // inventoryManager.SetItems(items);
                }
            }

            Debug.Log("📥 All data loaded!");
        }
    }
}
