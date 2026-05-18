using Eventra.Data;
using Eventra.Models;
using Eventra.Repositories.Interfaces;

namespace Eventra.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Notification> QueryByUser(int userId)
        {
            return _context.Notifications.Where(n => n.UserId == userId);
        }

        public bool Exists(int userId, int relatedEventId, string type) =>
            _context.Notifications.Any(n =>
                n.UserId == userId &&
                n.RelatedEventId == relatedEventId &&
                n.Type == type);

        public void DeleteByEventAndType(int userId, int relatedEventId, string type)
        {
            var matches = _context.Notifications
                .Where(n => n.UserId == userId && n.RelatedEventId == relatedEventId && n.Type == type)
                .ToList();
            _context.Notifications.RemoveRange(matches);
            _context.SaveChanges();
        }
    }
}
