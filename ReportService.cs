using System.Globalization;
using System.Text;
using FoodShare.Data;
using FoodShare.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodShare.Services;

public class ReportService
{
    private readonly ApplicationDbContext _db;

    public ReportService(ApplicationDbContext db) => _db = db;

    public async Task<ReportSummary> GetSummaryAsync(DateOnly from, DateOnly to)
    {
        var donations = await _db.Donations
            .Include(d => d.Items)
            .Where(d => d.DateReceived >= from && d.DateReceived <= to)
            .ToListAsync();

        var distributions = await _db.Distributions
            .Where(d => d.DateDistributed >= from && d.DateDistributed <= to)
            .ToListAsync();

        var donorCounts = await _db.Donors
            .GroupBy(d => d.DonorType)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();

        var categoryTotals = await _db.DonationItems
            .Include(di => di.FoodItem).ThenInclude(f => f.Category)
            .Include(di => di.Donation)
            .Where(di => di.Donation.DateReceived >= from && di.Donation.DateReceived <= to)
            .GroupBy(di => di.FoodItem.Category.CategoryName)
            .Select(g => new CategoryTotal { Name = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToListAsync();

        var monthly = await _db.Donations
            .Where(d => d.DateReceived.Year == from.Year)
            .GroupBy(d => d.DateReceived.Month)
            .Select(g => new MonthlyTotal { Month = g.Key, Count = g.Count() })
            .OrderBy(m => m.Month)
            .ToListAsync();

        return new ReportSummary
        {
            From = from,
            To = to,
            TotalDonations = donations.Count,
            TotalItemsReceived = donations.Sum(d => d.TotalItems),
            ParcelsDistributed = distributions.Count,
            RecipientCount = await _db.Recipients.CountAsync(r => r.IsActive),
            CorporateDonors = donorCounts.FirstOrDefault(x => x.Key == DonorType.Corporate)?.Count ?? 0,
            IndividualDonors = donorCounts.FirstOrDefault(x => x.Key == DonorType.Individual)?.Count ?? 0,
            CategoryTotals = categoryTotals,
            MonthlyDonations = monthly
        };
    }

    public async Task<byte[]> ExportDonationsCsvAsync(DateOnly from, DateOnly to)
    {
        var rows = await _db.Donations
            .Include(d => d.Donor).ThenInclude(don => don.User)
            .Include(d => d.Items).ThenInclude(i => i.FoodItem)
            .Where(d => d.DateReceived >= from && d.DateReceived <= to)
            .OrderByDescending(d => d.DateReceived)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("DonationId,Date,Donor,Item,Quantity");
        foreach (var d in rows)
        {
            foreach (var item in d.Items)
            {
                sb.AppendLine(string.Join(",",
                    d.DonationId,
                    d.DateReceived.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Escape(d.Donor.DisplayName),
                    Escape(item.FoodItem.Name),
                    item.Quantity));
            }
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> ExportInventoryCsvAsync()
    {
        var rows = await _db.Inventories
            .Include(i => i.FoodItem).ThenInclude(f => f.Category)
            .OrderBy(i => i.FoodItem.Name)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Item,Category,Quantity,Unit,Location,EarliestExpiry,Status");
        foreach (var i in rows)
        {
            sb.AppendLine(string.Join(",",
                Escape(i.FoodItem.Name),
                Escape(i.FoodItem.Category.CategoryName),
                i.QuantityAvailable,
                Escape(i.FoodItem.UnitOfMeasure),
                Escape(i.StorageLocation ?? ""),
                i.EarliestExpiry?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "",
                Escape(i.Status)));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> ExportAnonymisedRecipientsCsvAsync()
    {
        var rows = await _db.Recipients.Where(r => r.IsActive).ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Reference,HouseholdSize,DietaryRequirements,ReferralSource,RegistrationDate");
        foreach (var r in rows)
        {
            sb.AppendLine(string.Join(",",
                Escape(r.ReferenceNumber),
                r.HouseholdSize,
                Escape(r.DietaryRequirements ?? "None"),
                Escape(r.ReferralSource),
                r.RegistrationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Escape(string value) =>
        value.Contains(',') || value.Contains('"') ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
}

public class ReportSummary
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public int TotalDonations { get; set; }
    public int TotalItemsReceived { get; set; }
    public int ParcelsDistributed { get; set; }
    public int RecipientCount { get; set; }
    public int CorporateDonors { get; set; }
    public int IndividualDonors { get; set; }
    public List<CategoryTotal> CategoryTotals { get; set; } = new();
    public List<MonthlyTotal> MonthlyDonations { get; set; } = new();
    public int ActiveDonors => CorporateDonors + IndividualDonors;
}

public class CategoryTotal
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class MonthlyTotal
{
    public int Month { get; set; }
    public int Count { get; set; }
    public string MonthName => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Month);
}
