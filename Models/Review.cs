using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int? EventId { get; set; }

        public int? OrganizerId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; } = true;

        public User User { get; set; } = null!;
        public Event? Event { get; set; }
        public User? Organizer { get; set; }
    }
}