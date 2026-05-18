using Eventra.Models;
using Eventra.Models.ViewModels;

namespace Eventra.Services.Interfaces
{
    public interface IAccountService
    {
        User? FindByEmailOrUsername(string emailOrUsername);
        bool VerifyPassword(User user, string password);
        OrganizerApprovalRequest? GetLatestApprovalRequest(int userId);
        bool EmailExists(string email);
        bool UsernameExists(string username);
        User Register(RegisterViewModel vm);
        string GenerateResetToken(User user);
        User? GetUserByResetToken(string token);
        void ResetPassword(User user, string newPassword);
    }
}
