namespace Domain.Repositories.Base
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetItemByIdAsync(Guid id);
        List<T> GetAllItems();
        void AddItem(T item);
        void AddRange(IEnumerable<T> items);
        void UpdateItem(T item);
        void DeleteItem(T item);
        Task SaveChangesAsync();
    }
}
