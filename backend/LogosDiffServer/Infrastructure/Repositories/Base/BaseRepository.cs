using Domain.Repositories.Base;
using Infrastucture.Persistence.Context;

namespace Infrastucture.Repositories.Base
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDataContext _context;

        protected BaseRepository(ApplicationDataContext context)
        {
            _context = context;
        }

        public void AddItem(T item)
        {
            _context.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            _context.AddRange(items);
        }

        public void DeleteItem(T item)
        {
            _context.Remove(item);
        }

        public async Task<T?> GetItemByIdAsync(Guid id)
        {
            return await _context.FindAsync<T>(id);
        }

        public List<T> GetAllItems()
        {
            return [.. _context.Set<T>()];
        }

        public void UpdateItem(T item)
        {
            _context.Update(item);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
