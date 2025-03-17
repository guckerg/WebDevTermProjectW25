using EventManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration>? EventRegistrations { get; set; }
    public DbSet<Match>? Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Match>()
            .HasOne(m => m.Event) // Match references Event
            .WithMany(e => e.Matches) // Event has many Matches
            .HasForeignKey(m => m.EventID) // Foreign key in Match
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete if Event is deleted

        modelBuilder.Entity<Match>()
                .HasOne(m => m.Player1)
                .WithMany()
                .HasForeignKey("Player1Id") // Define foreign key in Match
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascading delete for users

        modelBuilder.Entity<Match>()
                .HasOne(m => m.Player2)
                .WithMany()
                .HasForeignKey("Player2Id")
                .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
                .HasOne(m => m.Winner)
                .WithMany()
                .HasForeignKey("WinnerId")
                .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EventRegistration>()
                .HasKey(er => er.RegistrationID);

        modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.User) // EventRegistration references AppUser
                .WithMany() // AppUser can have multiple registrations
                .HasForeignKey(er => er.UserID)
                .OnDelete(DeleteBehavior.Cascade); // Delete registrations if user is deleted

        modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.Event) // EventRegistration references Event
                .WithMany(e => e.Registrations) // Event can have multiple registrations
                .HasForeignKey(er => er.EventID)
                .OnDelete(DeleteBehavior.Cascade); // Delete registrations if Event is deleted

        // Seed Events
        modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    EventID = 1,
                    EventTitle = "Flesh and Blood CC Armory",
                    EventDate = new DateTime(2025, 3, 17),
                    Game = "Flesh and Blood",
                    MaxParticipants = 24,
                    IsLive = true
                },
                new Event
                {
                    EventID = 2,
                    EventTitle = "DigimonTCG BT20",
                    EventDate = new DateTime(2025, 5, 10),
                    Game = "DigimonTCG",
                    MaxParticipants = 16,
                    IsLive = false
                },
                new Event
                {
                    EventID = 3,
                    EventTitle = "MTG Bloomburrow Draft",
                    EventDate = new DateTime(2025, 6, 11),
                    Game = "Magic: The Gathering",
                    MaxParticipants = 50,
                    IsLive = false
                },
                new Event
                {
                    EventID = 4,
                    EventTitle = "Pokemon: Standard",
                    EventDate = new DateTime(2025, 3, 26),
                    Game = "Magic: The Gathering",
                    MaxParticipants = 50,
                    IsLive = false
                }
            );
    }
}