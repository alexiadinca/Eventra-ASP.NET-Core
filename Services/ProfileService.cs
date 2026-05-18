using Eventra.Models;
using Eventra.Models.ViewModels;
using Eventra.Repositories.Interfaces;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IWebHostEnvironment _environment;

        public ProfileService(
            IUserRepository userRepository,
            INotificationRepository notificationRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            IWaitingListRepository waitingListRepository,
            IEventRepository eventRepository,
            IReviewRepository reviewRepository,
            IFavoriteRepository favoriteRepository,
            IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
            _waitingListRepository = waitingListRepository;
            _eventRepository = eventRepository;
            _reviewRepository = reviewRepository;
            _favoriteRepository = favoriteRepository;
            _environment = environment;
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

        public void UpdateProfile(User user, EditProfileViewModel vm, IFormFile? profilePhoto)
        {
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Email = vm.Email;
            user.PhoneNumber = vm.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            if (profilePhoto != null && profilePhoto.Length > 0)
            {
                var extension = Path.GetExtension(profilePhoto.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(extension))
                    throw new InvalidOperationException("Please upload a valid image (.jpg, .jpeg, .png, .webp).");

                if (profilePhoto.Length > 5 * 1024 * 1024)
                    throw new InvalidOperationException("Profile photo must be smaller than 5 MB.");

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profile-photos");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + extension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                profilePhoto.CopyTo(stream);

                user.ProfilePhotoPath = "/uploads/profile-photos/" + uniqueFileName;
            }

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

        public void SaveQrCodePath(User user, string path)
        {
            user.QrCodePath = path;
            _userRepository.Update(user);
            _userRepository.Save();
        }

        public HashSet<int> GetRegisteredEventIds(int userId)
        {
            return _eventRegistrationRepository
                .QueryByUserWithEvent(userId)
                .Select(r => r.EventId)
                .ToHashSet();
        }

        public HashSet<int> GetWaitingListEventIds(int userId)
        {
            return _waitingListRepository
                .QueryByUserWithEvent(userId)
                .Select(w => w.EventId)
                .ToHashSet();
        }

        public HashSet<int> GetCheckedInEventIds(int userId)
        {
            return _eventRegistrationRepository
                .QueryByUserWithEvent(userId)
                .Where(r => r.CheckedInAt != null)
                .Select(r => r.EventId)
                .ToHashSet();
        }

        public List<Eventra.Models.Favorite> GetFavorites(int userId)
        {
            return _favoriteRepository
                .QueryByUserWithEvent(userId)
                .ToList();
        }

        public List<Review> GetMyReviews(int userId)
        {
            return _reviewRepository
                .QueryByUserWithEvent(userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public string ChangePassword(User user, string currentPassword, string newPassword)
        {
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (result == PasswordVerificationResult.Failed)
                return "WrongPassword";

            user.PasswordHash = hasher.HashPassword(user, newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _userRepository.Update(user);
            _userRepository.Save();
            return "Success";
        }
    }
}
