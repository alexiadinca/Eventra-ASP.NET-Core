using Eventra.Models;

namespace Eventra.Services.Interfaces
{
    public interface INotificationService
    {
        void Create(int userId, string title, string message, string type, int? relatedEventId = null);
        bool Exists(int userId, int relatedEventId, string type);
        List<Notification> GetForUser(int userId);
        void Delete(int notificationId, int currentUserId);
        void DeleteAll(int currentUserId);
    }
}
