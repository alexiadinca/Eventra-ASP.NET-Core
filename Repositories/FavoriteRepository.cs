using Eventra.Data;
using Eventra.Models;
using Eventra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Repositories
{
    public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Favorite> QueryByUserWithEvent(int userId)
        {
            return _context.Favorites
                .Include(f => f.Event)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt);
        }

        public bool IsFavorited(int eventId, int userId)
        {
            return _context.Favorites.Any(f => f.EventId == eventId && f.UserId == userId);
        }

        public Favorite? GetByUserAndEvent(int userId, int eventId)
        {
            return _context.Favorites.FirstOrDefault(f => f.UserId == userId && f.EventId == eventId);
        }

        public int GetFavoriteCount(int eventId)
        {
            return _context.Favorites.Count(f => f.EventId == eventId);
        }

        public Dictionary<int, int> GetFavoriteCounts(List<int> eventIds)
        {
            return _context.Favorites
                .Where(f => eventIds.Contains(f.EventId))
                .GroupBy(f => f.EventId)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
