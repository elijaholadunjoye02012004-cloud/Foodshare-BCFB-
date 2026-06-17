namespace FoodShare.Models;

public class Inventory
{
    public int InventoryId { get; set; }
    public int ItemId { get; set; }
    public int QuantityAvailable { get; set; }
    public string? StorageLocation { get; set; }
    public DateOnly? EarliestExpiry { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public FoodItem FoodItem { get; set; } = null!;

    public string Status
    {
        get
        {
            if (QuantityAvailable <= 5) return "Low Stock";
            if (EarliestExpiry.HasValue)
            {
                var days = (EarliestExpiry.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;
                if (days <= 0) return "Expiring";
                if (days <= 7) return "Expiring Soon";
            }
            return "In Stock";
        }
    }
}
