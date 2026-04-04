using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class EventTag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public ICollection<EventTagMapping> EventTagMappings { get; set; } = new List<EventTagMapping>();
    }
}