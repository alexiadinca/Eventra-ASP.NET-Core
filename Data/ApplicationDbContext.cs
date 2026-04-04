using Eventra.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<OrganizerApprovalRequest> OrganizerApprovalRequests => Set<OrganizerApprovalRequest>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<EventTag> EventTags => Set<EventTag>();
        public DbSet<EventTagMapping> EventTagMappings => Set<EventTagMapping>();
        public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
        public DbSet<WaitingListEntry> WaitingListEntries => Set<WaitingListEntry>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<EventCheckIn> EventCheckIns => Set<EventCheckIn>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<EventView> EventViews => Set<EventView>();
        public DbSet<SearchLog> SearchLogs => Set<SearchLog>();
        public DbSet<NewsletterSubscription> NewsletterSubscriptions => Set<NewsletterSubscription>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<EventTag>()
                .HasIndex(t => t.Name)
                .IsUnique();

            modelBuilder.Entity<EventTagMapping>()
                .HasIndex(et => new { et.EventId, et.EventTagId })
                .IsUnique();

            modelBuilder.Entity<EventRegistration>()
                .HasIndex(er => er.QrToken)
                .IsUnique();

            modelBuilder.Entity<EventRegistration>()
                .HasIndex(er => new { er.UserId, er.EventId })
                .IsUnique();

            modelBuilder.Entity<WaitingListEntry>()
                .HasIndex(w => new { w.UserId, w.EventId })
                .IsUnique();

            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.EventId })
                .IsUnique();

            modelBuilder.Entity<OrganizerApprovalRequest>()
                .HasOne(o => o.User)
                .WithMany(u => u.OrganizerApprovalRequests)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizerApprovalRequest>()
                .HasOne(o => o.ReviewedByAdmin)
                .WithMany(u => u.ReviewedOrganizerApprovalRequests)
                .HasForeignKey(o => o.ReviewedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.ApprovedByAdmin)
                .WithMany(u => u.ApprovedEvents)
                .HasForeignKey(e => e.ApprovedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventTagMapping>()
                .HasOne(et => et.Event)
                .WithMany(e => e.EventTagMappings)
                .HasForeignKey(et => et.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventTagMapping>()
                .HasOne(et => et.EventTag)
                .WithMany(t => t.EventTagMappings)
                .HasForeignKey(et => et.EventTagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.User)
                .WithMany(u => u.EventRegistrations)
                .HasForeignKey(er => er.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventRegistration>()
                .HasOne(er => er.Event)
                .WithMany(e => e.EventRegistrations)
                .HasForeignKey(er => er.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WaitingListEntry>()
                .HasOne(w => w.User)
                .WithMany(u => u.WaitingListEntries)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WaitingListEntry>()
                .HasOne(w => w.Event)
                .WithMany(e => e.WaitingListEntries)
                .HasForeignKey(w => w.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventCheckIn>()
                .HasOne(ec => ec.EventRegistration)
                .WithMany(er => er.EventCheckIns)
                .HasForeignKey(ec => ec.EventRegistrationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventCheckIn>()
                .HasOne(ec => ec.ScannedByOrganizer)
                .WithMany(u => u.EventCheckInsPerformed)
                .HasForeignKey(ec => ec.ScannedByOrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Event)
                .WithMany(e => e.Favorites)
                .HasForeignKey(f => f.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.ReviewsWritten)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Reviews)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Organizer)
                .WithMany(u => u.OrganizerReviewsReceived)
                .HasForeignKey(r => r.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventView>()
                .HasOne(ev => ev.Event)
                .WithMany(e => e.EventViews)
                .HasForeignKey(ev => ev.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventView>()
                .HasOne(ev => ev.User)
                .WithMany(u => u.EventViews)
                .HasForeignKey(ev => ev.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SearchLog>()
                .HasOne(s => s.User)
                .WithMany(u => u.SearchLogs)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SearchLog>()
                .HasOne(s => s.Category)
                .WithMany(c => c.SearchLogs)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<NewsletterSubscription>()
                .HasOne(n => n.User)
                .WithMany(u => u.NewsletterSubscriptions)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Review>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Review_EventOrOrganizer",
                    "([EventId] IS NOT NULL OR [OrganizerId] IS NOT NULL)"));

            modelBuilder.Entity<Review>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Review_Rating",
                    "[Rating] >= 1 AND [Rating] <= 5"));

            modelBuilder.Entity<Event>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Event_AvailableSeats",
                    "[AvailableSeats] >= 0 AND [AvailableSeats] <= [Capacity]"));

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Conference", Description = "Business and professional conferences" },
                new Category { Id = 2, Name = "Exhibition", Description = "Art and cultural exhibitions" },
                new Category { Id = 3, Name = "Opening Event", Description = "Store, cafe, brand openings" },
                new Category { Id = 4, Name = "Food & Drinks", Description = "Tastings and culinary events" },
                new Category { Id = 5, Name = "Technology", Description = "Tech and AI events" },
                new Category { Id = 6, Name = "Market", Description = "Lifestyle and local creator markets" },
                new Category { Id = 7, Name = "Workshop", Description = "Hands-on activities and classes" },
                new Category { Id = 8, Name = "Other", Description = "Other event types" }
            );
        }
    }
}