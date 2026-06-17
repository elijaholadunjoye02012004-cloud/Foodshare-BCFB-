namespace FoodShare.Models;

public class DonationItem
{
    public int DonationItemId { get; set; }
    public int DonationId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public DateOnly? ExpiryDate { get; set; }

    public Donation Donation { get; set; } = null!;
    public FoodItem FoodItem { get; set; } = null!;
}
