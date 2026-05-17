using Eventra.Data;
using Eventra.Models;
using Eventra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User? GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public bool EmailInUse(string email, int? excludedUserId = null)
        {
            return _context.Users.Any(u => u.Email == email && (!excludedUserId.HasValue || u.Id != excludedUserId.Value));
        }
    }
}
