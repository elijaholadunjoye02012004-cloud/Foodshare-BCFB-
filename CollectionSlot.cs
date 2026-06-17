namespace FoodShare.Models;

public class CollectionSlot
{
    public int SlotId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxBookings { get; set; } = 2;
    public bool IsActive { get; set; } = true;

    public ICollection<CollectionBooking> Bookings { get; set; } = new List<CollectionBooking>();

    public int ConfirmedCount => Bookings.Count(b => b.Status == BookingStatus.Confirmed);

    public bool HasAvailability => IsActive && ConfirmedCount < MaxBookings;
}
