using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = null!;

        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [StringLength(255)]
        public string? ProfilePhotoPath { get; set; }

        [StringLength(255)]
        public string? QrCodePath { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Guest"; // Guest, Organizer, Admin

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<OrganizerApprovalRequest> OrganizerApprovalRequests { get; set; } = new List<OrganizerApprovalRequest>();
        public ICollection<OrganizerApprovalRequest> ReviewedOrganizerApprovalRequests { get; set; } = new List<OrganizerApprovalRequest>();
        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
        public ICollection<Event> ApprovedEvents { get; set; } = new List<Event>();
        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
        public ICollection<WaitingListEntry> WaitingListEntries { get; set; } = new List<WaitingListEntry>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<EventCheckIn> EventCheckInsPerformed { get; set; } = new List<EventCheckIn>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();
        public ICollection<Review> OrganizerReviewsReceived { get; set; } = new List<Review>();
        public ICollection<EventView> EventViews { get; set; } = new List<EventView>();
        public ICollection<SearchLog> SearchLogs { get; set; } = new List<SearchLog>();
        public ICollection<NewsletterSubscription> NewsletterSubscriptions { get; set; } = new List<NewsletterSubscription>();
    }
}