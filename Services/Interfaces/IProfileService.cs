using Eventra.Models;
using Eventra.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Eventra.Services.Interfaces
{
    public interface IProfileService
    {
        User? GetUser(int userId);
        EditProfileViewModel CreateEditViewModel(User user);
        bool EmailInUse(string email, int userId);
        void UpdateProfile(User user, EditProfileViewModel vm, IFormFile? profilePhoto);
        List<Notification> GetNotifications(int userId);
        List<EventRegistration> GetFutureRegistrations(int userId);
        List<WaitingListEntry> GetWaitingList(int userId);
        List<EventRegistration> GetPastRegistrations(int userId, int take);
        bool HasReviewedEvent(int userId, int eventId);
        List<Event> GetOrganizedEvents(int organizerId);
        List<Review> GetOrganizerReviews(int organizerId);
        void SaveQrCodePath(User user, string path);
        HashSet<int> GetRegisteredEventIds(int userId);
        HashSet<int> GetWaitingListEventIds(int userId);
        HashSet<int> GetCheckedInEventIds(int userId);
        List<Eventra.Models.Favorite> GetFavorites(int userId);
        List<Review> GetMyReviews(int userId);
        string ChangePassword(User user, string currentPassword, string newPassword);
    }
}
