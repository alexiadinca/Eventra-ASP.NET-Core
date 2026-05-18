using Eventra.Models;
using Eventra.Models.ViewModels;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventra.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly INotificationService _notificationService;

        public ReviewsController(IReviewService reviewService, INotificationService notificationService)
        {
            _reviewService = reviewService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult Create(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account", new { returnUrl = Url.Action(nameof(Create), new { eventId }) });

            var role = HttpContext.Session.GetString("Role");
            if (role == "Admin" || role == "Organizer")
                return RedirectToAction("Details", "Events", new { id = eventId });

            var ev = _reviewService.GetEventForReview(eventId);
            if (ev == null)
                return NotFound();

            if (!_reviewService.IsRegisteredForEvent(userId.Value, eventId))
            {
                TempData["ErrorMessage"] = "You can only review events you registered for.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            ViewBag.Event = ev;
            var vm = _reviewService.GetReviewEditViewModel(eventId, userId.Value) ?? _reviewService.CreateReviewViewModel(eventId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubmitReviewViewModel vm)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account", new { returnUrl = Url.Action(nameof(Create), new { eventId = vm.EventId }) });

            var role = HttpContext.Session.GetString("Role");
            if (role == "Admin" || role == "Organizer")
                return RedirectToAction("Details", "Events", new { id = vm.EventId });

            if (!ModelState.IsValid)
            {
                ViewBag.Event = _reviewService.GetEventForReview(vm.EventId);
                return View(vm);
            }

            var result = _reviewService.SubmitReview(vm, userId.Value);

            if (result == "NotRegistered")
            {
                TempData["ErrorMessage"] = "You can only review events you registered for.";
                return RedirectToAction("Details", "Events", new { id = vm.EventId });
            }

            if (result == "EventNotFound" || result == "NotFound")
                return NotFound();

            _notificationService.DeleteReviewReminder(userId.Value, vm.EventId);

            return RedirectToAction("Details", "Events", new { id = vm.EventId });
        }
    }
}