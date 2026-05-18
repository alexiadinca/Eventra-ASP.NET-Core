namespace Eventra.Models.ViewModels
{
    public class EventDetailsViewModel
    {
        public Eventra.Models.Event Event { get; set; } = null!;
        public List<Eventra.Models.Event> SimilarEvents { get; set; } = new();
        public List<Eventra.Models.Review> OrganizerReviews { get; set; } = new();
        public bool IsLoggedIn { get; set; }
        public bool IsOrganizerOwner { get; set; }
        public bool HasRegistered { get; set; }
        public int? RegistrationId { get; set; }
        public bool HasReviewed { get; set; }
        public bool IsPastEvent { get; set; }
        public bool HasAvailableSeats { get; set; }
        public bool IsOnWaitingList { get; set; }
        public int? WaitingListPosition { get; set; }
        public int? WaitingListEntryId { get; set; }
        public bool IsFavorited { get; set; }
        public int FavoriteCount { get; set; }
        public bool IsAdmin { get; set; }
    }
}