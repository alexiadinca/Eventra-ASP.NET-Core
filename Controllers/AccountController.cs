using Eventra.Models.ViewModels;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventra.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        private static readonly HashSet<string> _demoUsernames = new(StringComparer.OrdinalIgnoreCase)
        {
            "admin", "EventraStudios", "TheLobbyRestaurant",
            "Mayfair39", "andreea", "radu", "bianca", "StudentBriceag", "TestOrganizer"
        };

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
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

            var user = _accountService.FindByEmailOrUsername(vm.EmailOrUsername);

            if (user == null || !_accountService.VerifyPassword(user, vm.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid email/username or password.");
                return View(vm);
            }

            if (user.Role == "Organizer" && !user.IsApproved)
            {
                var request = _accountService.GetLatestApprovalRequest(user.Id);

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

            if (_accountService.EmailExists(vm.Email))
                ModelState.AddModelError(nameof(vm.Email), "An account with this email already exists.");

            if (_accountService.UsernameExists(vm.Username))
                ModelState.AddModelError(nameof(vm.Username), "This username is already taken.");

            if (!ModelState.IsValid)
                return View(vm);

            var user = _accountService.Register(vm);

            if (user.Role == "Organizer")
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

            var user = _accountService.FindByEmailOrUsername(vm.EmailOrUsername);

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

            var token = _accountService.GenerateResetToken(user);
            return RedirectToAction(nameof(ResetPassword), new { token });
        }

        [HttpGet]
        public IActionResult ResetPassword(string? token = null)
        {
            if (string.IsNullOrWhiteSpace(token) || _accountService.GetUserByResetToken(token) == null)
            {
                TempData["ErrorMessage"] = "This password reset link is invalid or has expired.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = _accountService.GetUserByResetToken(vm.Token ?? "");
            if (user == null)
            {
                TempData["ErrorMessage"] = "This password reset link is invalid or has expired.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            _accountService.ResetPassword(user, vm.NewPassword);

            TempData["SuccessMessage"] = "Your password has been reset. You can now sign in with your new password.";
            return RedirectToAction(nameof(SignIn));
        }
    }
}
