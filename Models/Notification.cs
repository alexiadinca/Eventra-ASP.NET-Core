using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Message { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = "System";

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}