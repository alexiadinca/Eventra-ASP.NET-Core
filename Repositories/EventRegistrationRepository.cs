using Eventra.Data;
using Eventra.Models;
using Eventra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Repositories
{
    public class EventRegistrationRepository : Repository<EventRegistration>, IEventRegistrationRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRegistrationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<EventRegistration> QueryWithEvent()
        {
            return _context.EventRegistrations.Include(r => r.Event);
        }

        public IQueryable<EventRegistration> QueryByUserWithEvent(int userId)
        {
            return QueryWithEvent().Where(r => r.UserId == userId);
        }

        public IQueryable<EventRegistration> QueryByEventWithUserAndCheckIns(int eventId)
        {
            return _context.EventRegistrations
                .Include(r => r.User)
                .Include(r => r.EventCheckIns)
                .Where(r => r.EventId == eventId);
        }

        public bool IsRegisteredForEvent(int userId, int eventId)
        {
            return _context.EventRegistrations
                .Any(r => r.UserId == userId && r.EventId == eventId && r.Status != "Cancelled");
        }
    }
}
