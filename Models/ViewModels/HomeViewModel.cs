namespace Eventra.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Eventra.Models.Event> FeaturedEvents { get; set; } = new();
        public List<Eventra.Models.Review> LatestReviews { get; set; } = new();
    }
}