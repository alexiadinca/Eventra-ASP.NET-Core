using Eventra.Models;
using Eventra.Repositories.Interfaces;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Eventra.Data;

namespace Eventra.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AdminService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
            _passwordHasher = new PasswordHasher<User>();
        }

        public int PendingOrganizersCount() =>
            _context.OrganizerApprovalRequests.Count(r => r.Status == "Pending");

        public int PendingEventsCount() =>
            _context.Events.Count(e => e.Status == "PendingApproval");

        public int PendingReviewsCount() =>
            _context.Reviews.Count(r => !r.IsApproved && !r.IsFlagged);

        public int FlaggedReviewsCount() =>
            _context.Reviews.Count(r => r.IsFlagged);

        public List<OrganizerApprovalRequest> GetPendingOrganizers() =>
            _context.OrganizerApprovalRequests
                .Include(r => r.User)
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.RequestedAt)
                .ToList();

        public void ApproveOrganizer(int requestId, int adminId)
        {
            var request = _context.OrganizerApprovalRequests.Include(r => r.User).FirstOrDefault(r => r.Id == requestId);
            if (request == null) return;

            request.Status = "Approved";
            request.ReviewedAt = DateTime.UtcNow;
            request.ReviewedByAdminId = adminId;
            request.User.IsApproved = true;
            _context.SaveChanges();
        }

        public void RejectOrganizer(int requestId, int adminId)
        {
            var request = _context.OrganizerApprovalRequests.Include(r => r.User).FirstOrDefault(r => r.Id == requestId);
            if (request == null) return;

            request.Status = "Rejected";
            request.ReviewedAt = DateTime.UtcNow;
            request.ReviewedByAdminId = adminId;
            _context.SaveChanges();
        }

        public List<Event> GetPendingEvents() =>
            _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .Where(e => e.Status == "PendingApproval")
                .OrderBy(e => e.CreatedAt)
                .ToList();

        public Event? GetEventById(int id) =>
            _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .FirstOrDefault(e => e.Id == id);

        public void ApproveEvent(int eventId, int adminId)
        {
            var ev = _context.Events.Find(eventId);
            if (ev == null) return;

            ev.Status = "Approved";
            ev.ApprovedAt = DateTime.UtcNow;
            ev.ApprovedByAdminId = adminId;
            _context.SaveChanges();

            // Notify the organizer
            _notificationService.Create(
                ev.OrganizerId,
                "Event Approved",
                $"Your event \"{ev.Title}\" was approved by the admin.",
                "EventApproved",
                ev.Id);

            // Notify guests who previously attended this organizer's events
            var organizerName = ev.OrganizerDisplayName ?? "An organizer";
            var guestIds = (
                from reg in _context.EventRegistrations
                join pastEvent in _context.Events on reg.EventId equals pastEvent.Id
                where pastEvent.OrganizerId == ev.OrganizerId && pastEvent.Id != ev.Id
                select reg.UserId
            ).Distinct()
             .Where(uid => uid != ev.OrganizerId)
             .ToList();

            foreach (var guestId in guestIds)
            {
                if (!_notificationService.Exists(guestId, ev.Id, "NewEventFromOrganizer"))
                {
                    _notificationService.Create(
                        guestId,
                        "New Event Published",
                        $"{organizerName} published a new event: \"{ev.Title}\".",
                        "NewEventFromOrganizer",
                        ev.Id);
                }
            }
        }

        public void RejectEvent(int eventId)
        {
            var ev = _context.Events.Find(eventId);
            if (ev == null) return;

            int organizerId = ev.OrganizerId;
            string title = ev.Title;

            _context.Events.Remove(ev);
            _context.SaveChanges();

            _notificationService.Create(
                organizerId,
                "Event Rejected",
                $"Your event \"{title}\" was reviewed and rejected by the admin and has been removed from the platform.",
                "EventRejected");
        }

        public List<Review> GetPendingReviews() =>
            _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Event)
                .Where(r => !r.IsApproved && !r.IsFlagged)
                .OrderBy(r => r.CreatedAt)
                .ToList();

        public List<Review> GetFlaggedReviews() =>
            _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Event)
                .Where(r => r.IsFlagged)
                .OrderBy(r => r.CreatedAt)
                .ToList();

        public void ApproveReview(int reviewId)
        {
            var review = _context.Reviews.Find(reviewId);
            if (review == null) return;

            review.IsApproved = true;
            review.IsFlagged = false;
            _context.SaveChanges();
        }

        public void DeleteReview(int reviewId)
        {
            var review = _context.Reviews.Find(reviewId);
            if (review == null) return;

            _context.Reviews.Remove(review);
            _context.SaveChanges();
        }

        public User? GetAdmin(int adminId) =>
            _context.Users.Find(adminId);

        public bool EmailInUse(string email, int excludeUserId) =>
            _context.Users.Any(u => u.Email == email && u.Id != excludeUserId);

        public void UpdateAdminEmail(int adminId, string newEmail)
        {
            var admin = _context.Users.Find(adminId);
            if (admin == null) return;

            admin.Email = newEmail;
            admin.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }

        public void UpdateAdminPassword(int adminId, string newPassword)
        {
            var admin = _context.Users.Find(adminId);
            if (admin == null) return;

            admin.PasswordHash = _passwordHasher.HashPassword(admin, newPassword);
            admin.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}
