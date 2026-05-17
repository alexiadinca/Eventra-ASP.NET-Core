namespace Eventra.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();
        T? GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void Save();
    }
}
