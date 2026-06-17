namespace FoodShare.Models;

public class Recipient
{
    public int RecipientId { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public int HouseholdSize { get; set; }
    public string? DietaryRequirements { get; set; }
    public string ReferralSource { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<Distribution> Distributions { get; set; } = new List<Distribution>();

    public string FullName => $"{FirstName} {LastName}".Trim();
}
