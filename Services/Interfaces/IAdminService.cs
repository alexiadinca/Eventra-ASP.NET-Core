using Eventra.Models;

namespace Eventra.Services.Interfaces
{
    public interface IAdminService
    {
        // Dashboard counts
        int PendingOrganizersCount();
        int PendingEventsCount();
        int PendingReviewsCount();
        int FlaggedReviewsCount();

        // Organizers
        List<OrganizerApprovalRequest> GetPendingOrganizers();
        void ApproveOrganizer(int requestId, int adminId);
        void RejectOrganizer(int requestId, int adminId);

        // Events
        List<Event> GetPendingEvents();
        Event? GetEventById(int id);
        void ApproveEvent(int eventId, int adminId);
        void RejectEvent(int eventId);

        // Reviews
        List<Review> GetPendingReviews();
        List<Review> GetFlaggedReviews();
        void ApproveReview(int reviewId);
        void DeleteReview(int reviewId);

        // Admin profile
        User? GetAdmin(int adminId);
        bool EmailInUse(string email, int excludeUserId);
        void UpdateAdminEmail(int adminId, string newEmail);
        void UpdateAdminPassword(int adminId, string newPassword);
    }
}
