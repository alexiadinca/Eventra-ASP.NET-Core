using Eventra.Models;

namespace Eventra.Repositories.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        IQueryable<Notification> QueryByUser(int userId);
        bool Exists(int userId, int relatedEventId, string type);
        void DeleteByEventAndType(int userId, int relatedEventId, string type);
    }
}
