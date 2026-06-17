namespace FoodShare.Models;

public enum BookingStatus
{
    Confirmed,
    Cancelled
}

public class CollectionBooking
{
    public int BookingId { get; set; }
    public int SlotId { get; set; }
    public int RecipientId { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.UtcNow;
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

    public CollectionSlot Slot { get; set; } = null!;
    public Recipient Recipient { get; set; } = null!;
}
