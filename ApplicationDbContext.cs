using FoodShare.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodShare.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Donor> Donors => Set<Donor>();
    public DbSet<Volunteer> Volunteers => Set<Volunteer>();
    public DbSet<Recipient> Recipients => Set<Recipient>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<FoodItem> FoodItems => Set<FoodItem>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<DonationItem> DonationItems => Set<DonationItem>();
    public DbSet<Distribution> Distributions => Set<Distribution>();
    public DbSet<DistributionItem> DistributionItems => Set<DistributionItem>();
    public DbSet<VolunteerShift> VolunteerShifts => Set<VolunteerShift>();
    public DbSet<CollectionSlot> CollectionSlots => Set<CollectionSlot>();
    public DbSet<CollectionBooking> CollectionBookings => Set<CollectionBooking>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<FoodItem>().HasKey(f => f.ItemId);
        builder.Entity<Category>().HasKey(c => c.CategoryId);
        builder.Entity<Donor>().HasKey(d => d.DonorId);
        builder.Entity<Volunteer>().HasKey(v => v.VolunteerId);
        builder.Entity<Recipient>().HasKey(r => r.RecipientId);
        builder.Entity<Inventory>().HasKey(i => i.InventoryId);
        builder.Entity<Donation>().HasKey(d => d.DonationId);
        builder.Entity<DonationItem>().HasKey(di => di.DonationItemId);
        builder.Entity<Distribution>().HasKey(d => d.DistributionId);
        builder.Entity<DistributionItem>().HasKey(di => di.DistributionItemId);

        builder.Entity<Donor>()
            .HasOne(d => d.User)
            .WithOne(u => u.DonorProfile)
            .HasForeignKey<Donor>(d => d.UserId);

        builder.Entity<Volunteer>()
            .HasOne(v => v.User)
            .WithOne(u => u.VolunteerProfile)
            .HasForeignKey<Volunteer>(v => v.UserId);

        builder.Entity<Recipient>()
            .HasIndex(r => r.ReferenceNumber)
            .IsUnique();

        builder.Entity<Category>()
            .HasIndex(c => c.CategoryName)
            .IsUnique();

        builder.Entity<Inventory>()
            .HasOne(i => i.FoodItem)
            .WithOne(f => f.Inventory)
            .HasForeignKey<Inventory>(i => i.ItemId);

        builder.Entity<DonationItem>()
            .HasOne(di => di.FoodItem)
            .WithMany(f => f.DonationItems)
            .HasForeignKey(di => di.ItemId);

        builder.Entity<DistributionItem>()
            .HasOne(di => di.FoodItem)
            .WithMany(f => f.DistributionItems)
            .HasForeignKey(di => di.ItemId);

        builder.Entity<Donation>()
            .HasOne(d => d.LoggedBy)
            .WithMany(v => v.LoggedDonations)
            .HasForeignKey(d => d.LoggedByVolunteerId);

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<VolunteerShift>().HasKey(s => s.ShiftId);
        builder.Entity<CollectionSlot>().HasKey(s => s.SlotId);
        builder.Entity<CollectionBooking>().HasKey(b => b.BookingId);

        builder.Entity<VolunteerShift>()
            .HasOne(s => s.Volunteer)
            .WithMany()
            .HasForeignKey(s => s.VolunteerId);

        builder.Entity<CollectionBooking>()
            .HasOne(b => b.Slot)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.SlotId);

        builder.Entity<CollectionBooking>()
            .HasOne(b => b.Recipient)
            .WithMany()
            .HasForeignKey(b => b.RecipientId);

        builder.Entity<Recipient>()
            .HasIndex(r => r.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL AND [Email] <> ''");

        builder.Entity<Recipient>()
            .HasIndex(r => r.Phone)
            .IsUnique();
    }
}
