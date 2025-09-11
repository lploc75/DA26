using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.Dependencies.Sqlite;

namespace Assets.Scripts.StatsSystem.SQLiteItem
{
    [Table("PlayerStats")]
    public class PlayerData
    {
        [PrimaryKey] // nếu muốn số nguyên tự tăng
        public string PlayerId { get; set; }

        // hoặc dùng string ID

        // Core
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentMana { get; set; }
        public int MaxMana { get; set; }
        public int CurrentHunger { get; set; }
        public int MaxHunger { get; set; }
        public int CurrentSanity { get; set; }
        public int MaxSanity { get; set; }
        public int CurrentStamina { get; set; }
        public int MaxStamina { get; set; }

        // Attributes
        public int STR { get; set; }
        public int AGI { get; set; }
        public int INT { get; set; }
        public int VIT { get; set; }
        public int WIS { get; set; }

        // Derived
        public int Armor { get; set; }
        public int BaseDamage { get; set; }
        public float CritChance { get; set; }
        public float CritDamage { get; set; }
        public float HealthRegen { get; set; }
        public float ManaRegen { get; set; }

        // Progression
        public int Level { get; set; }
        public int RemainPoints { get; set; }
    }

}
