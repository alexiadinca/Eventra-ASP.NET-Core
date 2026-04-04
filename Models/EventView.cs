using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class EventView
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public int? UserId { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string? Source { get; set; }

        public Event Event { get; set; } = null!;
        public User? User { get; set; }
    }
}