namespace FoodShare.Models;

public class Distribution
{
    public int DistributionId { get; set; }
    public int RecipientId { get; set; }
    public int VolunteerId { get; set; }
    public DateOnly DateDistributed { get; set; }
    public string? Notes { get; set; }

    public Recipient Recipient { get; set; } = null!;
    public Volunteer Volunteer { get; set; } = null!;
    public ICollection<DistributionItem> Items { get; set; } = new List<DistributionItem>();

    public int TotalItems => Items.Sum(i => i.Quantity);
}
