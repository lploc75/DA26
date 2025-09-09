using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Database
{
    using UnityEngine;
    using System.IO;
    using System.Linq;
    using Assets.Scripts.DTO;
    using Unity.VisualScripting.Dependencies.Sqlite;

    public class DatabasePlayerManager : MonoBehaviour
    {
        private static SQLiteConnection db;

        void Awake()
        {
            string dbPath = Path.Combine(Application.persistentDataPath, "playerdata.db");
            db = new SQLiteConnection(dbPath);
            db.CreateTable<PlayerData>();   
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
            var existing = db.Find<PlayerData>(playerId);
            if (existing != null)
            {
                db.Update(data); // update
                Debug.Log($"🔄 Player {playerId} stats updated!");
            }
            else
            {
                db.Insert(data); // insert mới
                Debug.Log($"✨ New player {playerId} stats saved!");
            }
        }

        public PlayerData LoadPlayer(string playerId = "default")
        {
            var data = db.Table<PlayerData>().FirstOrDefault(p => p.PlayerId == playerId);
            if (data != null)
                Debug.Log("📥 Player stats loaded from DB");
            return data;
        }
    }

}
