using Eventra.Models;
using Eventra.Models.ViewModels;
using Eventra.Repositories.Interfaces;
using Eventra.Services.Interfaces;

namespace Eventra.Services
{
    public class ReviewService : IReviewService
    {
        private static readonly HashSet<string> BadWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "fuck", "shit", "bastard", "crap", "dumb", "stupid", "asshole",
            "cacat", "dracu", "prost", "proasta", "tampit", "idiot", "morti"
        };

        private static bool ContainsBadWords(string? text) =>
            !string.IsNullOrWhiteSpace(text) &&
            BadWords.Any(w => text.Contains(w, StringComparison.OrdinalIgnoreCase));

        private readonly IReviewRepository _reviewRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventRegistrationRepository _registrationRepository;

        public ReviewService(IReviewRepository reviewRepository, IEventRepository eventRepository, IEventRegistrationRepository registrationRepository)
        {
            _reviewRepository = reviewRepository;
            _eventRepository = eventRepository;
            _registrationRepository = registrationRepository;
        }

        public SubmitReviewViewModel CreateReviewViewModel(int eventId)
        {
            return new SubmitReviewViewModel
            {
                EventId = eventId
            };
        }

        public SubmitReviewViewModel? GetReviewEditViewModel(int eventId, int userId)
        {
            var review = _reviewRepository.QueryWithUser().FirstOrDefault(r => r.EventId == eventId && r.UserId == userId);

            if (review == null)
            {
                return null;
            }

            return new SubmitReviewViewModel
            {
                ReviewId = review.Id,
                EventId = eventId,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }

        public Event? GetEventForReview(int eventId)
        {
            return _eventRepository.QueryWithDetails().FirstOrDefault(e => e.Id == eventId);
        }

        public bool IsRegisteredForEvent(int userId, int eventId)
        {
            return _registrationRepository.IsRegisteredForEvent(userId, eventId);
        }

        public string SubmitReview(SubmitReviewViewModel vm, int userId)
        {
            if (!_registrationRepository.IsRegisteredForEvent(userId, vm.EventId))
                return "NotRegistered";

            var ev = _eventRepository.GetById(vm.EventId);
            if (ev == null)
                return "EventNotFound";

            bool flagged = ContainsBadWords(vm.Comment);

            if (vm.ReviewId.HasValue)
            {
                var existing = _reviewRepository.GetById(vm.ReviewId.Value);
                if (existing == null || existing.UserId != userId)
                    return "NotFound";

                existing.Rating = vm.Rating;
                existing.Comment = vm.Comment;
                existing.CreatedAt = DateTime.UtcNow;
                existing.IsFlagged = flagged;
                existing.IsApproved = !flagged;

                _reviewRepository.Update(existing);
                _reviewRepository.Save();
                return "Success";
            }

            var existingReview = _reviewRepository.QueryWithUser().FirstOrDefault(r => r.EventId == vm.EventId && r.UserId == userId);
            if (existingReview != null)
            {
                existingReview.Rating = vm.Rating;
                existingReview.Comment = vm.Comment;
                existingReview.CreatedAt = DateTime.UtcNow;
                existingReview.IsFlagged = flagged;
                existingReview.IsApproved = !flagged;
                _reviewRepository.Update(existingReview);
                _reviewRepository.Save();
                return "Success";
            }

            var review = new Review
            {
                UserId = userId,
                EventId = ev.Id,
                OrganizerId = ev.OrganizerId,
                Rating = vm.Rating,
                Comment = vm.Comment,
                CreatedAt = DateTime.UtcNow,
                IsFlagged = flagged,
                IsApproved = !flagged
            };

            _reviewRepository.Add(review);
            _reviewRepository.Save();
            return "Success";
        }

        public List<Review> GetLatestReviews(int count)
        {
            return _reviewRepository
                .QueryApproved()
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToList();
        }

        public List<Review> GetOrganizerReviews(int organizerId)
        {
            return _reviewRepository
                .QueryByOrganizerWithUser(organizerId)
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }
    }
}
