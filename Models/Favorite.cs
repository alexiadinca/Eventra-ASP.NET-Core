namespace Eventra.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int EventId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Event Event { get; set; } = null!;
    }
}