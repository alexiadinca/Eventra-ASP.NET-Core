using Eventra.Data;
using Eventra.Models;
using Eventra.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Eventra.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        private static readonly HashSet<string> _demoUsernames = new(StringComparer.OrdinalIgnoreCase)
        {
            "admin", "AlexiaDinca", "EventraStudios", "TheLobbyRestaurant",
            "Mayfair39", "andreea", "radu", "bianca", "StudentBriceag"
        };

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpGet]
        public IActionResult SignIn(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public IActionResult SignIn(LoginViewModel vm, string? returnUrl = null)
        {
            vm.ReturnUrl ??= returnUrl;

            if (!ModelState.IsValid)
                return View(vm);

            var user = _context.Users.FirstOrDefault(u =>
                u.Email == vm.EmailOrUsername || u.Username == vm.EmailOrUsername);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email/username or password.");
                return View(vm);
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, vm.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid email/username or password.");
                return View(vm);
            }

            if (user.Role == "Organizer" && !user.IsApproved)
            {
                var request = _context.OrganizerApprovalRequests
                    .Where(r => r.UserId == user.Id)
                    .OrderByDescending(r => r.RequestedAt)
                    .FirstOrDefault();

                if (request?.Status == "Rejected")
                    return RedirectToAction(nameof(RejectedAccount));

                return RedirectToAction(nameof(PendingApproval));
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("FirstName", user.FirstName);
            HttpContext.Session.SetString("Role", user.Role);

            if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return Redirect(vm.ReturnUrl);

            if (user.Role == "Admin")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (vm.Role == "Admin")
            {
                ModelState.AddModelError(nameof(vm.Role), "Invalid account type.");
                return View(vm);
            }

            bool emailExists = _context.Users.Any(u => u.Email == vm.Email);
            bool usernameExists = _context.Users.Any(u => u.Username == vm.Username);

            if (emailExists)
                ModelState.AddModelError(nameof(vm.Email), "An account with this email already exists.");

            if (usernameExists)
                ModelState.AddModelError(nameof(vm.Username), "This username is already taken.");

            if (!ModelState.IsValid)
                return View(vm);

            var isOrganizer = vm.Role == "Organizer";

            var user = new User
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                Username = vm.Username,
                PhoneNumber = vm.PhoneNumber,
                Role = vm.Role,
                IsActive = true,
                IsApproved = !isOrganizer,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, vm.Password);
            _context.Users.Add(user);
            _context.SaveChanges();

            if (isOrganizer)
            {
                var request = new OrganizerApprovalRequest
                {
                    UserId = user.Id,
                    Status = "Pending",
                    RequestedAt = DateTime.UtcNow
                };
                _context.OrganizerApprovalRequests.Add(request);
                _context.SaveChanges();
            }

            if (isOrganizer)
                return RedirectToAction(nameof(PendingApproval));

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("FirstName", user.FirstName);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Profile");
        }

        [HttpGet]
        public IActionResult PendingApproval()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RejectedAccount()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = _context.Users.FirstOrDefault(u =>
                u.Email == vm.EmailOrUsername || u.Username == vm.EmailOrUsername);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No account found with that email or username.");
                return View(vm);
            }

            if (_demoUsernames.Contains(user.Username))
            {
                ModelState.AddModelError(string.Empty, "Password reset is disabled for demo accounts.");
                return View(vm);
            }

            TempData["ResetUserId"] = user.Id;
            return RedirectToAction(nameof(ResetPassword));
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (!TempData.ContainsKey("ResetUserId"))
                return RedirectToAction(nameof(ForgotPassword));

            TempData.Keep("ResetUserId");
            return View(new ResetPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel vm)
        {
            if (!TempData.ContainsKey("ResetUserId"))
                return RedirectToAction(nameof(ForgotPassword));

            var userId = Convert.ToInt32(TempData["ResetUserId"]);

            if (!ModelState.IsValid)
            {
                TempData["ResetUserId"] = userId;
                return View(vm);
            }

            var user = _context.Users.Find(userId);
            if (user == null)
                return RedirectToAction(nameof(ForgotPassword));

            user.PasswordHash = _passwordHasher.HashPassword(user, vm.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Your password has been reset. You can now sign in with your new password.";
            return RedirectToAction(nameof(SignIn));
        }
    }
}
