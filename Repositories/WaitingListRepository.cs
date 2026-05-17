using Eventra.Data;
using Eventra.Models;
using Eventra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Repositories
{
    public class WaitingListRepository : Repository<WaitingListEntry>, IWaitingListRepository
    {
        private readonly ApplicationDbContext _context;

        public WaitingListRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<WaitingListEntry> QueryWithEvent()
        {
            return _context.WaitingListEntries.Include(w => w.Event);
        }

        public IQueryable<WaitingListEntry> QueryByUserWithEvent(int userId)
        {
            return QueryWithEvent().Where(w => w.UserId == userId);
        }
    }
}
