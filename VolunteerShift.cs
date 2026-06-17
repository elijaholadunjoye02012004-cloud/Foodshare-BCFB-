namespace FoodShare.Models;

public class VolunteerShift
{
    public int ShiftId { get; set; }
    public int VolunteerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }

    public Volunteer Volunteer { get; set; } = null!;
}
