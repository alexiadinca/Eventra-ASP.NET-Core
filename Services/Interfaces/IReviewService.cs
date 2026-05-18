using Eventra.Models;
using Eventra.Models.ViewModels;

namespace Eventra.Services.Interfaces
{
    public interface IReviewService
    {
        SubmitReviewViewModel CreateReviewViewModel(int eventId);
        SubmitReviewViewModel? GetReviewEditViewModel(int eventId, int userId);
        Event? GetEventForReview(int eventId);
        bool IsRegisteredForEvent(int userId, int eventId);
        string SubmitReview(SubmitReviewViewModel vm, int userId);
        List<Review> GetLatestReviews(int count);
        List<Review> GetOrganizerReviews(int organizerId);
    }
}
