using Eventra.Models;
using Eventra.Models.ViewModels;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventra.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public IActionResult Create(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account", new { returnUrl = Url.Action(nameof(Create), new { eventId }) });

            var ev = _reviewService.GetEventForReview(eventId);

            if (ev == null)
                return NotFound();

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

            if (!ModelState.IsValid)
            {
                ViewBag.Event = _reviewService.GetEventForReview(vm.EventId);

                return View(vm);
            }

            try
            {
                _reviewService.SubmitReview(vm, userId.Value);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            return RedirectToAction("Details", "Events", new { id = vm.EventId });
        }
    }
}