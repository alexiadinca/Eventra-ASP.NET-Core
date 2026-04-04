using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string? Description { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<SearchLog> SearchLogs { get; set; } = new List<SearchLog>();
    }
}