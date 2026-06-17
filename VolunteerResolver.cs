using FoodShare.Data;
using FoodShare.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodShare.Services;

public class VolunteerResolver
{
    private readonly ApplicationDbContext _db;

    public VolunteerResolver(ApplicationDbContext db) => _db = db;

    public async Task<Volunteer?> GetOrCreateForUserAsync(ApplicationUser user)
    {
        var volunteer = await _db.Volunteers.FirstOrDefaultAsync(v => v.UserId == user.Id);
        if (volunteer != null)
            return volunteer;

        volunteer = new Volunteer
        {
            UserId = user.Id,
            Availability = "Mon-Fri",
            Notes = "Auto-linked staff profile"
        };
        _db.Volunteers.Add(volunteer);
        await _db.SaveChangesAsync();
        return volunteer;
    }
}
