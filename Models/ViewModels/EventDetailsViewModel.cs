namespace Eventra.Models.ViewModels
{
    public class EventDetailsViewModel
    {
        public Eventra.Models.Event Event { get; set; } = null!;
        public List<Eventra.Models.Event> SimilarEvents { get; set; } = new();
        public List<Eventra.Models.Review> OrganizerReviews { get; set; } = new();
    }
}