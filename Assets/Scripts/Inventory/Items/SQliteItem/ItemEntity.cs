using Unity.VisualScripting.Dependencies.Sqlite;

public class ItemEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string DefId { get; set; }
    public int ItemLevel { get; set; }
    public int Rarity { get; set; }
    public int SellPrice { get; set; }

    // Bạn có thể serialize affix thành JSON
    public string AffixesJson { get; set; }
}
