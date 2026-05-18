using Eventra.Models;

namespace Eventra.Repositories.Interfaces
{
    public interface IEventCheckInRepository : IRepository<EventCheckIn>
    {
        IQueryable<EventCheckIn> QueryByEventWithAttendee(int eventId);
    }
}
