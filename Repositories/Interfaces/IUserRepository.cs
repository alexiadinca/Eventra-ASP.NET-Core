using Eventra.Models;

namespace Eventra.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User? GetByEmail(string email);
        User? GetByUsername(string username);
        bool EmailInUse(string email, int? excludedUserId = null);
    }
}
