using Eventra.Models;

namespace Eventra.Repositories.Interfaces
{
    public interface IWaitingListRepository : IRepository<WaitingListEntry>
    {
        IQueryable<WaitingListEntry> QueryWithEvent();
        IQueryable<WaitingListEntry> QueryByUserWithEvent(int userId);
    }
}
