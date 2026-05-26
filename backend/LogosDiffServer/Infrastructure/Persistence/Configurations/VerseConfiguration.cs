using Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastucture.Persistence.Configurations
{
    public class VerseConfiguration : IEntityTypeConfiguration<Verse>
    {
        public void Configure(EntityTypeBuilder<Verse> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Number)
                .IsRequired();

            builder.Property(v => v.Content)
                .IsRequired();

            builder.HasOne(v => v.Chapter)
                .WithMany()
                .HasForeignKey(v => v.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.Translation)
                .WithMany(t => t.Verses)
                .HasForeignKey(v => v.TranslationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
