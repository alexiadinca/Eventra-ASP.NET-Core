using Eventra.Models;
using Eventra.Repositories.Interfaces;
using Eventra.Services.Interfaces;

namespace Eventra.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        public void Create(int userId, string title, string message, string type, int? relatedEventId = null)
        {
            _repo.Add(new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                RelatedEventId = relatedEventId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
            _repo.Save();
        }

        public bool Exists(int userId, int relatedEventId, string type) =>
            _repo.Exists(userId, relatedEventId, type);

        public List<Notification> GetForUser(int userId) =>
            _repo.QueryByUser(userId)
                 .OrderByDescending(n => n.CreatedAt)
                 .ToList();

        public void Delete(int notificationId, int currentUserId)
        {
            var n = _repo.GetById(notificationId);
            if (n == null || n.UserId != currentUserId) return;
            _repo.Remove(n);
            _repo.Save();
        }

        public void DeleteAll(int currentUserId)
        {
            var list = _repo.QueryByUser(currentUserId).ToList();
            foreach (var n in list)
                _repo.Remove(n);
            _repo.Save();
        }
    }
}
