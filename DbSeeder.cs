using FoodShare.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodShare.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        foreach (var role in new[] { "Administrator", "Volunteer", "Donor" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        await SeedSupplementalDataAsync(context, userManager);

        if (await context.Categories.AnyAsync())
            return;

        var categories = new[]
        {
            new Category { CategoryName = "Tinned Goods", Description = "Canned and jarred items" },
            new Category { CategoryName = "Dry Foods", Description = "Pasta, rice, cereals" },
            new Category { CategoryName = "Fresh Produce", Description = "Fruit and vegetables" },
            new Category { CategoryName = "Beverages", Description = "Drinks and juices" },
            new Category { CategoryName = "Hygiene Products", Description = "Toiletries and cleaning" },
            new Category { CategoryName = "Bakery", Description = "Bread and baked goods" },
            new Category { CategoryName = "Dairy", Description = "Milk and dairy products" },
            new Category { CategoryName = "Chilled Meat", Description = "Refrigerated meat products" }
        };
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        var cat = categories.ToDictionary(c => c.CategoryName, c => c.CategoryId);

        var foodItems = new List<FoodItem>
        {
            new() { Name = "Baked Beans (400g)", CategoryId = cat["Tinned Goods"], UnitOfMeasure = "Tins", AllergenInfo = "None" },
            new() { Name = "Fresh Milk (1L)", CategoryId = cat["Fresh Produce"], UnitOfMeasure = "Bottles", AllergenInfo = "Contains dairy" },
            new() { Name = "White Bread (800g)", CategoryId = cat["Bakery"], UnitOfMeasure = "Loaves", AllergenInfo = "Contains gluten" },
            new() { Name = "Pasta (500g)", CategoryId = cat["Dry Foods"], UnitOfMeasure = "Packets", AllergenInfo = "Contains gluten" },
            new() { Name = "Orange Juice (1L)", CategoryId = cat["Beverages"], UnitOfMeasure = "Cartons", AllergenInfo = "None" },
            new() { Name = "Toothpaste", CategoryId = cat["Hygiene Products"], UnitOfMeasure = "Tubes", AllergenInfo = "None" },
            new() { Name = "Yoghurt Multipack", CategoryId = cat["Dairy"], UnitOfMeasure = "Packs", AllergenInfo = "Contains dairy" },
            new() { Name = "Cooked Ham Slices", CategoryId = cat["Chilled Meat"], UnitOfMeasure = "Packs", AllergenInfo = "None" },
            new() { Name = "Tinned Tomatoes", CategoryId = cat["Tinned Goods"], UnitOfMeasure = "Tins", AllergenInfo = "None" }
        };
        context.FoodItems.AddRange(foodItems);
        await context.SaveChangesAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);
        context.Inventories.AddRange(new[]
        {
            new Inventory { ItemId = foodItems[0].ItemId, QuantityAvailable = 142, StorageLocation = "Shelf C1", EarliestExpiry = today.AddMonths(4) },
            new Inventory { ItemId = foodItems[1].ItemId, QuantityAvailable = 14, StorageLocation = "Fridge A2", EarliestExpiry = today.AddDays(1) },
            new Inventory { ItemId = foodItems[2].ItemId, QuantityAvailable = 9, StorageLocation = "Shelf B1", EarliestExpiry = today.AddDays(2) },
            new Inventory { ItemId = foodItems[3].ItemId, QuantityAvailable = 86, StorageLocation = "Shelf D3", EarliestExpiry = today.AddMonths(8) },
            new Inventory { ItemId = foodItems[4].ItemId, QuantityAvailable = 45, StorageLocation = "Shelf E1", EarliestExpiry = today.AddMonths(2) },
            new Inventory { ItemId = foodItems[5].ItemId, QuantityAvailable = 3, StorageLocation = "Shelf H2", EarliestExpiry = today.AddYears(1) },
            new Inventory { ItemId = foodItems[6].ItemId, QuantityAvailable = 6, StorageLocation = "Fridge A3", EarliestExpiry = today.AddDays(4) },
            new Inventory { ItemId = foodItems[7].ItemId, QuantityAvailable = 4, StorageLocation = "Fridge A1", EarliestExpiry = today.AddDays(5) },
            new Inventory { ItemId = foodItems[8].ItemId, QuantityAvailable = 18, StorageLocation = "Shelf C2", EarliestExpiry = today.AddDays(7) }
        });

        var admin = await CreateUserAsync(userManager, "Jane", "Doe", "admin@foodshare.local", "Admin123!", "Administrator");
        context.Volunteers.Add(new Volunteer { UserId = admin.Id, Availability = "Mon-Fri", Notes = "Administrator" });
        var volunteer1 = await CreateUserAsync(userManager, "Mark", "Smith", "volunteer@foodshare.local", "Volunteer123!", "Volunteer");
        var volunteer2 = await CreateUserAsync(userManager, "Mary", "Smith", "msmith@foodshare.local", "Volunteer123!", "Volunteer");
        var volunteer3 = await CreateUserAsync(userManager, "Asha", "Patel", "apatel@foodshare.local", "Volunteer123!", "Volunteer");
        var volunteer4 = await CreateUserAsync(userManager, "James", "Williams", "jwilliams@foodshare.local", "Volunteer123!", "Volunteer");

        var vol1 = new Volunteer { UserId = volunteer1.Id, Availability = "Mon-Fri" };
        var vol2 = new Volunteer { UserId = volunteer2.Id };
        var vol3 = new Volunteer { UserId = volunteer3.Id };
        var vol4 = new Volunteer { UserId = volunteer4.Id };
        context.Volunteers.AddRange(vol1, vol2, vol3, vol4);
        await context.SaveChangesAsync();

        var donorUsers = new[]
        {
            await CreateUserAsync(userManager, "Tesco", "Manager", "manager@tesco-aylesbury.co.uk", "Donor123!", "Donor"),
            await CreateUserAsync(userManager, "Rachel", "Brown", "r.brown@email.com", "Donor123!", "Donor"),
            await CreateUserAsync(userManager, "St Mary's", "Office", "office@stmarys-hw.org.uk", "Donor123!", "Donor"),
            await CreateUserAsync(userManager, "David", "King", "d.king@email.com", "Donor123!", "Donor")
        };

        var donors = new[]
        {
            new Donor { UserId = donorUsers[0].Id, DonorType = DonorType.Corporate, OrganisationName = "Tesco Superstore", BranchName = "Aylesbury Branch", AddressLine1 = "High Street", City = "Aylesbury", Postcode = "HP20 1ST" },
            new Donor { UserId = donorUsers[1].Id, DonorType = DonorType.Individual, AddressLine1 = "12 Oak Lane", City = "High Wycombe", Postcode = "HP11 2AB" },
            new Donor { UserId = donorUsers[2].Id, DonorType = DonorType.Corporate, OrganisationName = "St. Mary's Church", BranchName = "Collection", AddressLine1 = "Church Road", City = "High Wycombe", Postcode = "HP13 6XY" },
            new Donor { UserId = donorUsers[3].Id, DonorType = DonorType.Individual, AddressLine1 = "8 Maple Close", City = "Aylesbury", Postcode = "HP21 7CD" }
        };
        context.Donors.AddRange(donors);
        await context.SaveChangesAsync();

        context.Recipients.AddRange(
            new Recipient { ReferenceNumber = "BCFB-2026-0042", FirstName = "Sarah", LastName = "Johnson", Email = "s.johnson@email.com", Phone = "07700 900123", HouseholdSize = 4, DietaryRequirements = "Nut-free", ReferralSource = "Citizens Advice Bureau", RegistrationDate = new DateTime(2026, 1, 12) },
            new Recipient { ReferenceNumber = "BCFB-2026-0018", FirstName = "Ahmed", LastName = "Hassan", Email = "a.hassan@email.com", Phone = "07700 900456", HouseholdSize = 3, DietaryRequirements = "Halal", ReferralSource = "Social Services", RegistrationDate = new DateTime(2025, 11, 3) },
            new Recipient { ReferenceNumber = "BCFB-2026-0031", FirstName = "Emma", LastName = "Wilson", Phone = "07700 900789", HouseholdSize = 2, ReferralSource = "GP Surgery", RegistrationDate = new DateTime(2026, 2, 20) }
        );
        await context.SaveChangesAsync();

        var donations = new List<Donation>
        {
            new() { DonorId = donors[0].DonorId, LoggedByVolunteerId = vol2.VolunteerId, DateReceived = today.AddDays(-1), Notes = "Weekly delivery" },
            new() { DonorId = donors[1].DonorId, LoggedByVolunteerId = vol3.VolunteerId, DateReceived = today.AddDays(-2) },
            new() { DonorId = donors[2].DonorId, LoggedByVolunteerId = vol4.VolunteerId, DateReceived = today.AddDays(-3) },
            new() { DonorId = donors[3].DonorId, LoggedByVolunteerId = vol2.VolunteerId, DateReceived = today.AddDays(-4) }
        };
        context.Donations.AddRange(donations);
        await context.SaveChangesAsync();

        context.DonationItems.AddRange(
            new DonationItem { DonationId = donations[0].DonationId, ItemId = foodItems[0].ItemId, Quantity = 20 },
            new DonationItem { DonationId = donations[0].DonationId, ItemId = foodItems[4].ItemId, Quantity = 22 },
            new DonationItem { DonationId = donations[1].DonationId, ItemId = foodItems[2].ItemId, Quantity = 8 },
            new DonationItem { DonationId = donations[2].DonationId, ItemId = foodItems[0].ItemId, Quantity = 40 },
            new DonationItem { DonationId = donations[2].DonationId, ItemId = foodItems[3].ItemId, Quantity = 27 },
            new DonationItem { DonationId = donations[3].DonationId, ItemId = foodItems[5].ItemId, Quantity = 5 }
        );

        var firstRecipient = await context.Recipients.OrderBy(r => r.RecipientId).FirstAsync();
        var distribution = new Distribution
        {
            RecipientId = firstRecipient.RecipientId,
            VolunteerId = vol1.VolunteerId,
            DateDistributed = today.AddDays(-3)
        };
        context.Distributions.Add(distribution);
        await context.SaveChangesAsync();

        context.DistributionItems.AddRange(
            new DistributionItem { DistributionId = distribution.DistributionId, ItemId = foodItems[0].ItemId, Quantity = 2 },
            new DistributionItem { DistributionId = distribution.DistributionId, ItemId = foodItems[3].ItemId, Quantity = 1 }
        );

        var shiftBase = DateTime.Today.AddDays(1);
        while (shiftBase.DayOfWeek == DayOfWeek.Saturday || shiftBase.DayOfWeek == DayOfWeek.Sunday)
            shiftBase = shiftBase.AddDays(1);

        context.VolunteerShifts.AddRange(
            new VolunteerShift { VolunteerId = vol1.VolunteerId, StartTime = shiftBase.Date.AddHours(9), EndTime = shiftBase.Date.AddHours(13), Notes = "Morning shift" },
            new VolunteerShift { VolunteerId = vol2.VolunteerId, StartTime = shiftBase.Date.AddHours(13), EndTime = shiftBase.Date.AddHours(17), Notes = "Afternoon shift" }
        );

        context.CollectionSlots.AddRange(
            new CollectionSlot { StartTime = shiftBase.Date.AddHours(10), EndTime = shiftBase.Date.AddHours(10).AddMinutes(30), MaxBookings = 2 },
            new CollectionSlot { StartTime = shiftBase.Date.AddHours(11), EndTime = shiftBase.Date.AddHours(11).AddMinutes(30), MaxBookings = 2 },
            new CollectionSlot { StartTime = shiftBase.AddDays(1).Date.AddHours(10), EndTime = shiftBase.AddDays(1).Date.AddHours(10).AddMinutes(30), MaxBookings = 2 }
        );

        await context.SaveChangesAsync();
    }

    private static async Task SeedSupplementalDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var admin = await userManager.FindByEmailAsync("admin@foodshare.local");
        if (admin != null && !await context.Volunteers.AnyAsync(v => v.UserId == admin.Id))
        {
            context.Volunteers.Add(new Volunteer { UserId = admin.Id, Availability = "Mon-Fri", Notes = "Administrator" });
            await context.SaveChangesAsync();
        }

        if (!await context.CollectionSlots.AnyAsync())
        {
            var shiftBase = DateTime.Today.AddDays(1);
            while (shiftBase.DayOfWeek == DayOfWeek.Saturday || shiftBase.DayOfWeek == DayOfWeek.Sunday)
                shiftBase = shiftBase.AddDays(1);

            context.CollectionSlots.AddRange(
                new CollectionSlot { StartTime = shiftBase.Date.AddHours(10), EndTime = shiftBase.Date.AddHours(10).AddMinutes(30), MaxBookings = 2 },
                new CollectionSlot { StartTime = shiftBase.Date.AddHours(11), EndTime = shiftBase.Date.AddHours(11).AddMinutes(30), MaxBookings = 2 },
                new CollectionSlot { StartTime = shiftBase.AddDays(1).Date.AddHours(10), EndTime = shiftBase.AddDays(1).Date.AddHours(10).AddMinutes(30), MaxBookings = 2 }
            );
            await context.SaveChangesAsync();
        }
    }

    private static async Task<ApplicationUser> CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string firstName,
        string lastName,
        string email,
        string password,
        string role)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true,
            PhoneNumber = "07700900000"
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, role);
        return user;
    }
}
