namespace FoodShare.Models;

public class Volunteer
{
    public int VolunteerId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? Availability { get; set; }
    public DateOnly? DbsCheckDate { get; set; }
    public string? Notes { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ICollection<Donation> LoggedDonations { get; set; } = new List<Donation>();
    public ICollection<Distribution> Distributions { get; set; } = new List<Distribution>();
}
