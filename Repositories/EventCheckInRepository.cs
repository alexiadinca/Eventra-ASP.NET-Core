using Eventra.Data;
using Eventra.Models;
using Eventra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Repositories
{
    public class EventCheckInRepository : Repository<EventCheckIn>, IEventCheckInRepository
    {
        private readonly ApplicationDbContext _context;

        public EventCheckInRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<EventCheckIn> QueryByEventWithAttendee(int eventId)
        {
            return _context.EventCheckIns
                .Include(c => c.EventRegistration).ThenInclude(r => r.User)
                .Where(c => c.EventRegistration.EventId == eventId)
                .OrderByDescending(c => c.ScannedAt);
        }
    }
}
