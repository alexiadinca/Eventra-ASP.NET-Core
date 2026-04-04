using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventra.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = null!;

        [StringLength(255)]
        public string? AddressLine { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }

        public int AvailableSeats { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 999999.99, ErrorMessage = "Invalid price.")]
        public decimal? Price { get; set; }

        [StringLength(10)]
        public string? Currency { get; set; }

        public bool IsFreeEntry { get; set; }

        [StringLength(255)]
        public string? ImagePath { get; set; }

        [StringLength(150)]
        public string? OrganizerDisplayName { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string? SupportEmail { get; set; }

        [StringLength(30)]
        public string? SupportPhone { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        public int CategoryId { get; set; }

        public int OrganizerId { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "PendingApproval";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public int? ApprovedByAdminId { get; set; }

        public Category Category { get; set; } = null!;
        public User Organizer { get; set; } = null!;
        public User? ApprovedByAdmin { get; set; }

        public ICollection<EventTagMapping> EventTagMappings { get; set; } = new List<EventTagMapping>();
        public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
        public ICollection<WaitingListEntry> WaitingListEntries { get; set; } = new List<WaitingListEntry>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<EventView> EventViews { get; set; } = new List<EventView>();
    }
}