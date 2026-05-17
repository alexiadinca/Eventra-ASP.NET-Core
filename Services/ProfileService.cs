using Eventra.Models;
using Eventra.Models.ViewModels;
using Eventra.Repositories.Interfaces;
using Eventra.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IWaitingListRepository _waitingListRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IReviewRepository _reviewRepository;

        public ProfileService(
            IUserRepository userRepository,
            INotificationRepository notificationRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            IWaitingListRepository waitingListRepository,
            IEventRepository eventRepository,
            IReviewRepository reviewRepository)
        {
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
            _waitingListRepository = waitingListRepository;
            _eventRepository = eventRepository;
            _reviewRepository = reviewRepository;
        }

        public User? GetUser(int userId)
        {
            return _userRepository.GetById(userId);
        }

        public EditProfileViewModel CreateEditViewModel(User user)
        {
            return new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public bool EmailInUse(string email, int userId)
        {
            return _userRepository.EmailInUse(email, userId);
        }

        public void UpdateProfile(User user, EditProfileViewModel vm)
        {
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Email = vm.Email;
            user.PhoneNumber = vm.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            _userRepository.Save();
        }

        public List<Notification> GetNotifications(int userId)
        {
            return _notificationRepository
                .QueryByUser(userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToList();
        }

        public List<EventRegistration> GetFutureRegistrations(int userId)
        {
            return _eventRegistrationRepository
                .QueryByUserWithEvent(userId)
                .Where(r => r.Event.EventDate >= DateTime.Today)
                .OrderBy(r => r.Event.EventDate)
                .ToList();
        }

        public List<WaitingListEntry> GetWaitingList(int userId)
        {
            return _waitingListRepository
                .QueryByUserWithEvent(userId)
                .OrderBy(w => w.JoinedAt)
                .ToList();
        }

        public List<EventRegistration> GetPastRegistrations(int userId, int take)
        {
            return _eventRegistrationRepository
                .QueryByUserWithEvent(userId)
                .Where(r => r.Event.EventDate < DateTime.Today)
                .OrderByDescending(r => r.Event.EventDate)
                .Take(take)
                .ToList();
        }

        public bool HasReviewedEvent(int userId, int eventId)
        {
            return _reviewRepository.QueryWithUser().Any(r => r.UserId == userId && r.EventId == eventId);
        }

        public List<Event> GetOrganizedEvents(int organizerId)
        {
            return _eventRepository
                .QueryByOrganizer(organizerId)
                .OrderByDescending(e => e.EventDate)
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
