using Eventra.Data;
using Eventra.Models;
using Eventra.Repositories.Interfaces;

namespace Eventra.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Category> QueryAll()
        {
            return _context.Categories;
        }
    }
}
