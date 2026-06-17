namespace FoodShare.Models;

public class Donor
{
    public int DonorId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DonorType DonorType { get; set; }
    public string? OrganisationName { get; set; }
    public string? BranchName { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public string DisplayName =>
        DonorType == DonorType.Corporate && !string.IsNullOrWhiteSpace(OrganisationName)
            ? OrganisationName
            : User?.FullName ?? "Unknown";

    public string Subtitle =>
        DonorType == DonorType.Corporate ? BranchName ?? City : City;
}
