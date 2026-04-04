using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class NewsletterSubscription
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public int? UserId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}