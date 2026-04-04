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

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(LoginViewModel vm)
        {
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

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("FirstName", user.FirstName);
            HttpContext.Session.SetString("Role", user.Role);

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

            bool emailExists = _context.Users.Any(u => u.Email == vm.Email);
            bool usernameExists = _context.Users.Any(u => u.Username == vm.Username);

            if (emailExists)
                ModelState.AddModelError(nameof(vm.Email), "An account with this email already exists.");

            if (usernameExists)
                ModelState.AddModelError(nameof(vm.Username), "This username is already taken.");

            if (!ModelState.IsValid)
                return View(vm);

            var user = new User
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                Username = vm.Username,
                PhoneNumber = vm.PhoneNumber,
                Role = vm.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                QrCodePath = "/images/QrCode.png"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, vm.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("FirstName", user.FirstName);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Profile");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}