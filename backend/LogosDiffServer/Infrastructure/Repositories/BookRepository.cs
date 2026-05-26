using Domain.Model.Entities;
using Domain.Repositories;
using Infrastucture.Persistence.Context;
using Infrastucture.Repositories.Base;

namespace Infrastucture.Repositories
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly ApplicationDataContext _context;

        public BookRepository(ApplicationDataContext context) : base(context)
        {
            _context = context;
        }
    }
}
