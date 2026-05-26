using Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastucture.Persistence.Configurations
{
    public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
    {
        public void Configure(EntityTypeBuilder<Translation> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Abbreviature)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.Language)
                .IsRequired();

            builder.HasMany(t => t.Verses)
                .WithOne(v => v.Translation)
                .HasForeignKey(v => v.TranslationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
