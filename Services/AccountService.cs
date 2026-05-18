using Eventra.Data;
using Eventra.Models;
using Eventra.Models.ViewModels;
using Eventra.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Eventra.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _hasher;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
            _hasher = new PasswordHasher<User>();
        }

        public User? FindByEmailOrUsername(string emailOrUsername) =>
            _context.Users.FirstOrDefault(u => u.Email == emailOrUsername || u.Username == emailOrUsername);

        public bool VerifyPassword(User user, string password) =>
            _hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;

        public OrganizerApprovalRequest? GetLatestApprovalRequest(int userId) =>
            _context.OrganizerApprovalRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestedAt)
                .FirstOrDefault();

        public bool EmailExists(string email) =>
            _context.Users.Any(u => u.Email == email);

        public bool UsernameExists(string username) =>
            _context.Users.Any(u => u.Username == username);

        public User Register(RegisterViewModel vm)
        {
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

            user.PasswordHash = _hasher.HashPassword(user, vm.Password);
            _context.Users.Add(user);
            _context.SaveChanges();

            if (isOrganizer)
            {
                _context.OrganizerApprovalRequests.Add(new OrganizerApprovalRequest
                {
                    UserId = user.Id,
                    Status = "Pending",
                    RequestedAt = DateTime.UtcNow
                });
                _context.SaveChanges();
            }

            return user;
        }

        public string GenerateResetToken(User user)
        {
            user.PasswordResetToken = Guid.NewGuid().ToString("N");
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            _context.SaveChanges();
            return user.PasswordResetToken;
        }

        public User? GetUserByResetToken(string token) =>
            _context.Users.FirstOrDefault(u =>
                u.PasswordResetToken == token &&
                u.PasswordResetTokenExpiry > DateTime.UtcNow);

        public void ResetPassword(User user, string newPassword)
        {
            user.PasswordHash = _hasher.HashPassword(user, newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}
