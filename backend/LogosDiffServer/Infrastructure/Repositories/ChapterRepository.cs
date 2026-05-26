using Domain.Model.Entities;
using Domain.Repositories;
using Infrastucture.Persistence.Context;
using Infrastucture.Repositories.Base;

namespace Infrastucture.Repositories
{
    public class ChapterRepository : BaseRepository<Chapter>, IChapterRepository
    {
        private readonly ApplicationDataContext _context;
        public ChapterRepository(ApplicationDataContext context) : base(context)
        {
            _context = context;
        }
    }
}
