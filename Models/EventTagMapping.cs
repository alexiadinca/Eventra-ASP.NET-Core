namespace Eventra.Models
{
    public class EventTagMapping
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public int EventTagId { get; set; }

        public Event Event { get; set; } = null!;
        public EventTag EventTag { get; set; } = null!;
    }
}