using Domain.Model.Entities;
using Domain.Repositories;
using Infrastucture.Persistence.Context;
using Infrastucture.Repositories.Base;

namespace Infrastucture.Repositories
{
    public class TranslationRepository : BaseRepository<Translation>, ITranslationRepository
    {
        private readonly ApplicationDataContext _context;
        public TranslationRepository(ApplicationDataContext context) : base(context)
        {
            _context = context;
        }
    }
}
