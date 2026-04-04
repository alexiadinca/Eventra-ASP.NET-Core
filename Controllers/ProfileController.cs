using Eventra.Data;
using Eventra.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);

            if (user == null)
                return RedirectToAction("SignIn", "Account");

            ViewBag.Notifications = _context.Notifications
                .Where(n => n.UserId == user.Id)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToList();

            ViewBag.FutureRegistrations = _context.EventRegistrations
                .Include(r => r.Event)
                .Where(r => r.UserId == user.Id && r.Event.EventDate >= DateTime.Today)
                .OrderBy(r => r.Event.EventDate)
                .ToList();

            ViewBag.WaitingList = _context.WaitingListEntries
                .Include(w => w.Event)
                .Where(w => w.UserId == user.Id)
                .OrderBy(w => w.JoinedAt)
                .ToList();

            ViewBag.PastRegistrations = _context.EventRegistrations
                .Include(r => r.Event)
                .Where(r => r.UserId == user.Id && r.Event.EventDate < DateTime.Today)
                .OrderByDescending(r => r.Event.EventDate)
                .Take(6)
                .ToList();

            ViewBag.OrganizedEvents = _context.Events
                .Where(e => e.OrganizerId == user.Id)
                .OrderByDescending(e => e.EventDate)
                .ToList();

            ViewBag.OrganizerReviews = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.OrganizerId == user.Id && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.OrganizedEvents = _context.Events
                .Where(e => e.OrganizerId == user.Id)
                .OrderByDescending(e => e.EventDate)
                .ToList();

            ViewBag.OrganizerReviews = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.OrganizerId == user.Id && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(user);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("SignIn", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);

            if (user == null)
                return RedirectToAction("SignIn", "Account");

            var vm = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

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

            var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);

            if (user == null)
                return RedirectToAction("SignIn", "Account");

            bool emailUsedByAnother = _context.Users.Any(u => u.Email == vm.Email && u.Id != user.Id);

            if (emailUsedByAnother)
            {
                ModelState.AddModelError(nameof(vm.Email), "This email is already used by another account.");
                return View(vm);
            }

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Email = vm.Email;
            user.PhoneNumber = vm.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}