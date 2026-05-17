using Eventra.Models;

namespace Eventra.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        IQueryable<Category> QueryAll();
    }
}
