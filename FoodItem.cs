namespace FoodShare.Models;

public class FoodItem
{
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? AllergenInfo { get; set; }

    public Category Category { get; set; } = null!;
    public Inventory? Inventory { get; set; }
    public ICollection<DonationItem> DonationItems { get; set; } = new List<DonationItem>();
    public ICollection<DistributionItem> DistributionItems { get; set; } = new List<DistributionItem>();

    public bool HasAllergen(string keyword) =>
        !string.IsNullOrWhiteSpace(AllergenInfo) &&
        AllergenInfo.Contains(keyword, StringComparison.OrdinalIgnoreCase);
}
