using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class WaitingListEntry
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int EventId { get; set; }

        public int Position { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Waiting";

        public User User { get; set; } = null!;
        public Event Event { get; set; } = null!;
    }
}