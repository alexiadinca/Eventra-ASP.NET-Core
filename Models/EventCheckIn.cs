using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class EventCheckIn
    {
        public int Id { get; set; }

        public int EventRegistrationId { get; set; }

        public int ScannedByOrganizerId { get; set; }

        public DateTime ScannedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Result { get; set; } = "Valid";

        public EventRegistration EventRegistration { get; set; } = null!;
        public User ScannedByOrganizer { get; set; } = null!;
    }
}