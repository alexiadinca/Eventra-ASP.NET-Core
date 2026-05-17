using Eventra.Data;
using Eventra.Models;
using Eventra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Event> QueryWithDetails()
        {
            return _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer);
        }

        public Event? GetByIdWithDetails(int id)
        {
            return QueryWithDetails().FirstOrDefault(e => e.Id == id);
        }

        public IQueryable<Event> QueryWithCategoryAndOrganizer()
        {
            return QueryWithDetails();
        }

        public IQueryable<Event> QueryApproved()
        {
            return QueryWithDetails().Where(e => e.Status == "Approved");
        }

        public IQueryable<Event> QueryByOrganizer(int organizerId)
        {
            return QueryWithDetails().Where(e => e.OrganizerId == organizerId);
        }
    }
}
