using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventra.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private IActionResult RequireAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
                return RedirectToAction("SignIn", "Account");
            return null!;
        }

        public IActionResult Index()
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            ViewBag.PendingOrganizers = _adminService.PendingOrganizersCount();
            ViewBag.PendingEvents = _adminService.PendingEventsCount();
            ViewBag.PendingReviews = _adminService.PendingReviewsCount();
            ViewBag.FlaggedReviews = _adminService.FlaggedReviewsCount();

            return View();
        }

        // ── ORGANIZERS ──────────────────────────────────────────

        public IActionResult Organizers()
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var list = _adminService.GetPendingOrganizers();
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveOrganizer(int id)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var adminId = HttpContext.Session.GetInt32("UserId")!.Value;
            _adminService.ApproveOrganizer(id, adminId);
            TempData["Success"] = "Organizer approved.";
            return RedirectToAction(nameof(Organizers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectOrganizer(int id)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var adminId = HttpContext.Session.GetInt32("UserId")!.Value;
            _adminService.RejectOrganizer(id, adminId);
            TempData["Success"] = "Organizer rejected.";
            return RedirectToAction(nameof(Organizers));
        }

        // ── EVENTS ──────────────────────────────────────────────

        public IActionResult Events()
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var list = _adminService.GetPendingEvents();
            return View(list);
        }

        public IActionResult EventDetail(int id)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var ev = _adminService.GetEventById(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveEvent(int id)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var adminId = HttpContext.Session.GetInt32("UserId")!.Value;
            _adminService.ApproveEvent(id, adminId);
            TempData["Success"] = "Event approved and published.";
            return RedirectToAction(nameof(Events));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectEvent(int id)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            _adminService.RejectEvent(id);
            TempData["Success"] = "Event rejected and removed.";
            return RedirectToAction(nameof(Events));
        }

        // ── REVIEWS ─────────────────────────────────────────────

        public IActionResult Reviews()
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            ViewBag.Pending = _adminService.GetPendingReviews();
            ViewBag.Flagged = _adminService.GetFlaggedReviews();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveReview(int id)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            _adminService.ApproveReview(id);
            TempData["Success"] = "Review approved.";
            return RedirectToAction(nameof(Reviews));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteReview(int id)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            _adminService.DeleteReview(id);
            TempData["Success"] = "Review deleted.";
            return RedirectToAction(nameof(Reviews));
        }

        // ── PROFILE ─────────────────────────────────────────────

        [HttpGet]
        public IActionResult EditProfile()
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var adminId = HttpContext.Session.GetInt32("UserId")!.Value;
            var admin = _adminService.GetAdmin(adminId);
            if (admin == null) return RedirectToAction("SignIn", "Account");

            ViewBag.Email = admin.Email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEmail(string email)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var adminId = HttpContext.Session.GetInt32("UserId")!.Value;

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            {
                TempData["EmailError"] = "Please enter a valid email address.";
                return RedirectToAction(nameof(EditProfile));
            }

            if (_adminService.EmailInUse(email, adminId))
            {
                TempData["EmailError"] = "This email is already in use by another account.";
                return RedirectToAction(nameof(EditProfile));
            }

            _adminService.UpdateAdminEmail(adminId, email);
            TempData["EmailSuccess"] = "Email updated successfully.";
            return RedirectToAction(nameof(EditProfile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(string newPassword, string confirmPassword)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var adminId = HttpContext.Session.GetInt32("UserId")!.Value;

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
            {
                TempData["PasswordError"] = "Password must be at least 8 characters.";
                return RedirectToAction(nameof(EditProfile));
            }

            if (newPassword != confirmPassword)
            {
                TempData["PasswordError"] = "Passwords do not match.";
                return RedirectToAction(nameof(EditProfile));
            }

            _adminService.UpdateAdminPassword(adminId, newPassword);
            TempData["PasswordSuccess"] = "Password updated successfully.";
            return RedirectToAction(nameof(EditProfile));
        }
    }
}
