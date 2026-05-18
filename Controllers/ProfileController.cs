using Eventra.Models.ViewModels;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventra.Controllers
{
    public class ProfileController : Controller
    {
        private static readonly HashSet<string> _demoUsernames = new(StringComparer.OrdinalIgnoreCase)
        {
            "admin", "EventraStudios", "TheLobbyRestaurant",
            "Mayfair39", "andreea", "radu", "bianca", "StudentBriceag", "TestOrganizer"
        };

        private readonly IProfileService _profileService;
        private readonly INotificationService _notificationService;
        private readonly IEventService _eventService;
        private readonly IQrCodeService _qrCodeService;

        public ProfileController(IProfileService profileService, INotificationService notificationService, IEventService eventService, IQrCodeService qrCodeService)
        {
            _profileService = profileService;
            _notificationService = notificationService;
            _eventService = eventService;
            _qrCodeService = qrCodeService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            var user = _profileService.GetUser(userId.Value);

            if (user == null)
                return RedirectToAction("SignIn", "Account");

            if (user.Role == "Guest")
            {
                var allPast = _profileService.GetPastRegistrations(userId.Value, int.MaxValue);
                foreach (var reg in allPast)
                {
                    if (!_profileService.HasReviewedEvent(userId.Value, reg.EventId) &&
                        !_notificationService.Exists(userId.Value, reg.EventId, "ReviewReminder"))
                    {
                        _notificationService.Create(
                            userId.Value,
                            "Leave a Review",
                            $"You attended \"{reg.Event.Title}\". Share your experience by leaving a review!",
                            "ReviewReminder",
                            reg.EventId);
                    }
                }
            }

            ViewBag.Notifications = _notificationService.GetForUser(userId.Value);
            ViewBag.FutureRegistrations = _profileService.GetFutureRegistrations(userId.Value);
            ViewBag.WaitingList = _profileService.GetWaitingList(userId.Value);
            ViewBag.PastRegistrations = _profileService.GetPastRegistrations(userId.Value, 6);
            ViewBag.OrganizedEvents = _profileService.GetOrganizedEvents(userId.Value);
            ViewBag.OrganizerReviews = _profileService.GetOrganizerReviews(userId.Value);
            ViewBag.Favorites = _profileService.GetFavorites(userId.Value);
            ViewBag.MyReviews = _profileService.GetMyReviews(userId.Value);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelRegistration(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            try
            {
                _eventService.CancelRegistration(id, userId.Value);
                TempData["SuccessMessage"] = "Your registration has been cancelled.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not cancel your registration.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelWaitingList(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            try
            {
                _eventService.CancelWaitingList(id, userId.Value);
                TempData["SuccessMessage"] = "You have been removed from the waiting list.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not remove you from the waiting list.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Qr(int id)
        {
            var profileUrl = $"{Request.Scheme}://{Request.Host}/Profile/View/{id}";
            var pngBytes = _qrCodeService.GenerateBytes(profileUrl);
            return File(pngBytes, "image/png");
        }

        [HttpGet]
        public IActionResult View(int id)
        {
            var targetUser = _profileService.GetUser(id);
            if (targetUser == null)
                return NotFound();

            ViewBag.TargetUser = targetUser;

            var viewerRole = HttpContext.Session.GetString("Role");
            var viewerId = HttpContext.Session.GetInt32("UserId");

            if (viewerRole == "Organizer" && viewerId.HasValue)
            {
                var now = DateTime.Now;
                var organizerEvents = _profileService.GetOrganizedEvents(viewerId.Value)
                    .Where(e => e.Status == "Approved" && e.EventDate.Date + e.StartTime > now)
                    .OrderBy(e => e.EventDate)
                    .ToList();

                ViewBag.OrganizerEvents = organizerEvents;
                ViewBag.RegisteredEventIds = _profileService.GetRegisteredEventIds(id);
                ViewBag.WaitingListEventIds = _profileService.GetWaitingListEventIds(id);
                ViewBag.CheckedInEventIds = _profileService.GetCheckedInEventIds(id);
            }

            return View();
        }

        [HttpPost]
        public IActionResult DeleteNotification(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            _notificationService.Delete(id, userId.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult DeleteAllNotifications()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            _notificationService.DeleteAll(userId.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            var user = _profileService.GetUser(userId.Value);
            if (user == null)
                return RedirectToAction("SignIn", "Account");

            if (_demoUsernames.Contains(user.Username))
            {
                TempData["ErrorMessage"] = "Password changes are disabled for demo accounts.";
                return RedirectToAction(nameof(Edit));
            }

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel vm)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            var user = _profileService.GetUser(userId.Value);
            if (user == null)
                return RedirectToAction("SignIn", "Account");

            if (_demoUsernames.Contains(user.Username))
            {
                TempData["ErrorMessage"] = "Password changes are disabled for demo accounts.";
                return RedirectToAction(nameof(Edit));
            }

            if (!ModelState.IsValid)
                return View(vm);

            var result = _profileService.ChangePassword(user, vm.CurrentPassword, vm.NewPassword);

            if (result == "WrongPassword")
            {
                ModelState.AddModelError(nameof(vm.CurrentPassword), "Current password is incorrect.");
                return View(vm);
            }

            TempData["SuccessMessage"] = "Your password has been updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            var user = _profileService.GetUser(userId.Value);

            if (user == null)
                return RedirectToAction("SignIn", "Account");

            var vm = _profileService.CreateEditViewModel(user);
            ViewBag.CurrentPhotoPath = user.ProfilePhotoPath;

            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(EditProfileViewModel vm, IFormFile? profilePhoto)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            var user = _profileService.GetUser(userId.Value);

            if (user == null)
                return RedirectToAction("SignIn", "Account");

            bool emailUsedByAnother = _profileService.EmailInUse(vm.Email, user.Id);

            if (emailUsedByAnother)
            {
                ModelState.AddModelError(nameof(vm.Email), "This email is already used by another account.");
                ViewBag.CurrentPhotoPath = user.ProfilePhotoPath;
                return View(vm);
            }

            try
            {
                _profileService.UpdateProfile(user, vm, profilePhoto);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.CurrentPhotoPath = user.ProfilePhotoPath;
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}