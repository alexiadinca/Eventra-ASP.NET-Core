using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class EventRegistration
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int EventId { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Registered";

        [Required]
        [StringLength(200)]
        public string QrToken { get; set; } = Guid.NewGuid().ToString();

        public DateTime? CheckedInAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public User User { get; set; } = null!;
        public Event Event { get; set; } = null!;

        public ICollection<EventCheckIn> EventCheckIns { get; set; } = new List<EventCheckIn>();
    }
}