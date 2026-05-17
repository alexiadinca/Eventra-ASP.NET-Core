using Eventra.Models;

namespace Eventra.Repositories.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        IQueryable<Event> QueryWithDetails();
        Event? GetByIdWithDetails(int id);
        IQueryable<Event> QueryWithCategoryAndOrganizer();
        IQueryable<Event> QueryApproved();
        IQueryable<Event> QueryByOrganizer(int organizerId);
    }
}
