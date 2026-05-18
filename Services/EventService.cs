using Eventra.Models;
using Eventra.Models.ViewModels;
using Eventra.Repositories.Interfaces;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
        private readonly IEventCheckInRepository _eventCheckInRepository;
        private readonly IFavoriteRepository _favoriteRepository;

        public EventService(
            IEventRepository eventRepository,
            ICategoryRepository categoryRepository,
            IReviewRepository reviewRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            IWaitingListRepository waitingListRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment environment,
            INotificationService notificationService,
            IEventCheckInRepository eventCheckInRepository,
            IFavoriteRepository favoriteRepository)
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
            _eventCheckInRepository = eventCheckInRepository;
            _favoriteRepository = favoriteRepository;
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

            if (filter == "MostPopular")
                return query.OrderByDescending(e => e.Capacity - e.AvailableSeats).ToList();

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

            var uid = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            EventRegistration? myReg = null;
            WaitingListEntry? myWaiting = null;
            bool hasReviewed = false;
            bool isFavorited = false;

            if (uid.HasValue)
            {
                myReg = _eventRegistrationRepository.QueryByUserWithEvent(uid.Value)
                    .FirstOrDefault(r => r.EventId == id);
                myWaiting = _waitingListRepository.QueryByUserWithEvent(uid.Value)
                    .FirstOrDefault(w => w.EventId == id);
                hasReviewed = _reviewRepository.QueryWithUser()
                    .Any(r => r.EventId == id && r.UserId == uid.Value);
                isFavorited = _favoriteRepository.IsFavorited(id, uid.Value);
            }

            return new EventDetailsViewModel
            {
                Event = ev,
                SimilarEvents = similarEvents,
                OrganizerReviews = organizerReviews,
                IsLoggedIn = uid.HasValue,
                IsOrganizerOwner = uid.HasValue && uid.Value == ev.OrganizerId,
                HasRegistered = myReg != null,
                RegistrationId = myReg?.Id,
                HasReviewed = hasReviewed,
                IsPastEvent = ev.EventDate.Date + ev.StartTime < DateTime.Now,
                HasAvailableSeats = ev.AvailableSeats > 0,
                IsOnWaitingList = myWaiting != null,
                WaitingListPosition = myWaiting?.Position,
                WaitingListEntryId = myWaiting?.Id,
                IsFavorited = isFavorited,
                FavoriteCount = ev.FavoriteCount,
                IsAdmin = _httpContextAccessor.HttpContext?.Session.GetString("Role") == "Admin",
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

            if (ev.EventDate.Date + ev.StartTime < DateTime.Now)
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

            if (!_notificationService.Exists(userId, eventId, "WaitingListJoined"))
            {
                _notificationService.Create(
                    userId,
                    "Added to Waiting List",
                    $"You are #{waiting.Position} on the waiting list for \"{ev.Title}\".",
                    "WaitingListJoined",
                    eventId);
            }

            return "WaitingList";
        }

        public void CancelRegistration(int registrationId, int userId)
        {
            var registration = _eventRegistrationRepository.QueryWithEvent()
                .FirstOrDefault(r => r.Id == registrationId && r.UserId == userId);

            if (registration == null)
                throw new InvalidOperationException("Registration not found.");

            if (registration.Event.EventDate.Date < DateTime.Today)
                throw new InvalidOperationException("Cannot cancel a past event registration.");

            var eventId = registration.EventId;
            var ev = registration.Event;

            _eventRegistrationRepository.Remove(registration);
            _eventRegistrationRepository.Save();

            var firstWaiting = _waitingListRepository.QueryWithEvent()
                .Where(w => w.EventId == eventId)
                .OrderBy(w => w.Position)
                .FirstOrDefault();

            if (firstWaiting != null)
            {
                var promotedUserId = firstWaiting.UserId;

                _waitingListRepository.Remove(firstWaiting);
                _waitingListRepository.Save();

                _eventRegistrationRepository.Add(new EventRegistration
                {
                    UserId = promotedUserId,
                    EventId = eventId,
                    RegisteredAt = DateTime.UtcNow,
                    Status = "Registered",
                    QrToken = Guid.NewGuid().ToString()
                });
                _eventRegistrationRepository.Save();

                var remaining = _waitingListRepository.QueryWithEvent()
                    .Where(w => w.EventId == eventId)
                    .OrderBy(w => w.Position)
                    .ToList();

                for (int i = 0; i < remaining.Count; i++)
                {
                    remaining[i].Position = i + 1;
                    _waitingListRepository.Update(remaining[i]);
                }
                _waitingListRepository.Save();

                _notificationService.Create(
                    promotedUserId,
                    "You got a spot!",
                    $"A spot opened up for \"{ev.Title}\" and you have been registered from the waiting list.",
                    "WaitingListPromotion",
                    eventId);
            }
            else
            {
                ev.AvailableSeats++;
                _eventRepository.Update(ev);
                _eventRepository.Save();
            }
        }

        public void CancelWaitingList(int entryId, int userId)
        {
            var entry = _waitingListRepository.QueryWithEvent()
                .FirstOrDefault(w => w.Id == entryId && w.UserId == userId);

            if (entry == null)
                throw new InvalidOperationException("Waiting list entry not found.");

            var eventId = entry.EventId;
            var removedPosition = entry.Position;

            _waitingListRepository.Remove(entry);
            _waitingListRepository.Save();

            var remaining = _waitingListRepository.QueryWithEvent()
                .Where(w => w.EventId == eventId && w.Position > removedPosition)
                .OrderBy(w => w.Position)
                .ToList();

            foreach (var w in remaining)
            {
                w.Position--;
                _waitingListRepository.Update(w);
            }
            _waitingListRepository.Save();
        }

        public User? GetOrganizerUser(int userId) =>
            _userRepository.GetById(userId);

        public (string result, string? attendeeName, int? attendeeUserId) ProcessCheckIn(int eventId, int organizerId, string qrToken)
        {
            var ev = _eventRepository.QueryByOrganizer(organizerId).FirstOrDefault(e => e.Id == eventId);
            if (ev == null)
                return ("NotYourEvent", null, null);

            int userId;
            var match = System.Text.RegularExpressions.Regex.Match(qrToken, @"/Profile/View/(\d+)");
            if (match.Success)
            {
                if (!int.TryParse(match.Groups[1].Value, out userId))
                    return ("InvalidToken", null, null);
            }
            else
            {
                return ("InvalidToken", null, null);
            }

            var registration = _eventRegistrationRepository
                .QueryByEventWithUserAndCheckIns(eventId)
                .FirstOrDefault(r => r.UserId == userId);

            if (registration == null)
                return ("NotRegistered", null, userId);

            var attendeeName = $"{registration.User.FirstName} {registration.User.LastName}";

            if (registration.CheckedInAt.HasValue || registration.EventCheckIns.Any())
                return ("AlreadyCheckedIn", attendeeName, userId);

            _eventCheckInRepository.Add(new EventCheckIn
            {
                EventRegistrationId = registration.Id,
                ScannedByOrganizerId = organizerId,
                ScannedAt = DateTime.UtcNow,
                Result = "Valid"
            });
            _eventCheckInRepository.Save();

            registration.CheckedInAt = DateTime.UtcNow;
            _eventRegistrationRepository.Update(registration);
            _eventRegistrationRepository.Save();

            return ("Valid", attendeeName, userId);
        }

        public List<EventCheckIn> GetCheckInsForEvent(int eventId)
        {
            return _eventCheckInRepository
                .QueryByEventWithAttendee(eventId)
                .ToList();
        }

        public string ToggleFavorite(int eventId, int userId)
        {
            var ev = _eventRepository.GetById(eventId);
            if (ev == null) return "NotFound";

            var existing = _favoriteRepository.GetByUserAndEvent(userId, eventId);
            if (existing != null)
            {
                _favoriteRepository.Remove(existing);
                _favoriteRepository.Save();
                ev.FavoriteCount = Math.Max(0, ev.FavoriteCount - 1);
                _eventRepository.Update(ev);
                _eventRepository.Save();
                return "Removed";
            }

            _favoriteRepository.Add(new Favorite
            {
                UserId = userId,
                EventId = eventId,
                CreatedAt = DateTime.UtcNow
            });
            _favoriteRepository.Save();
            ev.FavoriteCount++;
            _eventRepository.Update(ev);
            _eventRepository.Save();
            return "Added";
        }

        public HashSet<int> GetFavoriteEventIds(int userId)
        {
            return _favoriteRepository.Query()
                .Where(f => f.UserId == userId)
                .Select(f => f.EventId)
                .ToHashSet();
        }

        public Dictionary<int, int> GetFavoriteCounts(List<int> eventIds)
        {
            return _eventRepository.Query()
                .Where(e => eventIds.Contains(e.Id))
                .ToDictionary(e => e.Id, e => e.FavoriteCount);
        }
    }
}
