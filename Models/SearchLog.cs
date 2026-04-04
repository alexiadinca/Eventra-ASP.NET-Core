using System.ComponentModel.DataAnnotations;

namespace Eventra.Models
{
    public class SearchLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? CategoryId { get; set; }

        [Required]
        [StringLength(200)]
        public string QueryText { get; set; } = null!;

        [StringLength(100)]
        public string? City { get; set; }

        public DateTime SearchedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public Category? Category { get; set; }
    }
}