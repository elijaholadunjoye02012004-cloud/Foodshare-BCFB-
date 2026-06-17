namespace FoodShare.Models;

public class DistributionItem
{
    public int DistributionItemId { get; set; }
    public int DistributionId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }

    public Distribution Distribution { get; set; } = null!;
    public FoodItem FoodItem { get; set; } = null!;
}
