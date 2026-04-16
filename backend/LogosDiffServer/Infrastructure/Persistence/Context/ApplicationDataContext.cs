using Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastucture.Persistence.Context
{
    public class ApplicationDataContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Verse> Verses { get; set; }

        public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDataContext).Assembly);
        }
    }
}
