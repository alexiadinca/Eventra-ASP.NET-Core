using Eventra.Models;
using Eventra.Models.ViewModels;
using Eventra.Repositories.Interfaces;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Eventra.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IEventRegistrationRepository _eventRegistrationRepository;
        private readonly IWaitingListRepository _waitingListRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;

        public EventService(
            IEventRepository eventRepository,
            ICategoryRepository categoryRepository,
            IReviewRepository reviewRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            IWaitingListRepository waitingListRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment environment,
            INotificationService notificationService)
        {
            _eventRepository = eventRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
            _eventRegistrationRepository = eventRegistrationRepository;
            _waitingListRepository = waitingListRepository;
            _userRepository = userRepository;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
        }

        public List<Event> GetEvents(int? categoryId, string? city, string? search, string? filter)
        {
            var query = _eventRepository.QueryWithCategoryAndOrganizer()
                .Where(e => e.Status == "Approved");

            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(e => e.City.Contains(city));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.Title.Contains(search) ||
                    e.City.Contains(search) ||
                    e.Location.Contains(search) ||
                    (e.OrganizerDisplayName != null && e.OrganizerDisplayName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var today = DateTime.Today;
                switch (filter)
                {
                    case "Today":
                        query = query.Where(e => e.EventDate.Date == today);
                        break;
                    case "Tomorrow":
                        query = query.Where(e => e.EventDate.Date == today.AddDays(1));
                        break;
                    case "NextWeek":
                        query = query.Where(e => e.EventDate.Date >= today && e.EventDate.Date < today.AddDays(7));
                        break;
                    case "NextMonth":
                        query = query.Where(e => e.EventDate.Date >= today && e.EventDate.Date < today.AddMonths(1));
                        break;
                    case "FreeEntry":
                        query = query.Where(e => e.IsFreeEntry);
                        break;
                    case "Paid":
                        query = query.Where(e => !e.IsFreeEntry);
                        break;
                }
            }

            return query.OrderBy(e => e.EventDate).ToList();
        }

        public EventDetailsViewModel? GetEventDetails(int id)
        {
            var ev = _eventRepository.GetByIdWithDetails(id);

            if (ev == null)
            {
                return null;
            }

            var similarEvents = _eventRepository
                .QueryWithCategoryAndOrganizer()
                .Where(e => e.Id != id && e.CategoryId == ev.CategoryId && e.Status == "Approved")
                .OrderByDescending(e => e.CreatedAt)
                .Take(3)
                .ToList();

            var organizerReviews = _reviewRepository
                .QueryByOrganizerWithUser(ev.OrganizerId)
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Take(6)
                .ToList();

            return new EventDetailsViewModel
            {
                Event = ev,
                SimilarEvents = similarEvents,
                OrganizerReviews = organizerReviews,
                IsLoggedIn = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId").HasValue == true,
                IsOrganizerOwner = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId") == ev.OrganizerId,
                HasRegistered = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId") is int uid && _eventRegistrationRepository.QueryByUserWithEvent(uid).Any(r => r.EventId == id),
                HasReviewed = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId") is int reviewerId && _reviewRepository.QueryWithUser().Any(r => r.EventId == id && r.UserId == reviewerId),
                IsPastEvent = ev.EventDate.Date < DateTime.Today,
                HasAvailableSeats = ev.AvailableSeats > 0
            };
        }

        public Event CreateEvent(Event model, IFormFile? imageFile, int organizerId, string? role)
        {
            if (role != "Organizer")
            {
                throw new InvalidOperationException("Only organizers can create events.");
            }

            model.OrganizerId = organizerId;
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;
            model.Status = "PendingApproval";
            model.AvailableSeats = model.Capacity;

            var organizer = _userRepository.GetById(organizerId);
            model.OrganizerDisplayName = organizer != null
                ? $"{organizer.FirstName} {organizer.LastName}".Trim()
                : null;

            if (model.IsFreeEntry)
            {
                model.Price = 0;
                model.Currency = "RON";
            }
            else if (string.IsNullOrWhiteSpace(model.Currency))
            {
                model.Currency = "RON";
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(extension))
                {
                    throw new InvalidOperationException("Please upload a valid image (.jpg, .jpeg, .png, .webp).");
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "events");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid() + extension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                model.ImagePath = "/uploads/events/" + uniqueFileName;
            }

            _eventRepository.Add(model);
            _eventRepository.Save();

            return model;
        }

        public List<Category> GetCategories()
        {
            return _categoryRepository.QueryAll().ToList();
        }

        public List<Event> GetOrganizedEvents(int organizerId)
        {
            return _eventRepository
                .QueryByOrganizer(organizerId)
                .OrderByDescending(e => e.EventDate)
                .ToList();
        }

        public Event? GetEventForOrganizer(int eventId, int organizerId)
        {
            return _eventRepository.QueryWithDetails().FirstOrDefault(e => e.Id == eventId && e.OrganizerId == organizerId);
        }

        public void UpdateEvent(Event model, IFormFile? imageFile)
        {
            var existing = _eventRepository.GetById(model.Id);
            if (existing == null)
            {
                throw new InvalidOperationException("Event not found.");
            }

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.EventDate = model.EventDate;
            existing.StartTime = model.StartTime;
            existing.EndTime = model.EndTime;
            existing.City = model.City;
            existing.Location = model.Location;
            existing.AddressLine = model.AddressLine;
            existing.Capacity = model.Capacity;
            existing.Price = model.IsFreeEntry ? 0 : model.Price;
            existing.Currency = model.IsFreeEntry ? "RON" : model.Currency;
            existing.IsFreeEntry = model.IsFreeEntry;
            // OrganizerDisplayName is not editable from the form — preserve the original value
            existing.SupportEmail = model.SupportEmail;
            existing.SupportPhone = model.SupportPhone;
            existing.CategoryId = model.CategoryId;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.AvailableSeats = Math.Min(existing.AvailableSeats, existing.Capacity);
            existing.Status = "PendingApproval";

            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(extension))
                {
                    throw new InvalidOperationException("Please upload a valid image (.jpg, .jpeg, .png, .webp).");
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "events");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid() + extension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                imageFile.CopyTo(stream);

                existing.ImagePath = "/uploads/events/" + uniqueFileName;
            }

            _eventRepository.Update(existing);
            _eventRepository.Save();
        }

        public void DeleteEvent(int eventId, int organizerId)
        {
            var existing = _eventRepository.QueryByOrganizer(organizerId).FirstOrDefault(e => e.Id == eventId);
            if (existing == null)
            {
                throw new InvalidOperationException("Event not found.");
            }

            var title = existing.Title;

            _eventRepository.Remove(existing);
            _eventRepository.Save();

            _notificationService.Create(
                organizerId,
                "Event Deleted",
                $"Your event \"{title}\" was successfully deleted.",
                "EventDeleted");
        }

        public string RegisterForEvent(int eventId, int userId)
        {
            var ev = _eventRepository.GetByIdWithDetails(eventId);
            if (ev == null)
            {
                throw new InvalidOperationException("Event not found.");
            }

            if (ev.OrganizerId == userId)
            {
                return "OrganizerOwner";
            }

            if (ev.EventDate.Date < DateTime.Today)
            {
                return "PastEvent";
            }

            if (_eventRegistrationRepository.QueryByUserWithEvent(userId).Any(r => r.EventId == eventId))
            {
                return "AlreadyRegistered";
            }

            if (_waitingListRepository.QueryByUserWithEvent(userId).Any(w => w.EventId == eventId))
            {
                return "AlreadyWaiting";
            }

            if (ev.AvailableSeats > 0)
            {
                var registration = new EventRegistration
                {
                    UserId = userId,
                    EventId = eventId,
                    RegisteredAt = DateTime.UtcNow,
                    Status = "Registered",
                    QrToken = Guid.NewGuid().ToString()
                };

                ev.AvailableSeats--;
                _eventRegistrationRepository.Add(registration);
                _eventRepository.Update(ev);
                _eventRegistrationRepository.Save();
                _eventRepository.Save();
                return "Registered";
            }

            var lastPosition = _waitingListRepository.QueryWithEvent().Where(w => w.EventId == eventId).Select(w => (int?)w.Position).Max() ?? 0;
            var waiting = new WaitingListEntry
            {
                UserId = userId,
                EventId = eventId,
                Position = lastPosition + 1,
                JoinedAt = DateTime.UtcNow,
                Status = "Waiting"
            };

            _waitingListRepository.Add(waiting);
            _waitingListRepository.Save();
            return "WaitingList";
        }

        public User? GetOrganizerUser(int userId) =>
            _userRepository.GetById(userId);
    }
}
