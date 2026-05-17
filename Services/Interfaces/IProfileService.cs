using Eventra.Models;
using Eventra.Models.ViewModels;

namespace Eventra.Services.Interfaces
{
    public interface IProfileService
    {
        User? GetUser(int userId);
        EditProfileViewModel CreateEditViewModel(User user);
        bool EmailInUse(string email, int userId);
        void UpdateProfile(User user, EditProfileViewModel vm);
        List<Notification> GetNotifications(int userId);
        List<EventRegistration> GetFutureRegistrations(int userId);
        List<WaitingListEntry> GetWaitingList(int userId);
        List<EventRegistration> GetPastRegistrations(int userId, int take);
        bool HasReviewedEvent(int userId, int eventId);
        List<Event> GetOrganizedEvents(int organizerId);
        List<Review> GetOrganizerReviews(int organizerId);
    }
}
