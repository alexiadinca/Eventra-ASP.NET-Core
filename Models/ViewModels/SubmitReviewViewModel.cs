using System.ComponentModel.DataAnnotations;

namespace Eventra.Models.ViewModels
{
    public class SubmitReviewViewModel
    {
        [Required]
        public int EventId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }
    }
}