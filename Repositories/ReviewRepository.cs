using Eventra.Data;
using Eventra.Models;
using Eventra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventra.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Review> QueryWithUser()
        {
            return _context.Reviews.Include(r => r.User);
        }

        public IQueryable<Review> QueryApproved()
        {
            return QueryWithUser().Where(r => r.IsApproved);
        }

        public IQueryable<Review> QueryByOrganizer(int organizerId)
        {
            return _context.Reviews.Where(r => r.OrganizerId == organizerId);
        }

        public IQueryable<Review> QueryByOrganizerWithUser(int organizerId)
        {
            return QueryWithUser().Where(r => r.OrganizerId == organizerId);
        }

        public IQueryable<Review> QueryByOrganizerOrEventsWithUser(int organizerId)
        {
            return _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Event)
                .Where(r => r.OrganizerId == organizerId || (r.EventId.HasValue && r.Event != null && r.Event.OrganizerId == organizerId));
        }
    }
}
