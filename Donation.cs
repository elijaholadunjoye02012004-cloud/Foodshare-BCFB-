namespace FoodShare.Models;

public class Donation
{
    public int DonationId { get; set; }
    public int DonorId { get; set; }
    public int LoggedByVolunteerId { get; set; }
    public DateOnly DateReceived { get; set; }
    public string? Notes { get; set; }

    public Donor Donor { get; set; } = null!;
    public Volunteer LoggedBy { get; set; } = null!;
    public ICollection<DonationItem> Items { get; set; } = new List<DonationItem>();

    public int TotalItems => Items.Sum(i => i.Quantity);
}
