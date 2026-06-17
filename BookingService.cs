using FoodShare.Data;
using FoodShare.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodShare.Services;

public class BookingService
{
    private readonly ApplicationDbContext _db;

    public BookingService(ApplicationDbContext db) => _db = db;

    public async Task<List<CollectionSlot>> GetAvailableSlotsAsync(DateTime from, DateTime to) =>
        await _db.CollectionSlots
            .Include(s => s.Bookings)
            .Where(s => s.IsActive && s.StartTime >= from && s.StartTime <= to)
            .OrderBy(s => s.StartTime)
            .ToListAsync();

    public async Task<(bool Success, string? Error)> BookSlotAsync(int slotId, int recipientId)
    {
        var slot = await _db.CollectionSlots
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.SlotId == slotId);

        if (slot == null || !slot.IsActive)
            return (false, "This time slot is no longer available.");

        if (slot.ConfirmedCount >= slot.MaxBookings)
            return (false, "This time slot is fully booked.");

        var recipient = await _db.Recipients.FindAsync(recipientId);
        if (recipient == null || !recipient.IsActive)
            return (false, "Recipient not found or inactive.");

        var existing = await _db.CollectionBookings
            .Include(b => b.Slot)
            .AnyAsync(b =>
                b.RecipientId == recipientId &&
                b.Status == BookingStatus.Confirmed &&
                b.Slot.StartTime.Date == slot.StartTime.Date);

        if (existing)
            return (false, "You already have a collection booked for this day.");

        _db.CollectionBookings.Add(new CollectionBooking
        {
            SlotId = slotId,
            RecipientId = recipientId,
            BookedAt = DateTime.UtcNow,
            Status = BookingStatus.Confirmed
        });
        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<Recipient?> VerifyRecipientAsync(string referenceNumber, string lastName)
    {
        var term = referenceNumber.Trim();
        var name = lastName.Trim();
        return await _db.Recipients.FirstOrDefaultAsync(r =>
            r.IsActive &&
            r.ReferenceNumber == term &&
            r.LastName.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
