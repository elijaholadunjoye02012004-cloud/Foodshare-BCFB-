using FoodShare.Data;
using FoodShare.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodShare.Services;

public class DonationService
{
    private readonly ApplicationDbContext _db;

    public DonationService(ApplicationDbContext db) => _db = db;

    public async Task<Donation> LogDonationAsync(Donation donation, IEnumerable<DonationItem> items)
    {
        donation.Items = items.ToList();
        _db.Donations.Add(donation);
        await _db.SaveChangesAsync();

        foreach (var item in donation.Items)
        {
            var inventory = await _db.Inventories.FirstOrDefaultAsync(i => i.ItemId == item.ItemId);
            if (inventory == null)
            {
                inventory = new Inventory { ItemId = item.ItemId, QuantityAvailable = 0 };
                _db.Inventories.Add(inventory);
            }

            inventory.QuantityAvailable += item.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;
            if (item.ExpiryDate.HasValue &&
                (!inventory.EarliestExpiry.HasValue || item.ExpiryDate < inventory.EarliestExpiry))
            {
                inventory.EarliestExpiry = item.ExpiryDate;
            }
        }

        await _db.SaveChangesAsync();
        return donation;
    }
}
