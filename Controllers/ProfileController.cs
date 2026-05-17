using Eventra.Models.ViewModels;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventra.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly INotificationService _notificationService;

        public ProfileController(IProfileService profileService, INotificationService notificationService)
        {
            _profileService = profileService;
            _notificationService = notificationService;
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

            return View(user);
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
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            var user = _profileService.GetUser(userId.Value);

            if (user == null)
                return RedirectToAction("SignIn", "Account");

            var vm = _profileService.CreateEditViewModel(user);

            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(EditProfileViewModel vm)
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
                return View(vm);
            }

            _profileService.UpdateProfile(user, vm);

            return RedirectToAction(nameof(Index));
        }
    }
}