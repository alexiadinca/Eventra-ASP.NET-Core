using Eventra.Models;

namespace Eventra.Repositories.Interfaces
{
    public interface IFavoriteRepository : IRepository<Favorite>
    {
        IQueryable<Favorite> QueryByUserWithEvent(int userId);
        bool IsFavorited(int eventId, int userId);
        Favorite? GetByUserAndEvent(int userId, int eventId);
        int GetFavoriteCount(int eventId);
        Dictionary<int, int> GetFavoriteCounts(List<int> eventIds);
    }
}
