using Domain.Model.Entities;
using Domain.Repositories;
using Infrastucture.Persistence.Context;
using Infrastucture.Repositories.Base;

namespace Infrastucture.Repositories
{
    public class VerseRepository : BaseRepository<Verse>, IVerseRepository
    {
        private readonly ApplicationDataContext _context;
        public VerseRepository(ApplicationDataContext context) : base(context)
        {
            _context = context;
        }
    }
}
