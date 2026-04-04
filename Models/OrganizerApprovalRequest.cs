using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class OrganizerApprovalRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [StringLength(150)]
        public string? BusinessName { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string? BusinessEmail { get; set; }

        [StringLength(30)]
        public string? BusinessPhone { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedByAdminId { get; set; }

        [StringLength(500)]
        public string? ReviewNotes { get; set; }

        public User User { get; set; } = null!;
        public User? ReviewedByAdmin { get; set; }
    }
}