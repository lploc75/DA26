using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

namespace Assets.Scripts.StatsSystem.SQLiteItem
{
    public class CharacterStatsDB
    {
        private static SQLiteConnection _conn;
        public CharacterStatsDB(string dbName = "playerdata.db")
        {
            string dbPath = Path.Combine(Application.persistentDataPath, dbName);
            _conn = new SQLiteConnection(dbPath);
            _conn.CreateTable<PlayerData>();
            Debug.Log("✅ SQLite DB initialized at: " + dbPath);
        }

        public void SavePlayer(CharacterStats stats, string playerId = "default")
        {
            var data = new PlayerData
            {
                PlayerId = playerId,  // 🔑 ID người chơi

                // Core
                CurrentHealth = stats.CurrentHealth,
                MaxHealth = stats.MaxHealth,
                CurrentMana = stats.CurrentMana,
                MaxMana = stats.MaxMana,
                CurrentHunger = stats.CurrentHunger,
                MaxHunger = stats.MaxHunger,
                CurrentSanity = stats.CurrentSanity,
                MaxSanity = stats.MaxSanity,
                CurrentStamina = stats.CurrentStamina,
                MaxStamina = stats.MaxStamina,

                // Attributes
                STR = stats.STR,
                AGI = stats.AGI,
                INT = stats.INT,
                VIT = stats.VIT,
                WIS = stats.WIS,

                // Derived
                Armor = stats.Armor,
                BaseDamage = stats.BaseDamage,
                CritChance = stats.CritChance,
                CritDamage = stats.CritDamage,
                HealthRegen = stats.HealthRegen,
                ManaRegen = stats.ManaRegen,

                // Progression
                Level = stats.Level,
                RemainPoints = stats.RemainPoints
            };

            // Kiểm tra nếu player đã tồn tại
            var existing = _conn.Find<PlayerData>(playerId);
            if (existing != null)
            {
                _conn.Update(data); // update
                Debug.Log($"🔄 Player {playerId} stats updated!");
            }
            else
            {
                _conn.Insert(data); // insert mới
                Debug.Log($"✨ New player {playerId} stats saved!");
            }
        }

        public PlayerData LoadPlayer(string playerId = "default")
        {
            var data = _conn.Table<PlayerData>().FirstOrDefault(p => p.PlayerId == playerId);
            if (data != null)
                Debug.Log("📥 Player stats loaded from DB");
            return data;
        }
    }
}
