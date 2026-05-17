using Eventra.Models;

namespace Eventra.Repositories.Interfaces
{
    public interface IEventRegistrationRepository : IRepository<EventRegistration>
    {
        IQueryable<EventRegistration> QueryWithEvent();
        IQueryable<EventRegistration> QueryByUserWithEvent(int userId);
    }
}
