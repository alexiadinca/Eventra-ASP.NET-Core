using Eventra.Models;

namespace Eventra.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        IQueryable<Review> QueryWithUser();
        IQueryable<Review> QueryApproved();
        IQueryable<Review> QueryByOrganizer(int organizerId);
        IQueryable<Review> QueryByOrganizerWithUser(int organizerId);
        IQueryable<Review> QueryByOrganizerOrEventsWithUser(int organizerId);
    }
}
