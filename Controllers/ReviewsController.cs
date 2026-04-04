using Eventra.Data;
using Eventra.Models;
using Eventra.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create(int eventId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            var ev = _context.Events
                .Include(e => e.Organizer)
                .FirstOrDefault(e => e.Id == eventId);

            if (ev == null)
                return NotFound();

            ViewBag.Event = ev;

            var vm = new SubmitReviewViewModel
            {
                EventId = eventId
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubmitReviewViewModel vm)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.Event = _context.Events
                    .Include(e => e.Organizer)
                    .FirstOrDefault(e => e.Id == vm.EventId);

                return View(vm);
            }

            var ev = _context.Events.FirstOrDefault(e => e.Id == vm.EventId);
            if (ev == null)
                return NotFound();

            var review = new Review
            {
                UserId = userId.Value,
                EventId = ev.Id,
                OrganizerId = ev.OrganizerId,
                Rating = vm.Rating,
                Comment = vm.Comment,
                CreatedAt = DateTime.UtcNow,
                IsApproved = true
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("Details", "Events", new { id = vm.EventId });
        }
    }
}