using Eventra.Models;

namespace Eventra.Repositories.Interfaces
{
    public interface IEventRegistrationRepository : IRepository<EventRegistration>
    {
        IQueryable<EventRegistration> QueryWithEvent();
        IQueryable<EventRegistration> QueryByUserWithEvent(int userId);
        IQueryable<EventRegistration> QueryByEventWithUserAndCheckIns(int eventId);
        bool IsRegisteredForEvent(int userId, int eventId);
    }
}
