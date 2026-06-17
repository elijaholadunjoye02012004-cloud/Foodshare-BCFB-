using FoodShare.Data;
using FoodShare.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodShare.Services;

public class DistributionService
{
    private readonly ApplicationDbContext _db;
    private readonly DietarySafetyService _dietarySafety;

    public DistributionService(ApplicationDbContext db, DietarySafetyService dietarySafety)
    {
        _db = db;
        _dietarySafety = dietarySafety;
    }

    public async Task<(bool Success, string? Error, List<string> Warnings)> SaveDistributionAsync(
        Distribution distribution,
        IEnumerable<DistributionItem> items)
    {
        var warnings = new List<string>();
        var recipient = await _db.Recipients.FindAsync(distribution.RecipientId);
        if (recipient == null)
            return (false, "Recipient not found.", warnings);

        var itemList = items.ToList();
        foreach (var line in itemList)
        {
            var inventory = await _db.Inventories
                .Include(i => i.FoodItem)
                .FirstOrDefaultAsync(i => i.ItemId == line.ItemId);

            if (inventory == null || inventory.QuantityAvailable < line.Quantity)
            {
                var name = inventory?.FoodItem?.Name ?? "Item";
                return (false, $"Insufficient stock for {name}.", warnings);
            }

            warnings.AddRange(_dietarySafety.GetConflicts(recipient.DietaryRequirements, inventory.FoodItem));
        }

        distribution.Items = itemList;
        _db.Distributions.Add(distribution);
        await _db.SaveChangesAsync();

        foreach (var line in itemList)
        {
            var inventory = await _db.Inventories.FirstAsync(i => i.ItemId == line.ItemId);
            inventory.QuantityAvailable -= line.Quantity;
            inventory.LastUpdated = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return (true, null, warnings);
    }
}
